using UnityEngine;
using UnityEngine.UI;

public class FishingUIController : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;
    public GameObject fishingUIPanel;
    public FishingManager fishingManager;

    void Start()
    {
        yesButton.onClick.AddListener(YesFishing);
        noButton.onClick.AddListener(NoFishing);
    }

    void YesFishing()
    {
        fishingUIPanel.SetActive(false);
        if (fishingManager != null)
        {
            fishingManager.CatchFish();
        }
        else
        {
            Debug.LogError("FishingManager chưa được gán vào FishingUIController!");
        }
    }

    void NoFishing()
    {
        fishingUIPanel.SetActive(false);
        Debug.Log("Fishing canceled.");
    }
}
