using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    
    [Header("Night Overlay")]
    public Image nightOverlay;
    public float maxDarkness = 0.7f; // Maximum alpha value for the overlay
    public float fadeSpeed = 2f; // How fast to fade in/out
    
    private Color nightColor;
    
    private void Awake()
    {
        instance = this;
        
        // Initialize night color
        nightColor = new Color(0, 0, 0, 0);
        if (nightOverlay != null)
        {
            nightOverlay.color = nightColor;
        }
    }
    
    private void Update()
    {
        // Only update when TimeController exists
        if (TimeController.instance != null)
        {
            UpdateNightOverlay();
        }
    }
    
private void UpdateNightOverlay()
{
    float targetAlpha = 0f;
    float currentTime = TimeController.instance.currentTime;
    
    // Define transition periods
    float sunsetStart = 17f;   // 5 PM - sunset begins
    float nightStart = 20f;    // 8 PM - fully night
    float sunriseStart = 4f;   // 4 AM - sunrise begins
    float dayStart = 7f;       // 7 AM - fully day

    // Calculate target alpha based on time
    if (currentTime >= nightStart || currentTime < sunriseStart)
    {
        // Full night darkness
        targetAlpha = maxDarkness;
    }
    else if (currentTime >= sunsetStart && currentTime < nightStart)
    {
        // Sunset transition (gradually darkening)
        float transitionProgress = (currentTime - sunsetStart) / (nightStart - sunsetStart);
        targetAlpha = maxDarkness * transitionProgress;
    }
    else if (currentTime >= sunriseStart && currentTime < dayStart)
    {
        // Sunrise transition (gradually brightening)
        float transitionProgress = 1f - ((currentTime - sunriseStart) / (dayStart - sunriseStart));
        targetAlpha = maxDarkness * transitionProgress;
    }
    
    // Fade in/out the overlay
    nightColor.a = Mathf.Lerp(nightColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
    nightOverlay.color = nightColor;
}
}