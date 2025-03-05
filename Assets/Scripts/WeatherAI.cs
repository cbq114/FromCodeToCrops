using UnityEngine;

public class WeatherAI : MonoBehaviour
{
    [System.Serializable]
    public class SeasonWeatherProfile
    {
        public SeasonSystem.Season season;
        [Range(0f, 1f)] public float rainProbability;
        [Range(0f, 1f)] public float stormProbability;
        [Range(0f, 1f)] public float fogProbability;
        [Range(1, 5)] public int maxConsecutiveRainDays;
    }
    
    public SeasonWeatherProfile[] seasonProfiles;
    
    private int consecutiveRainDays = 0;
    private WeatherSystem.WeatherType currentWeather;
    private SeasonWeatherProfile currentProfile;
    
    void Start()
    {
        if (SeasonSystem.instance != null)
        {
            UpdateSeasonProfile(SeasonSystem.instance.currentSeason);
        }
    }
    
    public void UpdateSeasonProfile(SeasonSystem.Season season)
    {
        foreach (var profile in seasonProfiles)
        {
            if (profile.season == season)
            {
                currentProfile = profile;
                break;
            }
        }
    }
    
    public WeatherSystem.WeatherType PredictWeather()
    {
        if (currentProfile == null) return WeatherSystem.WeatherType.Clear;
        
        // Nếu đã mưa nhiều ngày liên tiếp, tăng khả năng trời nắng
        float clearModifier = Mathf.Min(1f, (float)consecutiveRainDays / currentProfile.maxConsecutiveRainDays);
        
        float random = Random.value;
        
        // Tính toán xác suất cho từng loại thời tiết
        if (random < currentProfile.stormProbability * (1 - clearModifier))
        {
            consecutiveRainDays++;
            return WeatherSystem.WeatherType.Storm;
        }
        else if (random < currentProfile.stormProbability + currentProfile.rainProbability * (1 - clearModifier))
        {
            consecutiveRainDays++;
            return WeatherSystem.WeatherType.Rain;
        }
        else if (random < currentProfile.stormProbability + currentProfile.rainProbability + currentProfile.fogProbability)
        {
            consecutiveRainDays = 0;
            return WeatherSystem.WeatherType.Fog;
        }
        else
        {
            consecutiveRainDays = 0;
            return WeatherSystem.WeatherType.Clear;
        }
    }
}