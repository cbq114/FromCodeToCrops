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
    }
}
