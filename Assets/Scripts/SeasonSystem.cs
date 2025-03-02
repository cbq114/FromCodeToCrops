using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeasonSystem : MonoBehaviour
{
	public static SeasonSystem instance;
	public enum Season { Spring, Summer, Fall, Winter }

	[Header("Season Settings")]
	public Season currentSeason = Season.Spring;
	public int daysPerSeason = 11;
	[SerializeField] private int currentSeasonDay = 1;

	[Header("UI Elements")]
	public TextMeshProUGUI seasonNameText;
	public TextMeshProUGUI seasonDayText;
	public Image seasonIcon;
	public Slider seasonProgressBar;

	[Header("Visual Effects")]
	public Color springColor = new Color(0.5f, 0.8f, 0.5f);
	public Color summerColor = new Color(0.8f, 0.8f, 0.4f);
	public Color fallColor = new Color(0.8f, 0.5f, 0.3f);
	public Color winterColor = new Color(0.7f, 0.7f, 0.9f);

	[Header("Season Effects")]
	public GameObject fallLeavesFX;
	public GameObject snowFX;
	public GameObject butterfliesFX;
	public Light directionalLight;

	[Header("Season Sprites")]
	public Sprite springIcon;
	public Sprite summerIcon;
	public Sprite fallIcon;
	public Sprite winterIcon;

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

	void Start()
	{
		UpdateSeasonUI();
		ApplySeasonVisuals();
	}

	// Gọi khi ngày mới bắt đầu từ TimeController
	public void NewDay()
	{
		currentSeasonDay++;
		if (currentSeasonDay > daysPerSeason)
		{
			AdvanceSeason();
		}

		UpdateSeasonUI();
	}

	private void AdvanceSeason()
	{
		currentSeasonDay = 1;
		currentSeason = (Season)(((int)currentSeason + 1) % 4);

		ApplySeasonVisuals();

		// Sửa thành một trong các cách sau:
		// Cách 1: Ghi log để debug
		Debug.Log("Thay đổi mùa! Hiện tại là " + GetSeasonName());

		// HOẶC Cách 2: Hiển thị thông báo trực tiếp (nếu đã có UI)
		if (seasonNameText != null)
		{
			seasonNameText.text = GetSeasonName();
		}
	}

	private void UpdateSeasonUI()
	{
		if (seasonNameText != null)
		{
			seasonNameText.text = GetSeasonName();
		}

		if (seasonDayText != null)
		{
			seasonDayText.text = "Ngày " + currentSeasonDay;
		}

		if (seasonProgressBar != null)
		{
			seasonProgressBar.value = (float)currentSeasonDay / daysPerSeason;
		}

		if (seasonIcon != null)
		{
			seasonIcon.sprite = GetSeasonIcon();
		}
	}

	private string GetSeasonName()
	{
		switch (currentSeason)
		{
			case Season.Spring: return "Spring";
			case Season.Summer: return "Summer";
			case Season.Fall: return "autumn";
			case Season.Winter: return "Winter";
			default: return "Không xác định";
		}
	}

	private Sprite GetSeasonIcon()
	{
		switch (currentSeason)
		{
			case Season.Spring: return springIcon;
			case Season.Summer: return summerIcon;
			case Season.Fall: return fallIcon;
			case Season.Winter: return winterIcon;
			default: return null;
		}
	}

	private void ApplySeasonVisuals()
	{
		// Tắt tất cả hiệu ứng
		if (fallLeavesFX != null) fallLeavesFX.SetActive(false);
		if (snowFX != null) snowFX.SetActive(false);
		if (butterfliesFX != null) butterfliesFX.SetActive(false);

		// Áp dụng hiệu ứng theo mùa
		switch (currentSeason)
		{
			case Season.Spring:
				RenderSettings.ambientLight = springColor;
				if (butterfliesFX != null) butterfliesFX.SetActive(true);
				if (directionalLight != null) directionalLight.intensity = 1.2f;
				break;

			case Season.Summer:
				RenderSettings.ambientLight = summerColor;
				if (directionalLight != null) directionalLight.intensity = 1.4f;
				break;

			case Season.Fall:
				RenderSettings.ambientLight = fallColor;
				if (fallLeavesFX != null) fallLeavesFX.SetActive(true);
				if (directionalLight != null) directionalLight.intensity = 1.0f;
				break;

			case Season.Winter:
				RenderSettings.ambientLight = winterColor;
				if (snowFX != null) snowFX.SetActive(true);
				if (directionalLight != null) directionalLight.intensity = 0.8f;
				break;
		}
	}

	// Ảnh hưởng của mùa lên tốc độ phát triển cây
	public float GetSeasonalGrowthModifier(CropController.CropType cropType)
	{
		// Ví dụ:
		switch (cropType)
		{
			case CropController.CropType.tomato:
				if (currentSeason == Season.Summer) return 1.5f; // Phát triển tốt hơn vào mùa hè
				if (currentSeason == Season.Winter) return 0.5f; // Phát triển chậm hơn vào mùa đông
				break;

			case CropController.CropType.potato:
				if (currentSeason == Season.Summer) return 1.3f;
				if (currentSeason == Season.Winter) return 0.6f;
				break;

			case CropController.CropType.carrot:
				if (currentSeason == Season.Fall) return 1.4f; // Phát triển tốt vào mùa thu
				break;

				// Thêm các loại cây trồng khác
		}

		return 1.0f; // Tốc độ mặc định
	}
	public void ForceChangeSeasonForTesting()
	{
		// Reset ngày trong mùa về 1
		currentSeasonDay = 1;

		// Chuyển sang mùa tiếp theo
		currentSeason = (Season)(((int)currentSeason + 1) % 4);

		// Cập nhật visual và UI
		ApplySeasonVisuals();
		UpdateSeasonUI();

		Debug.Log("TESTING MODE: Đã chuyển mùa thành " + GetSeasonName());
	}
}