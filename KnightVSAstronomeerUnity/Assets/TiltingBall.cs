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
        if (Time.unscaledTime < 2f)
            return;
        Debug.Log("tilting head collision detected");
        animator.Play("tilt",0,0f);
    }
}
