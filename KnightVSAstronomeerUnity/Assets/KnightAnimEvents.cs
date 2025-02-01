using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class KnightAnimEvents : MonoBehaviour
{
    public UnityEvent jumpback, stepSound;

    public void JumpBack()
    {
        jumpback.Invoke();
    }

    public void StepSound()
    {
        stepSound.Invoke();
    }
}
