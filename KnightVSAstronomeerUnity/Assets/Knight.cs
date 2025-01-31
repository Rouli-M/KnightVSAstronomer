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
    public enum State { walking_around, rushing, attacking, retreating, jumping, intro }
    public State state;
    Animator animator;

    public PlayableDirector playableDirector;
    public TimelineAsset comeBackTimeline, charge_attack_timeline, attack_timeline;
    public ShakeCamera shake;
    public Vector3 initialAttackPosition;

    public CinemachineCamera runCam, attackCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animator.GetBehaviour<HandyBehaviour>().onStateExitEvent("challenge", () => 
        { 
            state = State.walking_around;
            animator.Play("walk");
        });
        animator.GetBehaviour<HandyBehaviour>().onStateExitEvent("attack1", () =>
        {
            shake.Trigger();
            timeSinceLastAttack = 0f;
            //damage astronomeer
        });
        animator.GetBehaviour<HandyBehaviour>().onStateExitEvent("attack2", () =>
        {
            shake.Trigger();
            timeSinceLastAttack = 0f;
            //damage astronomeer
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
            GetComponentInChildren<Animator>().Play("blink", 1, 0f);
            ParticleSystem ps = collision.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.transform.SetParent(null);
            ps.Play();
            Destroy(collision.gameObject);
            rot_vel = -1;
        }
    }

    public void Jump()
    {
        Debug.Log("Knight.jump");
        //rb.linearVelocity = Vector3.up * 12;
        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);

        if(isGrounded())
        {
            midair_jumps_left = 1;
            jumpCoroutine = StartCoroutine(JumpEnumerator(false));
        }
        else if(midair_jumps_left>0)
        {
            midair_jumps_left--;
            jumpCoroutine = StartCoroutine(JumpEnumerator(true));
        }
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
    }


    public void JumpBack()
    {
        DG.Tweening.Sequence jumpback = rb.DOJump(initialAttackPosition, 8f,1, 0.8f);
        jumpback.OnComplete<DG.Tweening.Sequence>(() => { 
            state = State.walking_around;
            animator.Play("walk");
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
