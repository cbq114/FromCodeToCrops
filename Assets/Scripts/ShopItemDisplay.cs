using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopItemDisplay : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;
    public string itemDescription;
    public int itemPrice;
    public Sprite itemIcon;
    
    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public Image iconImage;
    public Button buyButton;
    
    [Header("Events")]
    public UnityEvent onBuyItem;
    
    // Start is called before the first execution of Update
    void Start()
    {
        UpdateDisplay();
        
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyItem);
        }
    }
    
    public void UpdateDisplay()
    {
        if (nameText != null)
            nameText.text = itemName;
            
        if (descriptionText != null)
            descriptionText.text = itemDescription;
            
        if (priceText != null)
            priceText.text = itemPrice.ToString() + " xu";
            
        if (iconImage != null && itemIcon != null)
            iconImage.sprite = itemIcon;
    }
    
    public void BuyItem()
    {
        // Nút mua được nhấn
        if (MoneyManager.instance.HasEnoughMoney(itemPrice))
        {
            // Gọi event khi mua
            onBuyItem.Invoke();
            
            // Phát âm thanh mua hàng
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(4); // Giả sử 4 là âm thanh mua hàng
            }
        }
        else
        {
            // Không đủ tiền
            if (UIController.instance != null)
            {
                UIController.instance.ShowMessage("Not enough money!");
            }
            
            // Phát âm thanh lỗi
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(7); // Giả sử 7 là âm thanh lỗi
            }
        }
    }
}