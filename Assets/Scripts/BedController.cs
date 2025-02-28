using UnityEngine;
using UnityEngine.InputSystem;

public class BedController : MonoBehaviour
{
    private bool canSleep;

private void Update()
{
    // Kiểm tra debug
    if (canSleep)
    {
        Debug.Log("Đang trong khu vực giường ngủ, chờ nhấn phím Space/E");
        
        // Kiểm tra input theo nhiều cách
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool ePressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        bool oldInputSpace = Input.GetKeyDown(KeyCode.Space);  // Kiểm tra cả old input system
        bool oldInputE = Input.GetKeyDown(KeyCode.E);
        
        if (spacePressed || ePressed || oldInputSpace || oldInputE)
        {
            Debug.Log("Phát hiện phím ngủ được nhấn: Space=" + spacePressed + ", E=" + ePressed + 
                     ", Old-Space=" + oldInputSpace + ", Old-E=" + oldInputE);
            Sleep();
        }
    }
}

private void Sleep()
{
    Debug.Log("Bắt đầu thực hiện hàm Sleep");
    
    // Phục hồi thể lực TRƯỚC KHI kết thúc ngày
    PlayerController.instance.currentStamina = PlayerController.instance.maxStamina;
    PlayerController.instance.UpdateStaminaUI();
    
    // Chuyển sang ngày tiếp theo (đặt cuối cùng vì có thể gây load scene)
    TimeController.instance.EndDay();
    
    AudioManager.instance.PlaySFX(6); // Âm thanh ngủ/thức dậy
}

// Trong BedController.cs
private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.tag == "Player")
    {
        canSleep = true;
        UIController.instance.ShowSleepPrompt(true);
        // Thêm dòng debug để kiểm tra
        Debug.Log("Player đã vào vùng giường ngủ, canSleep = " + canSleep);
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
