using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopFishDisplay : MonoBehaviour
{
    public FishController.FishType fish;

    public Image fishImage;
    public TMP_Text amountText, priceText;

    // Hàm cập nhật UI
    public void UpdateDisplay()
    {
        FishInfo info = FishController.instance.GetFishInfo(fish);

        if (info != null)
        {
            fishImage.sprite = info.fishSprite;
            amountText.text = "x" + info.fishAmount;
            priceText.text = "$" + info.fishPrice + " each";
        }
    }

    // Bán 1 con cá
    public void SellOneFish()
    {
        SellFish(1);
    }

    // Bán 10 con cá
    public void SellTenFish()
    {
        SellFish(10);
    }

    // Hàm dùng chung để bán số lượng tùy ý
    private void SellFish(int amount)
    {
        FishInfo info = FishController.instance.GetFishInfo(fish);

        if (info != null && info.fishAmount > 0)
        {
            // Số cá thực sự có thể bán (tránh vượt quá số cá đang có)
            int fishToSell = Mathf.Min(info.fishAmount, amount);

            // Cộng tiền
            CurrencyController.instance.AddMoney(fishToSell * info.fishPrice);

            // Trừ số cá trong FishController
            FishController.instance.RemoveFish(fish, fishToSell);

            // Cập nhật UI
            UpdateDisplay();

            // Phát âm thanh
            AudioManager.instance.PlaySFXPitchAdjusted(5);
        }
    }
}
