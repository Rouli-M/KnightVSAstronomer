using UnityEngine;

public class fireball : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
            Destroy(gameObject);
        else
        {
            ParticleSystem ps = transform.GetChild(1).GetComponent<ParticleSystem>();
            ps.transform.SetParent(null);
            ps.Play();
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        transform.LookAt(transform.position + transform.GetComponent<Rigidbody>().linearVelocity);
    }
}
