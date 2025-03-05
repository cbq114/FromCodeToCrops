// SceneTransitionManager.cs - tạo mới nếu chưa có
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    public bool isTransitioningToNewDay = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Thêm event listener khi scene được tải
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Xóa listener khi không cần nữa
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene {scene.name} đã được tải, kiểm tra transition: {PlayerPrefs.GetString("Transition", "Không có")}");

        // Kiểm tra nếu đang chuyển từ màn hình DayEnd về Main hoặc House
        string transition = PlayerPrefs.GetString("Transition", "");

        if (transition == "Wake Up" && (scene.name == "Main" || scene.name == "House"))
        {
            Debug.Log("Đang thức dậy và bắt đầu ngày mới!");
            PlayerPrefs.DeleteKey("Transition"); // Xóa flag

            // Đợi một frame để đảm bảo mọi thứ đã được khởi tạo
            StartCoroutine(StartDayNextFrame());
        }
    }

    private IEnumerator StartDayNextFrame()
    {
        yield return null; // Wait one frame

        // Start the new day
        if (TimeController.instance != null)
        {
            isTransitioningToNewDay = true;
            Debug.Log("SceneTransitionManager: Calling TimeController.StartDay()");

            // Gọi StartDay và chờ 0.5 giây để đảm bảo các giá trị được khởi tạo
            TimeController.instance.StartDay();

            // Đảm bảo rằng nếu người chơi ở trong nhà, hasPendingWeatherForecast được đặt đúng
            if (SceneManager.GetActiveScene().name == "House")
            {
                yield return new WaitForSeconds(0.5f);
                if (!TimeController.instance.hasPendingWeatherForecast)
                {
                    Debug.Log("Đặt lại hasPendingWeatherForecast thành true sau khi StartDay");
                    TimeController.instance.hasPendingWeatherForecast = true;
                    TimeController.instance.ForceSavePendingForecast();
                }
            }

            isTransitioningToNewDay = false;
        }
        else
        {
            Debug.LogWarning("TimeController.instance is null when trying to start day");
        }
    }
}