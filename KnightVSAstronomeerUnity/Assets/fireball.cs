using UnityEngine;

public class fireball : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
            Destroy(gameObject);
    }
}
