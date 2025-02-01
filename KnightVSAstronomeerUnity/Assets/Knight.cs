using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class Knight : MonoBehaviour
{
    Rigidbody rb;
    public Rigidbody astronomeer_rb;
    public Astronomer astronomeer;
    public float current_max_velocity = 5;
    public float rot_vel;
    public float y_velocity;
    public float ground_y_level;
    public AnimationCurve jumpCurveBig, jumpCurveSmall;
    public float jump_time;
    public Coroutine jumpCoroutine, rushingCoroutine;
    public int midair_jumps_left = 0;
    public float timeSinceLastAttack= 0f;
    public LayerMask groundLayer;
    public enum State { walking_around, rushing, attacking, retreating, jumping, intro, ending }
    public State state;
    Animator animator;
    public AudioClipPlayRandomizer swordHitSound, jumpSound, jumpSound2, stepSound;
    public AudioClip roll;
    public AudioSource hitAudio;

    public PlayableDirector playableDirector;
    public TimelineAsset comeBackTimeline, charge_attack_timeline, attack_timeline, ending_timeline, intro_timeline;
    public ShakeCamera shake;
    public Vector3 initialAttackPosition;
    public AudioSource music;

    public CooldownButton jumpButton, attackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        animator.GetBehaviour<HandyBehaviour>().onStateEnterEvent("attack1", () =>
        {
            shake.Trigger();
            swordHitSound.PlayRandom();
            timeSinceLastAttack = 0f;
            astronomeer.TakeDamage();
        });
        animator.GetBehaviour<HandyBehaviour>().onStateEnterEvent("attack2", () =>
        {
            shake.Trigger();
            swordHitSound.PlayRandom();
            timeSinceLastAttack = 0f;
            astronomeer.TakeDamage();
        });
        animator.GetBehaviour<HandyBehaviour>().onStateEnterEvent("roll", () =>
        {
            GetComponent<AudioSource>().clip = roll;
            GetComponent<AudioSource>().Play();
        });
        rot_vel = current_max_velocity;
    }

    // Update is called once per frame
    void Update()
    {
        rb.DOLookAt(astronomeer_rb.transform.position, 0);
        //transform.LookAt(astronomeer.transform);

        if(state == State.walking_around || state == State.jumping)
        {
            if(rot_vel < current_max_velocity)
            {
                rot_vel += Time.deltaTime * 5;
            }
            else if (rot_vel > current_max_velocity)
            {
                rot_vel = current_max_velocity;
            }
        }

        if(state == State.attacking)
        {
            // allow repeated attack
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack > 1.2f)
            {
                ComeBack();
            }
        }
    }

    public void StartTurning()
    {
        state = State.walking_around;
        animator.Play("walk");
        attackButton.SetCooldown(5f);
        music.Play();
    }


    private void ComeBack()
    {
        state = State.retreating;
        playableDirector.Play(comeBackTimeline);
    }

    public void FixedUpdate()
    {
        if(state == State.walking_around || state == State.jumping)
            transform.RotateAround(astronomeer.transform.position, Vector3.up, -rot_vel * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            ground_y_level = transform.position.y;

            if (state == State.jumping)
            {
                animator.Play("roll");
                state = State.walking_around ;
            }
        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
            if(state == State.rushing)
            {
                StopCoroutine(rushingCoroutine);
                ComeBack();
                animator.Play("knocked");
            }
        }
        else if (collision.transform.CompareTag("Spell"))
        {
            if(state == State.walking_around || state == State.jumping)
            {
                GetComponentInChildren<Animator>().Play("blink", 1, 0f);
                hitAudio.Play();
                rot_vel = -1;
            }
        }
    }

    public void Jump()
    {
        Debug.Log("Knight.jump");
        if (state != State.jumping && state != State.walking_around)
            return;

        //rb.linearVelocity = Vector3.up * 12;
        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);

        if(isGrounded())
        {
            jumpSound.PlayRandom();
            midair_jumps_left = 1;
            jumpCoroutine = StartCoroutine(JumpEnumerator(false));
        }
        else if(midair_jumps_left>0)
        {
            jumpSound2.PlayRandom();
            midair_jumps_left--;
            jumpCoroutine = StartCoroutine(JumpEnumerator(true));
        }
    }

    public void TryPlayStepSound()
    {
        if (state == State.walking_around)
            stepSound.PlayRandom();
    }

    public IEnumerator JumpEnumerator(bool small = false)
    {
        animator.Play("jump", 0, 0f);
        state = State.jumping;

        AnimationCurve curve = jumpCurveBig;
        if (small) curve = jumpCurveSmall;
        float time = 0f;
        while(time < curve.keys[curve.length - 1].time)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, curve.Evaluate(time), rb.linearVelocity.z);
            time += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, curve.keys[curve.length - 1].value, rb.linearVelocity.z);
        yield return null;
    }

    public IEnumerator ChargeAttackCoroutine()
    {
        initialAttackPosition = transform.position;
        playableDirector.Play(charge_attack_timeline); // will play the animation and set the camera
        state = State.rushing;

        Vector3 start = transform.position;
        Vector3 target = astronomeer.transform.position - transform.forward * 2f;

        float time = 0f;
        float total_time = 0.67f;
        while (time < total_time)
        {
            transform.position = Vector3.Lerp(start, target, time/ total_time);
            time += Time.deltaTime;
            yield return null;
        }

        playableDirector.Play(attack_timeline);
        state = State.attacking;
        animator.Play("attack1"); // playing it here instead of in timeline to use trigger
        attackButton.SetCooldown(-1.2f);

        timeSinceLastAttack = 0f;
        yield return null;
    }

    public void Attack()
    {
        //if (state == State.rushing || state == State.retreating || state == State.attacking) return; // prevent attack while attack

        if (state == State.attacking)
        {
            animator.SetTrigger("attack");
            return;
        }
        if (state != State.walking_around) return; // prevent attack while attack

        if (rushingCoroutine != null) 
            StopCoroutine(rushingCoroutine);
        rushingCoroutine = StartCoroutine(ChargeAttackCoroutine());

        // allow attack for a little while (this will be overwritten by jump back)
        attackButton.SetCooldown(10000);
        jumpButton.SetCooldown(10000f);
    }



    public void JumpBack()
    {
        DG.Tweening.Sequence jumpback = rb.DOJump(initialAttackPosition, 8f,1, 0.8f);
        jumpback.OnComplete<DG.Tweening.Sequence>(() => { 
            state = State.walking_around;
            animator.Play("walk");
            attackButton.SetCooldown(6f);
            jumpButton.SetCooldown(0f);

            astronomeer.lifeHUDAnimator.SetBool("show", false);

            if (astronomeer.life <= 0f)
            {
                //attackButton.SetCooldown(9999f);
                //jumpButton.SetCooldown(9999f);
                state = State.ending;
                playableDirector.Play(ending_timeline);
                music.Stop();
            }
        });
    }


    public IEnumerator AttackEnumerator()
    {
        yield return null;
    }

    private bool isGrounded()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        float radius = capsule.radius * 0.9f;
        //get the position (assuming its right at the bottom) and move it up by almost the whole radius
        Vector3 pos = transform.position + Vector3.up * (radius * 0.9f - capsule.height / 2);

        //returns true if the sphere touches something on that layer
        return Physics.CheckSphere(pos, radius, groundLayer);
    }
}
