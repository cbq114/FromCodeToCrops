using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DayEndController : MonoBehaviour
{
    public TMP_Text dayText;

    public string wakeUpScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TimeController.instance != null)
        {
            dayText.text = "- Day " + TimeController.instance.currentDay + " -";
        }

        AudioManager.instance.PauseMusic();


        AudioManager.instance.PlaySFX(1);
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Đừng gọi TimeController.StartDay() ở đây
            // TimeController.instance.StartDay();

            // Chỉ tiếp tục nhạc và chuyển scene
            AudioManager.instance.ResumeMusic();

            // QUAN TRỌNG: Vẫn giữ nguyên PlayerPrefs "Transition" để SceneTransitionManager xử lý
            // PlayerPrefs.SetString("Transition", "Wake Up"); - đã được đặt trong TimeController.EndDay()

            SceneManager.LoadScene(wakeUpScene);
        }
    }
}
