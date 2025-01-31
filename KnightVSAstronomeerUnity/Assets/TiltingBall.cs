using UnityEngine;

public class TiltingBall : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        animator.Play("tilt");
    }
}
