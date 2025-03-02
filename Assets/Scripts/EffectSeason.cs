using UnityEngine;

public class EffectSeason : MonoBehaviour
{
    public static EffectSeason instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ParticleSystem snowflakeEffect;
    public ParticleSystem leafEffect;
    public ParticleSystem blossomEffect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void AdjustParticleSettings(ParticleSystem ps, float size, float emissionRate)
    {
        if (ps != null)
        {
            var main = ps.main;
            main.startSize = size;

            var emission = ps.emission;
            emission.rateOverTime = emissionRate;
        }
    }
    public void UpdateSeasonEffect(string season)
    {
        snowflakeEffect?.Stop();
        leafEffect?.Stop();
        blossomEffect?.Stop();

        switch (season.ToLower())
        {
            case "winter":
                AdjustParticleSettings(snowflakeEffect, 0.2f, 50f);
                snowflakeEffect?.Play();
                break;
            case "autumn":
                AdjustParticleSettings(leafEffect, 0.3f, 30f);
                leafEffect?.Play();
                break;
            case "spring":
                AdjustParticleSettings(blossomEffect, 0.2f, 40f);
                blossomEffect?.Play();
                break;
        }
    }
}
