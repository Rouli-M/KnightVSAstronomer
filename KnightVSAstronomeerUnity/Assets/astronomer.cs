using UnityEngine;

public class Astronomer : MonoBehaviour
{
    public GameObject knight;
    public GameObject spellprefab;
    public float radius;


    public float spell_cooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        radius = (knight.transform.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (spell_cooldown > 0)
        {
            spell_cooldown -= Time.deltaTime;
            if(spell_cooldown <= 0)
            {
                ShootFireball();
                spell_cooldown = Random.Range(3, 6);
            }
        }
    }

    public void TakeDamage()
    {
        GetComponent<Animator>().Play("blink", 1, 0f);
    }

    public void ShootFireball()
    {
        GameObject fireball = Instantiate(spellprefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        fireball.transform.position = transform.position;

        float player_hit_time = 2f;// we want to hit the player in 2 seconds
        float spell_velocity = radius / player_hit_time;

        float angle_offset = knight.GetComponent<Knight>().current_max_velocity / spell_velocity;
        angle_offset = Mathf.Rad2Deg * angle_offset * 0.5f;
        Debug.Log("spell angle offset in degree = " + angle_offset);

        fireball.transform.LookAt(new Vector3(knight.transform.position.x, knight.GetComponent<Knight>().ground_y_level, knight.transform.position.z)); // dont aim at jumping knight
        fireball.transform.Rotate(Vector3.up, -angle_offset, Space.World);

        fireball.GetComponent<Rigidbody>().linearVelocity = fireball.transform.forward * spell_velocity;
    }
}
