using UnityEngine;
using System.Collections;

public class WeatherSystem : MonoBehaviour
{
	public static WeatherSystem instance;

	public enum WeatherType
	{
		Clear,
		Rain,
		Storm,
		Fog
	}

	[Header("Weather Settings")]
	public GameObject rainEffect;
	public float rainChance = 0.2f;
	public float rainDuration = 3f;

	[Header("Additional Effects")]
	public GameObject stormEffect;
	public GameObject snowEffect;

	public bool isRaining = false;
	public bool isStorming = false;
	public bool isSnowing = false;
	private bool hasDetailedForecastPending = false;
	public WeatherType currentWeatherType = WeatherType.Clear;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);

		rainEffect.SetActive(false);
		if (stormEffect != null) stormEffect.SetActive(false);
		if (snowEffect != null) snowEffect.SetActive(false);
	}

	public void CheckWeatherForNewDay()
	{
		WeatherAI weatherAI = GetComponent<WeatherAI>();
		if (weatherAI != null)
		{
			currentWeatherType = weatherAI.PredictWeather();
			ApplyWeather(currentWeatherType);
		}
		else
		{
			float chance = Random.value;
			if (chance < rainChance * 0.3f) // 30% of rainChance for storm
				StartStorm();
			else if (chance < rainChance)
				StartRain();
			else if (chance < rainChance * 1.5f && SeasonSystem.instance.currentSeason == SeasonSystem.Season.Winter)
				StartSnow();
			else
				ApplyWeather(WeatherType.Clear);
		}
	}

	public void ApplyWeather(WeatherType weatherType)
	{
		rainEffect.SetActive(false);
		if (stormEffect != null) stormEffect.SetActive(false);
		if (snowEffect != null) snowEffect.SetActive(false);

		switch (weatherType)
		{
			case WeatherType.Rain:
				rainEffect.SetActive(true);
				isRaining = true;
				break;
			case WeatherType.Storm:
				if (stormEffect != null) stormEffect.SetActive(true);
				isStorming = true;
				break;
			case WeatherType.Fog:
				// Add fog effect logic here if implemented
				break;
			case WeatherType.Clear:
			default:
				isRaining = false;
				isStorming = false;
				isSnowing = false;
				break;
		}
	}

	private void StartRain()
	{
		isRaining = true;
		rainEffect.SetActive(true);
		currentWeatherType = WeatherType.Rain;

		float rainEndTime = TimeController.instance.currentTime + rainDuration;
		if (rainEndTime > TimeController.instance.dayEnd)
			rainEndTime = TimeController.instance.dayEnd;

		StartCoroutine(StopRainAt(rainEndTime));
	}

	private void StartStorm()
	{
		isRaining = true;
		isStorming = true;
		rainEffect.SetActive(true);
		if (stormEffect != null) stormEffect.SetActive(true);
		currentWeatherType = WeatherType.Storm;

		if (AudioManager.instance != null)
			AudioManager.instance.PlaySFX(9);

		float stormEndTime = TimeController.instance.currentTime + rainDuration * 0.7f;
		if (stormEndTime > TimeController.instance.dayEnd)
			stormEndTime = TimeController.instance.dayEnd;

		StartCoroutine(StopStormAt(stormEndTime));
	}

	private void StartSnow()
	{
		isSnowing = true;
		if (snowEffect != null) snowEffect.SetActive(true);
		currentWeatherType = WeatherType.Clear; // Snow isn't a WeatherType yet, using Clear as placeholder

		if (AudioManager.instance != null)
			AudioManager.instance.PlaySFX(10);

		float snowEndTime = TimeController.instance.currentTime + rainDuration;
		if (snowEndTime > TimeController.instance.dayEnd)
			snowEndTime = TimeController.instance.dayEnd;

		StartCoroutine(StopSnowAt(snowEndTime));
	}

	private IEnumerator StopRainAt(float endTime)
	{
		while (TimeController.instance.currentTime < endTime && isRaining)
			yield return null;

		isRaining = false;
		rainEffect.SetActive(false);
		currentWeatherType = WeatherType.Clear;
	}

	private IEnumerator StopStormAt(float endTime)
	{
		while (TimeController.instance.currentTime < endTime && isStorming)
			yield return null;

		isRaining = false;
		isStorming = false;
		rainEffect.SetActive(false);
		if (stormEffect != null) stormEffect.SetActive(false);
		currentWeatherType = WeatherType.Clear;
	}

	private IEnumerator StopSnowAt(float endTime)
	{
		while (TimeController.instance.currentTime < endTime && isSnowing)
			yield return null;

		isSnowing = false;
		if (snowEffect != null) snowEffect.SetActive(false);
		currentWeatherType = WeatherType.Clear;
	}

	public bool IsRaining() => isRaining;

	public void ApplyRainBoostToCrops()
	{
		if (isRaining)
			GridInfo.instance.RainBoostAllCrops();
	}

	// Chỉnh sửa phương thức hiện tại để không sử dụng .Result
	public string GetWeatherForecast()
    {
        string basicForecast = $"Today's weather: {GetWeatherName(currentWeatherType)}";
        StartCoroutine(GetDetailedForecastAsync()); // Tự động lấy dự báo chi tiết
        return basicForecast;
    }

    private string GetWeatherName(WeatherType type)
    {
        switch (type)
        {
            case WeatherType.Clear:
                return "Clear";
            case WeatherType.Rain:
                return "Rain";
            case WeatherType.Storm:
                return "Storm";
            case WeatherType.Fog:
                return "Fog";
            default:
                return "Unknown";
        }
    }

    private IEnumerator GetDetailedForecastAsync()
    {
        if (GeminiAPIClient.instance != null)
        {
            var task = GeminiAPIClient.instance.GetWeatherDescription(
                currentWeatherType.ToString(),
                SeasonSystem.instance.currentSeason.ToString(),
                TimeController.instance.currentDay);

            // Đợi API hoàn thành
            while (!task.IsCompleted)
                yield return null;

            if (!task.IsFaulted && task.IsCompletedSuccessfully && UIController.instance != null)
            {
                string detailedForecast = task.Result;
                Debug.Log($"Dự báo chi tiết: {detailedForecast}");
                UIController.instance.ShowMessage($"Weather forecast: {detailedForecast}");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Lỗi API thời tiết: " + (task.Exception != null ? task.Exception.Message : "Lỗi không xác định"));
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("Không thể lấy dự báo chi tiết.");
            }
        }
        else
        {
            if (UIController.instance != null)
                UIController.instance.ShowMessage("Không có kết nối API để lấy dự báo.");
        }
    }
}