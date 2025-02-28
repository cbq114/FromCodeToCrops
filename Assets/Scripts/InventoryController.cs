using UnityEngine;

public class InventoryController : MonoBehaviour
{

    public SeedDisplay[] seeds;
    public CropDisplay[] crops;

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

                UpdateDisplay();
            }
            else
            {
                gameObject.SetActive(false);
                            if (UIController.instance.staminaBarContainer != null)
            {
                UIController.instance.staminaBarContainer.SetActive(true);
            }
            }
        }
    }


    public void UpdateDisplay()
    {
        foreach(SeedDisplay seed in seeds)
        {
            seed.UpdateDisplay();
        }

        foreach(CropDisplay crop in crops)
        {
            crop.UpdateDisplay();
        }
    }
}
