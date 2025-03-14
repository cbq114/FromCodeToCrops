using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishDisplay : MonoBehaviour
{
    public FishController.FishType fish;

    public Image fishImage;
    public TMP_Text amountText;

    public void UpdateDisplay()
    {

        FishInfo info = FishController.instance.GetFishInfo(fish);
        fishImage.sprite = info.fishSprite;
        amountText.text = "x" + info.fishAmount;
    }
}
