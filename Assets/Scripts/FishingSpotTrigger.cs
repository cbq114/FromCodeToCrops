using UnityEngine;
public class FishingSpotTrigger : MonoBehaviour
{
    public GameObject FishingUIPanel;
    public FishingManager fishingManager;
    private PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.EnterFishingArea();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController != null)
            {
                playerController.ExitFishingArea();
                playerController = null;
            }

            FishingUIPanel.SetActive(false);
            fishingManager.CancelFishing();
        }
    }

    void Update()
    {
        if (playerController != null && Input.GetKeyDown(KeyCode.Alpha5))
        {
            FishingUIPanel.SetActive(true);
        }
    }

    public void FishYes()
    {
        FishingUIPanel.SetActive(false);
        if (playerController != null)
        {
            playerController.StartFishing();
        }
    }

    public void FishNo()
    {
        FishingUIPanel.SetActive(false);
    }
}