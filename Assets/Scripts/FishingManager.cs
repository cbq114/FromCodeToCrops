using System.Collections;
using TMPro;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public GameObject fishingPromptPanel;
    public TMP_Text fishingPromptText;
    public GameObject catchPromptPanel;
    public TMP_Text catchPromptText;
    public GameObject fishCaughtPanel;
    public TMP_Text fishNameText;

    private FishController fishController;

    private Coroutine fishingCoroutine;
    private bool fishBiting;

    public FishController.FishType[] fishTypes =
    {
        FishController.FishType.Salmon,
        FishController.FishType.Tuna,
        FishController.FishType.Catfish
    };

    void Start()
    {
        fishController = FishController.instance;
        fishCaughtPanel.SetActive(false);
    }

    public void StartFishing()
    {
        if (fishingCoroutine == null)
        {
            fishingCoroutine = StartCoroutine(FishingCoroutine());
        }
    }

    private IEnumerator FishingCoroutine()
    {
        fishBiting = false;

        fishingPromptPanel.SetActive(true);
        fishingPromptText.text = "Waiting for the fish to bite...";

        yield return new WaitForSeconds(10f);

        fishBiting = true;
        fishingPromptPanel.SetActive(false);
        catchPromptPanel.SetActive(true);
        catchPromptText.text = "The fish is biting! Do you want to catch it?";
    }

    public void CatchFish()
    {
        if (fishBiting)
        {
            catchPromptPanel.SetActive(false);
            fishingCoroutine = null;
            fishBiting = false;

            bool success = Random.value <= 0.9f;

            if (success)
            {
                FishController.FishType caughtFish = fishTypes[Random.Range(0, fishTypes.Length)];

                Debug.Log("Bạn đã câu được: " + caughtFish);

                // Hiện panel cá câu được lên màn hình
                ShowCaughtFishPanel(caughtFish);

                // Thêm vào inventory
                fishController.AddFish(caughtFish);

                // Cập nhật inventory nếu đang mở
                if (UIController.instance.theIC != null && UIController.instance.theIC.gameObject.activeSelf)
                {
                    UIController.instance.theIC.UpdateDisplay();
                }
                PlayerController.instance.isWaitingForFish = false;


                if (PlayerController.instance.fishingRodVisual != null)
                    PlayerController.instance.fishingRodVisual.SetActive(false);


            }
            else
            {
                Debug.Log("Cá đã chạy mất!");
                UIController.instance.ShowMessage("Cá đã chạy mất!");
            }
        }
    }

    private void ShowCaughtFishPanel(FishController.FishType caughtFish)
    {
        fishCaughtPanel.SetActive(true);

        // Hiển thị tên cá
        fishNameText.text = caughtFish.ToString();

        // Ẩn thông báo sau 3 giây
        StartCoroutine(HideCaughtFishPanel());
    }

    private IEnumerator HideCaughtFishPanel()
    {
        yield return new WaitForSeconds(3f);
        fishCaughtPanel.SetActive(false);
    }

    public void CancelFishing()
    {
        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
            fishBiting = false;
            fishingPromptPanel.SetActive(false);
            catchPromptPanel.SetActive(false);

            UIController.instance.ShowMessage("Bạn đã ngừng câu cá.");
        }
        PlayerController.instance.isWaitingForFish = false;

        // Tắt nhân vật câu cá, bật lại nhân vật thường
        if (PlayerController.instance.fishingRodVisual != null)
            PlayerController.instance.fishingRodVisual.SetActive(false);


    }
}
