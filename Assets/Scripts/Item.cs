using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Farming Game/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string description;
    public int baseValue; // Giá bán
    public bool isConsumable;
    public Recipe recipe; // Công thức nếu là món ăn
}