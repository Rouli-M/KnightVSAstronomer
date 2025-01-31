using Unity.Cinemachine;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void Trigger()
    {
        impulseSource.GenerateImpulse(0.1f * new Vector3(0.75f,0.75f));
    }
}