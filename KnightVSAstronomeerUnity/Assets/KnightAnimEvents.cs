using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class KnightAnimEvents : MonoBehaviour
{
    public UnityEvent jumpback, stepSound, stepSound2;

    public void JumpBack()
    {
        jumpback.Invoke();
    }

    public void StepSound()
    {
        stepSound.Invoke();
    }

    public void StepSound2()
    {
        stepSound2.Invoke();
    }
}
