using Unity.Cinemachine;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void Trigger()
    {
        impulseSource.GenerateImpulse(new Vector3(0.5f,0.5f));
    }
}