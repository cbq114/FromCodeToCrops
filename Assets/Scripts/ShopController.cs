using UnityEngine;

public class ShopController : MonoBehaviour
{
    public ShopSeedDisplay[] seeds;
    public ShopCropDisplay[] crops;

    public void OpenClose()
    {
        if (UIController.instance.theIC.gameObject.activeSelf == false)
        {
            // Lưu trạng thái hiện tại để biết chúng ta đang mở hay đóng shop
            bool isOpening = !gameObject.activeSelf;
            
            gameObject.SetActive(isOpening);

            if (isOpening)
            {
                // Đang mở shop, ẩn thanh stamina
                if (UIController.instance.staminaBarContainer != null)
                {
                    UIController.instance.staminaBarContainer.SetActive(false);
                }
                
                // Cập nhật hiển thị các mặt hàng
                foreach(ShopSeedDisplay seed in seeds) 
                {
                    seed.UpdateDisplay();
                }

                foreach(ShopCropDisplay crop in crops)
                {
                    crop.UpdateDisplay();
                }
            }
            else
            {
                // Đang đóng shop, hiện lại thanh stamina
                if (UIController.instance.staminaBarContainer != null)
                {
                    UIController.instance.staminaBarContainer.SetActive(true);
                }
            }
        }
    }
}