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
        AudioManager.instance.StopAllMusic(); // Dừng tất cả nhạc trước
        AudioManager.instance.PlayTitle(); // Phát nhạc menu chính
    }

    public void PlayGame()
    {
        AudioManager.instance.StopAllMusic(); // Dừng nhạc menu
        AudioManager.instance.PlayNextBGM(); // Phát nhạc gameplay
        AudioManager.instance.PlaySFXPitchAdjusted(5);
        SceneManager.LoadScene(levelToStart);
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
            // Chuyển scene trước khi kiểm tra các instance
            SceneManager.LoadScene("Main");
            
            // Lưu ý: Sau khi chuyển scene, các instance có thể chưa được khởi tạo ngay lập tức
            // Nên không nên gọi các instance ở đây, vì scene mới đang được tải
            
            // SaveManager sẽ tự động load trong Start() của nó sau khi scene mới được tải
        }
        else
        {
            // Hiển thị thông báo không có save
            Debug.Log("Không có file lưu game!");
            
            // Nếu muốn hiển thị thông báo cho người dùng, kiểm tra UIController trước
            if (UIController.instance != null)
            {
                UIController.instance.ShowMessage("Không tìm thấy file lưu game!");
            }
        }
    }
}
