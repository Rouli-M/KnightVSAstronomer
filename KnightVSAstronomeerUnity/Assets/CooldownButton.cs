using UnityEngine;
using UnityEngine.UI;

public class CooldownButton : MonoBehaviour
{
    float total_time, current_time;
    public Image circleCD;
    public Button button;
    public Color disabledColor;

    public void SetCooldown(float time)
    {
        total_time = Mathf.Abs(time);
        current_time = time;

        if(time > 0)
        {
            // cant use for time
            button.interactable = false;
            circleCD.color = disabledColor;
        }
        else if (time < 0)
        {
            // can use for -time
            button.interactable = true;
            circleCD.color = Color.white;
        }
        else
        {
            button.interactable = true;
            circleCD.color = Color.white;
            circleCD.fillAmount = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
