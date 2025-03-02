using UnityEngine;

public class PetInteraction : MonoBehaviour
{
    void OnMouseOver()
    {
        // Kiểm tra click chuột phải
        if (Input.GetMouseButtonDown(1)) // 1 = chuột phải
        {
            // Tìm và mở Pet Menu
            PetMenuController petMenu = FindObjectOfType<PetMenuController>();
            if (petMenu != null)
            {
                petMenu.TogglePetMenu();
                Debug.Log("Đã nhấp chuột phải vào pet!");
            }
        }
    }
}