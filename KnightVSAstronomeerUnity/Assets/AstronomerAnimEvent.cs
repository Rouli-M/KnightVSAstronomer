using UnityEngine;
using UnityEngine.Events;

public class AstronomerAnimEvent : MonoBehaviour
{
    public UnityEvent castballEvent;
    public void CastBall()
    {
        castballEvent.Invoke();
    }
}
