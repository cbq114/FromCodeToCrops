using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject petMenuPanel;
    public Button petButton;
    public Button feedButton;
    public Button renameButton;
    public Button closeButton;
    public TMP_Text petNameText;
    public TMP_Text affectionLevelText;
    public Image affectionImage;

    [Header("Rename Menu")]
    public GameObject renamePanel;
    public TMP_InputField nameInputField;
    public Button confirmNameButton;
    public Button cancelNameButton;

    [Header("Item")]
    public GameObject petFoodItem; // Hiển thị số lượng thức ăn thú cưng
    public TMP_Text petFoodCountText;

    private int petFoodCount = 0;


    void Start()
    {
        // Đóng menu ban đầu
        if (petMenuPanel != null)
            petMenuPanel.SetActive(false);

        if (renamePanel != null)
            renamePanel.SetActive(false);

        // Gán sự kiện cho các nút
        petButton.onClick.AddListener(OnPetButtonClick);
        feedButton.onClick.AddListener(OnFeedButtonClick);
        renameButton.onClick.AddListener(OnRenameButtonClick);
        closeButton.onClick.AddListener(ClosePetMenu);

        confirmNameButton.onClick.AddListener(ConfirmNewName);
        cancelNameButton.onClick.AddListener(() => renamePanel.SetActive(false));

        // Tải lượng thức ăn thú cưng
        LoadPetFoodCount();
        UpdateFoodCountUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Phím P để mở menu thú cưng
        {
            TogglePetMenu();
        }
    }

    public void TogglePetMenu()
    {
        if (petMenuPanel != null)
        {
            bool newState = !petMenuPanel.activeSelf;
            petMenuPanel.SetActive(newState);

            // Thêm code ẩn/hiện UI
            if (newState) // Khi mở menu pet
            {
                // Ẩn thanh stamina và bảng mùa
                if (UIController.instance.staminaBarContainer != null)
                {
                    UIController.instance.staminaBarContainer.SetActive(false);
                }
                if (UIController.instance.seasonPanel != null)
                {
                    UIController.instance.seasonPanel.SetActive(false);
                }

                UpdatePetMenuInfo();
            }
            else // Khi đóng menu pet
            {
                // Hiện lại thanh stamina và bảng mùa
                if (UIController.instance.staminaBarContainer != null)
                {
                    UIController.instance.staminaBarContainer.SetActive(true);
                }
                if (UIController.instance.seasonPanel != null)
                {
                    UIController.instance.seasonPanel.SetActive(true);
                }
            }
        }
    }
    private void UpdatePetMenuInfo()
    {
        if (PetSystem.instance != null)
        {
            petNameText.text = PetSystem.instance.petName;
            affectionLevelText.text = $"Độ thân thiết: {PetSystem.instance.affectionLevel}/{PetSystem.instance.maxAffectionLevel}";

            // Thêm kiểm tra mảng affectionIcons trước khi truy cập
            if (PetSystem.instance.affectionIcons != null && PetSystem.instance.affectionIcons.Length > 0)
            {
                int iconIndex = Mathf.Clamp(PetSystem.instance.affectionLevel, 0, PetSystem.instance.affectionIcons.Length - 1);
                affectionImage.sprite = PetSystem.instance.affectionIcons[iconIndex];
            }
            else
            {
                Debug.LogWarning("Mảng affectionIcons chưa được gán sprites trong PetSystem!");
                // Có thể gán một sprite mặc định ở đây nếu có
                // affectionImage.sprite = defaultSprite;
            }

            // Cập nhật trạng thái nút thức ăn
            feedButton.interactable = petFoodCount > 0;
        }
    }

    private void OnPetButtonClick()
    {
        if (PetSystem.instance != null)
        {
            PetSystem.instance.PetInteraction();
            UpdatePetMenuInfo();
        }
    }

    private void OnFeedButtonClick()
    {
        if (petFoodCount > 0 && PetSystem.instance != null)
        {
            PetSystem.instance.FeedPet();
            petFoodCount--;
            SavePetFoodCount();
            UpdateFoodCountUI();
            UpdatePetMenuInfo();
        }
    }

    private void OnRenameButtonClick()
    {
        if (renamePanel != null)
        {
            renamePanel.SetActive(true);
            nameInputField.text = PetSystem.instance.petName;
        }
    }

    private void ConfirmNewName()
    {
        if (nameInputField.text.Length > 0)
        {
            PetSystem.instance.petName = nameInputField.text;
            renamePanel.SetActive(false);
            UpdatePetMenuInfo();

            // Hiển thị thông báo
            UIController.instance.ShowMessage($"Đã đổi tên thú cưng thành: {nameInputField.text}!");
        }
    }

    private void ClosePetMenu()
    {
        petMenuPanel.SetActive(false);

        // Hiện lại UI
        if (UIController.instance != null)
        {
            if (UIController.instance.staminaBarContainer != null)
            {
                UIController.instance.staminaBarContainer.SetActive(true);
            }
            if (UIController.instance.seasonPanel != null)
            {
                UIController.instance.seasonPanel.SetActive(true);
            }
        }
    }

    public void AddPetFood(int amount)
    {
        petFoodCount += amount;
        SavePetFoodCount();
        UpdateFoodCountUI();

        UIController.instance.ShowMessage($"Nhận được {amount} thức ăn cho thú cưng!");
    }

    private void UpdateFoodCountUI()
    {
        if (petFoodCountText != null)
            petFoodCountText.text = petFoodCount.ToString();

        if (feedButton != null)
            feedButton.interactable = petFoodCount > 0;
    }

    private void SavePetFoodCount()
    {
        PlayerPrefs.SetInt("PetFoodCount", petFoodCount);
        PlayerPrefs.Save();
    }

    private void LoadPetFoodCount()
    {
        if (PlayerPrefs.HasKey("PetFoodCount"))
            petFoodCount = PlayerPrefs.GetInt("PetFoodCount");
        else
            petFoodCount = 0;
    }

    // Thêm phương thức public này để SaveManager có thể gọi
    public void ReloadPetFoodData()
    {
        if (PlayerPrefs.HasKey("PetFoodCount"))
            petFoodCount = PlayerPrefs.GetInt("PetFoodCount");
        else
            petFoodCount = 0;

        if (petFoodCountText != null)
            petFoodCountText.text = petFoodCount.ToString();

        if (feedButton != null)
            feedButton.interactable = petFoodCount > 0;
    }
}