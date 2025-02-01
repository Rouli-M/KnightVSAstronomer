using Unity.Cinemachine;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void Trigger()
    {
        impulseSource.GenerateImpulse(0.3f * new Vector3(1f,1f));
    }
}