using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string levelToStart;
    public string guideToStart;
    public string mainMenu;

    private void Start()
    {
        AudioManager.instance.PlayTitle();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(levelToStart);

        AudioManager.instance.PlayNextBGM();

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void GuideGame()
    {
        SceneManager.LoadScene(guideToStart);

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenu);

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quitting The Game");

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }
}
