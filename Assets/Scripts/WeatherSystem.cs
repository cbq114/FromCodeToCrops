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

	private bool isRaining = false;
	private bool isStorming = false;
	private bool isSnowing = false;
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
		WeatherAI weatherAI = GetComponent<WeatherAI>();

		if (weatherAI == null)
			return "Không có dự báo thời tiết.";

		// Trả về thông tin sơ bộ ngay lập tức, không cần đợi API
		string basicForecast = $"Thời tiết hôm nay: {GetWeatherName(currentWeatherType)}";

		// Lưu trạng thái để có thể hiển thị chi tiết sau
		hasDetailedForecastPending = true;

		return basicForecast;
	}

	// Thêm phương thức để lấy tên thời tiết tiếng Việt
	private string GetWeatherName(WeatherType type)
	{
		switch (type)
		{
			case WeatherType.Clear:
				return "Quang đãng";
			case WeatherType.Rain:
				return "Mưa";
			case WeatherType.Storm:
				return "Bão";
			case WeatherType.Fog:
				return "Sương mù";
			default:
				return "Không xác định";
		}
	}

	// Coroutine để lấy dự báo chi tiết và hiển thị
	private IEnumerator GetDetailedForecastAsync()
	{
		if (GeminiAPIClient.instance != null)
		{
			// Bắt đầu request API
			var task = GeminiAPIClient.instance.GetWeatherDescription(
				currentWeatherType.ToString(),
				SeasonSystem.instance.currentSeason.ToString(),
				TimeController.instance.currentDay);

			// Đợi 3 giây để đảm bảo người chơi đã đọc dự báo cơ bản
			yield return new WaitForSeconds(3.0f);

			// Đợi API trả về kết quả
			while (!task.IsCompleted)
				yield return null;

			// Kiểm tra kỹ hơn trước khi sử dụng kết quả
			if (!task.IsFaulted && task.IsCompletedSuccessfully && UIController.instance != null)
			{
				string detailedForecast = task.Result;
				Debug.Log($"Detailed forecast received: {detailedForecast}");
				UIController.instance.ShowMessage(detailedForecast);
			}
			else if (task.IsFaulted)
			{
				Debug.LogError("Weather API task failed: " + (task.Exception != null ? task.Exception.Message : "Unknown error"));
			}
		}
	}
	public void ShowDetailedForecast()
	{
		if (hasDetailedForecastPending)
		{
			StartCoroutine(GetDetailedForecastAsync());
			hasDetailedForecastPending = false;
		}
	}
}