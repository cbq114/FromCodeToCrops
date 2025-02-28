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

	public bool isStorming = false;
	public bool isSnowing = false;
	public GameObject stormEffect; // Kéo game object hiệu ứng bão vào đây trong Inspector
	public GameObject snowEffect; // Kéo game object hiệu ứng tuyết vào đây trong Inspector

	private void Awake()
	{
		instance = this;
		rainEffect.SetActive(false);
	}

	// Gọi hàm này khi bắt đầu ngày mới
	public void CheckWeatherForNewDay()
	{
		float baseRainChance = rainChance;

		// Điều chỉnh xác suất mưa theo mùa
		if (SeasonSystem.instance != null)
		{
			switch (SeasonSystem.instance.currentSeason)
			{
				case SeasonSystem.Season.Spring:
					baseRainChance *= 1.2f; // Mùa xuân mưa nhiều hơn
					break;

				case SeasonSystem.Season.Summer:
					// Mùa hè có thể có bão
					if (Random.value < 0.05f) // 5% cơ hội có bão
					{
						StartStorm();
						return;
					}
					break;

				case SeasonSystem.Season.Fall:
					baseRainChance *= 1.1f;
					break;

				case SeasonSystem.Season.Winter:
					baseRainChance *= 0.5f; // Mùa đông ít mưa hơn
					if (Random.value < baseRainChance)
					{
						StartSnow(); // Tuyết thay vì mưa
						return;
					}
					break;
			}
		}

		if (Random.value < baseRainChance)
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
	private void StartStorm()
	{
		isRaining = true;
		isStorming = true;
		rainEffect.SetActive(true);
		stormEffect.SetActive(true);

		// Hiệu ứng âm thanh sấm sét
		if (AudioManager.instance != null)
		{
			AudioManager.instance.PlaySFX(9); // Giả sử số 9 là âm thanh sấm sét
		}

		float stormEndTime = TimeController.instance.currentTime + rainDuration * 0.7f;
		if (stormEndTime > TimeController.instance.dayEnd)
			stormEndTime = TimeController.instance.dayEnd;

		StartCoroutine(StopStormAt(stormEndTime));
	}
	private void StartSnow()
	{
		isRaining = false; // Không mưa nước mà mưa tuyết
		isSnowing = true;
		snowEffect.SetActive(true);

		// Phát âm thanh tuyết rơi nếu cần
		if (AudioManager.instance != null)
		{
			AudioManager.instance.PlaySFX(10); // Giả sử âm thanh tuyết rơi là số 10
		}

		float snowEndTime = TimeController.instance.currentTime + rainDuration;
		if (snowEndTime > TimeController.instance.dayEnd)
			snowEndTime = TimeController.instance.dayEnd;

		StartCoroutine(StopSnowAt(snowEndTime));
	}

	private IEnumerator StopStormAt(float endTime)
	{
		// Đợi cho đến khi đến thời gian kết thúc
		while (TimeController.instance.currentTime < endTime && isStorming)
		{
			yield return null;
		}
		isRaining = false;
		isStorming = false;
		rainEffect.SetActive(false);
		stormEffect.SetActive(false);
	}

	private IEnumerator StopSnowAt(float endTime)
	{
		// Đợi cho đến khi đến thời gian kết thúc
		while (TimeController.instance.currentTime < endTime && isSnowing)
		{
			yield return null;
		}
		isSnowing = false;
		snowEffect.SetActive(false);
	}
}