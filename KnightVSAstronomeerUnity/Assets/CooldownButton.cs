using UnityEngine;
using UnityEngine.UI;

public class CooldownButton : MonoBehaviour
{
    float total_time, current_time;
    public Image circleCD;
    public Button button;

    public void SetCooldown(float time)
    {
        total_time = time;
        current_time = time;
        button.interactable = false;
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
                current_time = 0f;
            }
        }

        circleCD.fillAmount = 1 - current_time / (float) total_time;
    }
}
