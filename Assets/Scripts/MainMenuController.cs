using System.IO;
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

    public void NewGame()
{
    // Xóa file save nếu cần
    string path = Application.persistentDataPath + "/savegame.json";
    if (File.Exists(path))
        File.Delete(path);
        
    SceneManager.LoadScene("Main");
}

public void ContinueGame()
{
    string path = Application.persistentDataPath + "/savegame.json";
    if (File.Exists(path))
    {
        SceneManager.LoadScene("Main");
        // SaveManager sẽ tự động load trong Start()
    }
    else
    {
        // Hiển thị thông báo không có save
        Debug.Log("Không có file lưu game!");
    }
}
}
