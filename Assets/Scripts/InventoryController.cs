using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance; // Add singleton pattern for consistency
    public List<InventoryItem> theItems = new List<InventoryItem>();
    public SeedDisplay[] seeds;
    public CropDisplay[] crops;
    public FishDisplay[] fishs;

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

    public void OpenClose()
    {
        if (UIController.instance.theShop.gameObject.activeSelf == false)
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
                if (UIController.instance.staminaBarContainer != null)
                {
                    UIController.instance.staminaBarContainer.SetActive(false);
                }
                if (UIController.instance.seasonPanel != null)
                {
                    UIController.instance.seasonPanel.SetActive(false);
                }
                UpdateDisplay();
            }
            else
            {
                gameObject.SetActive(false);
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

    public void UpdateDisplay()
    {
        foreach (SeedDisplay seed in seeds)
        {
            seed.UpdateDisplay();
        }

        foreach (CropDisplay crop in crops)
        {
            crop.UpdateDisplay();
        }

        foreach (FishDisplay fish in fishs)
        {
            fish.UpdateDisplay();
        }
    }
}