using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    public bool hasPendingWeatherForecast = false;
    private string pendingWeatherForecast = "";

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

    }

    public float currentTime;

    public float dayStart, dayEnd;

    [Header("Time Settings")]
    [Range(0.05f, 1.0f)] public float timeSpeed = 0.25f;

    private bool timeActive;

    public int currentDay = 1;

    public string dayEndScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = dayStart;

        timeActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeActive == true)
        {
            currentTime += Time.deltaTime * timeSpeed;

            if (currentTime > dayEnd)
            {
                currentTime = dayEnd;
                EndDay();
            }

            if (UIController.instance != null)
            {
                UIController.instance.UpdateTimeText(currentTime);
            }
        }
    }

    public void EndDay()
    {
        Debug.Log("Đang thực hiện EndDay");
        timeActive = false;
        currentDay++;

        // Thêm this để chắc chắn đúng instance được gọi
        if (GridInfo.instance != null)
        {
            GridInfo.instance.GrowCrop();
        }

        // Thêm logic cho hệ thống mùa vụ
        if (SeasonSystem.instance != null)
        {
            SeasonSystem.instance.NewDay();
        }
        //hồi thể lực khi hết ngày
        if (PlayerController.instance != null)
        {
            PlayerController.instance.UpdateStaminaUI();

        }
        OnNewDay();

        // Kiểm tra xem PlayerPrefs và SceneManager có hoạt động không
        try
        {
            PlayerPrefs.SetString("Transition", "Wake Up");
            SceneManager.LoadScene(dayEndScene);
            Debug.Log("Đã load scene mới: " + dayEndScene);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi chuyển scene: " + e.Message);
        }
    }

    public void StartDay()
    {
        timeActive = true;
        currentTime = dayStart;
        AudioManager.instance.PlaySFX(6);

        // Thêm kiểm tra thời tiết cho ngày mới
        if (WeatherSystem.instance != null)
        {
            WeatherSystem.instance.CheckWeatherForNewDay();

            // Thay vì hiển thị ngay, lưu lại dự báo để hiển thị sau
            if (SceneManager.GetActiveScene().name == "House" || SceneManager.GetActiveScene().name == "DayEnd")
            {
                // Đang trong nhà, lưu trữ dự báo để hiển thị sau
                pendingWeatherForecast = WeatherSystem.instance.GetWeatherForecast();
                hasPendingWeatherForecast = true;
                Debug.Log("Lưu trữ dự báo thời tiết để hiển thị khi ra ngoài: " + pendingWeatherForecast);
            }
            else
            {
                // Đang ở ngoài trời, hiển thị bình thường
                StartCoroutine(ShowWeatherForecastDelayed());
            }
        }

        // Cơ hội tìm thấy thức ăn thú cưng mỗi ngày mới
        if (Random.value < 0.3f) // 30% cơ hội mỗi ngày
        {
            PetMenuController petMenu = FindObjectOfType<PetMenuController>();
            if (petMenu != null)
                petMenu.AddPetFood(1);
        }
    }
    // Sửa trong TimeController.cs
    private IEnumerator ShowWeatherForecastDelayed()
    {
        // Thêm debug để theo dõi
        Debug.Log("Bắt đầu hiển thị dự báo thời tiết...");

        // Đợi nhiều frame hơn để đảm bảo mọi thành phần đã sẵn sàng
        yield return new WaitForSeconds(0.5f);

        Debug.Log("WeatherSystem sẵn sàng: " + (WeatherSystem.instance != null));
        Debug.Log("UIController sẵn sàng: " + (UIController.instance != null));

        if (WeatherSystem.instance != null && UIController.instance != null)
        {
            string forecast = WeatherSystem.instance.GetWeatherForecast();
            Debug.Log("Dự báo cơ bản: " + forecast);
            UIController.instance.ShowMessage(forecast);
        }
        else
        {
            Debug.LogError("Không thể hiển thị dự báo: WeatherSystem hoặc UIController là null");
        }
    }

    // Sửa phương thức ShowPendingWeatherForecast trong TimeController.cs
    public void ShowPendingWeatherForecast()
    {
        Debug.Log($"ShowPendingWeatherForecast được gọi. hasPendingWeatherForecast={hasPendingWeatherForecast}, forecast={pendingWeatherForecast}");

        if (hasPendingWeatherForecast && !string.IsNullOrEmpty(pendingWeatherForecast))
        {
            Debug.Log("Điều kiện thỏa mãn, hiển thị dự báo thời tiết");

            if (UIController.instance != null)
            {
                Debug.Log("UIController.instance OK, gọi ShowMessage");
                UIController.instance.ShowMessage(pendingWeatherForecast);

                // Lấy dự báo chi tiết từ API nếu có
                if (WeatherSystem.instance != null)
                {
                    WeatherSystem.instance.GetWeatherForecast();
                }

                // Reset các biến sau khi hiển thị
                hasPendingWeatherForecast = false;
                pendingWeatherForecast = "";
            }
            else
            {
                Debug.LogError("UIController.instance là null");
            }
        }
        else
        {
            Debug.Log("Không có dự báo thời tiết cần hiển thị");
        }
    }

    private void OnNewDay()
    {
        // Các code hiện tại của OnNewDay

        // Thêm dòng này để tự động lưu khi sang ngày mới
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    public void ForceSavePendingForecast()
    {
        if (WeatherSystem.instance != null)
        {
            pendingWeatherForecast = WeatherSystem.instance.GetWeatherForecast();
            hasPendingWeatherForecast = true;
            Debug.Log("Đã lưu trữ dự báo thời tiết: " + pendingWeatherForecast);
        }
    }
}
