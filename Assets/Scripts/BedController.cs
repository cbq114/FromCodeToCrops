using UnityEngine;
using UnityEngine.InputSystem;

public class BedController : MonoBehaviour
{
    private bool canSleep;

    private void Update()
    {
        if (canSleep)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            {
                Sleep();
            }
        }
    }

    private void Sleep()
    {
        // Chuyển sang ngày tiếp theo
        TimeController.instance.EndDay();
        
        // Phục hồi toàn bộ thể lực
        PlayerController.instance.currentStamina = PlayerController.instance.maxStamina;
        PlayerController.instance.UpdateStaminaUI();
        
        AudioManager.instance.PlaySFX(6); // Âm thanh ngủ/thức dậy
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canSleep = true;
            UIController.instance.ShowSleepPrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canSleep = false;
            UIController.instance.ShowSleepPrompt(false);
        }
    }
}
