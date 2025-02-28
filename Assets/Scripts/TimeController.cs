using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

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

    public float timeSpeed = .25f;

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

    // Kiểm tra xem PlayerPrefs và SceneManager có hoạt động không
    try {
        PlayerPrefs.SetString("Transition", "Wake Up");
        SceneManager.LoadScene(dayEndScene);
        Debug.Log("Đã load scene mới: " + dayEndScene);
    }
    catch (System.Exception e) {
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
        }
    }
}
