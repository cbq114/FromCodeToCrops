using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
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

    [Header("Interaction UI")]
    public GameObject interactionHint;
    public TMP_Text interactionHintText;

    public GameObject[] toolbarActivatorIcons;

    public TMP_Text timeText;

    public InventoryController theIC;
    public ShopController theShop;

    public Image seedImage;

    public TMP_Text moneyText;

    public GameObject pauseScreen;
    public string mainMenuScene;

    public Slider staminaBar;

    public GameObject sleepPrompt;

    public GameObject seasonPanel;
    public Image seasonIcon;
    public TMP_Text seasonName;
    //season progress bar
    public Slider seasonProgressBar;

    public GameObject messagePanel;
    public TMP_Text messageText;
    public float messageDuration = 3f;
    private Coroutine messageCoroutine;



    public GameObject staminaBarContainer;
    public Image staminaFillImage; // Drag the Fill image here in inspector
    public Color normalStaminaColor = Color.green;
    public Color lowStaminaColor = Color.red;
    public float lowStaminaThreshold = 0.3f; // 30% threshold


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize UI elements
        if (staminaBar != null)
        {
            staminaBar.value = staminaBar.maxValue;

            // Set initial color
            if (staminaFillImage != null)
            {
                staminaFillImage.color = normalStaminaColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            theIC.OpenClose();
        }

#if UNITY_EDITOR

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            theShop.OpenClose();
        }

#endif

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            PauseUnpause();
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (CookingSystem.instance != null)
            {
                if (!CookingSystem.instance.cookingPanel.activeSelf)
                    CookingSystem.instance.OpenCookingPanel();
                else
                    CookingSystem.instance.CloseCookingPanel();
            }
        }
    }

    public void SwitchTool(int selected)
    {
        if (toolbarActivatorIcons == null || selected < 0 || selected >= toolbarActivatorIcons.Length)
        {
            Debug.LogWarning("SwitchTool: Chỉ số không hợp lệ hoặc mảng chưa được khởi tạo!");
            return;
        }

        foreach (GameObject icon in toolbarActivatorIcons)
        {
            if (icon != null)
            {
                icon.SetActive(false);
            }
        }

        if (toolbarActivatorIcons[selected] != null)
        {
            toolbarActivatorIcons[selected].SetActive(true);
        }
    }

    public void UpdateTimeText(float currentTime)
    {
        if (currentTime < 12)
        {
            timeText.text = Mathf.FloorToInt(currentTime) + "AM";
        }
        else if (currentTime < 13)
        {
            timeText.text = "12PM";
        }
        else if (currentTime < 24)
        {
            timeText.text = Mathf.FloorToInt(currentTime - 12) + "PM";
        }
        else if (currentTime < 25)
        {
            timeText.text = "12AM";
        }
        else
        {
            timeText.text = Mathf.FloorToInt(currentTime - 24) + "AM";
        }
    }

    public void SwitchSeed(CropController.CropType crop)
    {
        seedImage.sprite = CropController.instance.GetCropInfo(crop).seedType;

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void UpdateMoneyText(float currentMoney)
    {
        moneyText.text = "$" + currentMoney;
    }

    public void UpdateStaminaBar(float current, float max)
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = max;
            staminaBar.value = current;

            // Change color based on stamina level
            if (staminaFillImage != null)
            {
                if (current / max <= lowStaminaThreshold)
                    staminaFillImage.color = lowStaminaColor;
                else
                    staminaFillImage.color = normalStaminaColor;
            }
        }
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            if (staminaBarContainer != null)
            {
                staminaBarContainer.SetActive(false);
            }

            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            if (staminaBarContainer != null)
            {
                staminaBarContainer.SetActive(true);
            }

            Time.timeScale = 1f;
        }

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);

        Destroy(gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GridInfo.instance.gameObject);
        Destroy(TimeController.instance.gameObject);
        Destroy(CropController.instance.gameObject);
        Destroy(CurrencyController.instance.gameObject);

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void QuitGame()
    {
        Application.Quit();

        AudioManager.instance.PlaySFXPitchAdjusted(5);
    }

    public void ShowSleepPrompt(bool show)
    {
        if (sleepPrompt != null)
        {
            sleepPrompt.SetActive(show);
        }
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            // Hủy coroutine đang hiển thị thông báo cũ nếu có
            if (messageCoroutine != null)
                StopCoroutine(messageCoroutine);

            // Hiển thị thông báo mới
            messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
        }
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);

        // Tính toán thời gian hiển thị dựa vào độ dài tin nhắn
        float displayTime = Mathf.Max(messageDuration, 3.0f + (message.Length * 0.05f));

        // Giới hạn thời gian tối đa 15 giây để không quá lâu
        displayTime = Mathf.Min(displayTime, 15f);

        yield return new WaitForSeconds(displayTime);

        messagePanel.SetActive(false);
        messageCoroutine = null;
    }
    public void SaveGameButton()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
        }
        else
        {
            Debug.LogError("SaveManager.instance is null! Make sure there's a SaveManager in the scene.");
            ShowMessage("Không thể lưu game: SaveManager không tồn tại!");
        }
    }

    public void LoadGameButton()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.LoadGame();
        }
        else
        {
            Debug.LogError("SaveManager.instance is null! Make sure there's a SaveManager in the scene.");
            ShowMessage("Không thể tải game: SaveManager không tồn tại!");
        }
    }


}
