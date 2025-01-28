using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    public Coroutine jumpCoroutine;
    public int midair_jumps_left = 0;
    public LayerMask groundLayer;
    public bool turning = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rot_vel = current_max_velocity;
    }

    // Update is called once per frame
    void Update()
    {
        rb.DOLookAt(astronomeer_rb.transform.position, 0);
        //transform.LookAt(astronomeer.transform);

        if(turning)
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
        
    }

    public void FixedUpdate()
    {
        if(turning)
            transform.RotateAround(astronomeer.transform.position, Vector3.up, -rot_vel * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
            ground_y_level = transform.position.y;
        else if (collision.transform.CompareTag("Spell"))
        {
            GetComponent<Animator>().Play("blink", 1, 0f);
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

    public void Attack()
    {
        if (!turning) return; // prevent attack while attack
        turning = false;
        TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> approachTween = rb.DOMove(astronomeer.transform.position - transform.forward * 2f, 0.25f, false);
        approachTween.onComplete += () => { astronomeer.TakeDamage(); };
        TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> gobackTween = rb.DOMove(transform.position, 0.4f);
        gobackTween.onComplete += () => { turning = true; };

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(approachTween);
        sequence.Append(gobackTween);
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
