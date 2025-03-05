using UnityEngine;

public class InventoryItem
{
	public enum ItemType
	{
		Crop,
		Seed,
		Tool // Thêm các loại khác nếu cần
	}

	public ItemType itemType;
	public string itemName;
	public int numberHeld;
}