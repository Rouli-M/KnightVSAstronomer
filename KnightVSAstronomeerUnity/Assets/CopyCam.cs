using UnityEngine;

public class CopyCam : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        // Set the current camera's settings from the main Scene camera
        cam.CopyFrom(Camera.main);
    }

    public void LateUpdate()
    {
        cam.CopyFrom(Camera.main);
    }
}
