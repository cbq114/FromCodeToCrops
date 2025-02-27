// Tạo file WeatherSystem.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem instance;
    
    [Header("Weather Settings")]
    public GameObject rainEffect; // Prefab hiệu ứng mưa (particles)
    public float rainChance = 0.2f; // Xác suất mưa mỗi ngày
    public float rainDuration = 3f; // Thời gian mưa (giờ)
    
    private bool isRaining = false;
    
    private void Awake()
    {
        instance = this;
        rainEffect.SetActive(false);
    }
    
    // Gọi hàm này khi bắt đầu ngày mới
    public void CheckWeatherForNewDay()
    {
        if (Random.value < rainChance)
        {
            StartRain();
        }
    }
    
    private void StartRain()
    {
        isRaining = true;
        rainEffect.SetActive(true);
        
        // Tính toán thời gian kết thúc mưa dựa trên thời gian trong game
        float rainEndTime = TimeController.instance.currentTime + rainDuration;
        if (rainEndTime > TimeController.instance.dayEnd)
            rainEndTime = TimeController.instance.dayEnd;
            
        StartCoroutine(StopRainAt(rainEndTime));
    }
    
    private IEnumerator StopRainAt(float endTime)
    {
        // Đợi cho đến khi đến thời gian kết thúc
        while (TimeController.instance.currentTime < endTime && isRaining)
        {
            yield return null;
        }
        
        isRaining = false;
        rainEffect.SetActive(false);
    }
    
    // Kiểm tra xem có đang mưa không
    public bool IsRaining()
    {
        return isRaining;
    }
    
    // Tăng tốc độ phát triển cây trồng khi trời mưa
    public void ApplyRainBoostToCrops()
    {
        if (isRaining)
        {
            // Tăng tốc độ phát triển của tất cả cây trồng
            GridInfo.instance.RainBoostAllCrops();
        }
    }
}