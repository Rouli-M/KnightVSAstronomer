using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Astronomer : MonoBehaviour
{
    public GameObject knight;
    public GameObject spellprefab;
    public float radius;
    Animator animator;
    public float life = 1f;
    public Image lifeFill, lifeWhiteFill;

    public float spell_cooldown;
    public Animator lifeHUDAnimator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.GetBehaviour<HandyBehaviour>().onStateExitEvent("hurt", () => 
        {
            if (spell_cooldown < 2f) 
                spell_cooldown += 2f; 
        });

        animator.GetBehaviour<HandyBehaviour>().onStateEnterEvent("idle", () =>
        {
            lifeWhiteFill.fillAmount = lifeFill.fillAmount;
        });
        radius = (knight.transform.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (knight.GetComponent<Knight>().state == Knight.State.intro)
            return;

        if (life <= 0f)
            return;

        if (spell_cooldown > 0)
        {
            spell_cooldown -= Time.deltaTime;
            
            if(spell_cooldown <= 0)
            {
                //ShootFireball();
                animator.Play("cast_fireball"); // anim event will call ShootFireball
                spell_cooldown = Random.Range(3, 6);
                spell_cooldown *= 0.2f + life * 0.8f;
            }
        }

        animator.SetFloat("speed", 1 + (1-life) * 1.5f);
    }

    public void TakeDamage()
    {
        animator.Play("hurt",0,0f);
        GetComponent<AudioClipPlayRandomizer>().PlayRandom();
        life -= 0.025f;
        lifeHUDAnimator.SetBool("show", true);

        Canvas canvas = lifeHUDAnimator.GetComponentInParent<Canvas>();
        Vector3 canvasLookAt = new Vector3(-knight.transform.position.x, canvas.transform.position.y, -knight.transform.position.z);
        canvas.transform.LookAt(canvasLookAt, Vector3.up);
        // make UI face player? idk
        //FindObjectOfType<CinemachineBrain>().ParentCamera

        lifeFill.fillAmount = life;
        //GetComponent<Animator>().Play("blink", 1, 0f);
    }

    public void ShootFireball()
    {

        float player_hit_time = 2f;// we want to hit the player in 2 seconds
        float spell_velocity = radius / player_hit_time;

        float angle_offset = knight.GetComponent<Knight>().current_max_velocity / spell_velocity;
        angle_offset = Mathf.Rad2Deg * angle_offset * 0.5f;


        Vector3 AstronomerLookAtDirection = new Vector3(knight.transform.position.x, transform.position.y, knight.transform.position.z);
        transform.LookAt(AstronomerLookAtDirection);
        transform.Rotate(Vector3.up, -angle_offset, Space.World);


        Vector3 fireballSpawnPos = transform.position + Vector3.up * 1f + transform.forward;
        GameObject fireball = Instantiate(spellprefab, fireballSpawnPos, Quaternion.identity);

        Debug.Log("spell angle offset in degree = " + angle_offset);


        fireball.transform.LookAt(new Vector3(knight.transform.position.x, knight.GetComponent<Knight>().ground_y_level, knight.transform.position.z)); // dont aim at jumping knight
        fireball.transform.Rotate(Vector3.up, -angle_offset, Space.World);
        fireball.GetComponent<Rigidbody>().linearVelocity = fireball.transform.forward * spell_velocity;
    }
}
