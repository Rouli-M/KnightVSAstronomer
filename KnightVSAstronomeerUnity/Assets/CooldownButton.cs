using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CooldownButton : MonoBehaviour
{
    float total_time, current_time;
    public Image circleCD;
    public Button button;
    public Color disabledColor;
    public Key overrideKey;

    public GameObject tap_anim;

    public void SetCooldown(float time)
    {
        total_time = Mathf.Abs(time);
        current_time = time;

        if(time > 0)
        {
            // cant use for time
            button.interactable = false;
            tap_anim.SetActive(false);
            circleCD.color = disabledColor;
        }
        else if (time < 0)
        {
            // can use for -time
            button.interactable = true;
            circleCD.color = Color.white;
            tap_anim.SetActive(true);
        }
        else
        {
            button.interactable = true;
            circleCD.color = Color.white;
            circleCD.fillAmount = 1f;
            tap_anim.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(overrideKey != Key.None)
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            if (Keyboard.current[overrideKey].wasPressedThisFrame && GetComponent<Button>().interactable)
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }


        if(current_time>0f)
        {
            current_time-=Time.deltaTime;
            if (current_time <= 0f)
            {
                button.interactable = true;
                circleCD.color = Color.white;
                current_time = 0f;
            }
            circleCD.fillAmount = 1 - current_time / (float)total_time;
        }
        else if (current_time < 0f)
        {
            current_time += Time.deltaTime;
            if(current_time >=0f)
            {
                // disable button
                SetCooldown(999f);
            }

            circleCD.fillAmount = - current_time / (float)total_time;
        }

    }
}
