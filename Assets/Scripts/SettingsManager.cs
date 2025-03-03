using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject settingsPanel;

    private void Start()
    {

        // Load giá trị âm lượng đã lưu
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Gán sự kiện khi thay đổi giá trị Slider
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    public void BackToPause()
    {
        settingsPanel.SetActive(false); // Tắt Settings
        //pauseMenuPanel.SetActive(true); // Hiện màn hình Pause
    }
}
