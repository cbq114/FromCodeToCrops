using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System;

public class CookingSystem : MonoBehaviour
{
	public static CookingSystem instance;

	[Header("UI")]
	public GameObject cookingPanel;
	public RectTransform recipeContainer;
	public GameObject recipeButtonPrefab;

	[Header("Recipe Details")]
	public Image selectedRecipeImage;
	public TMP_Text selectedRecipeName;
	public TMP_Text selectedRecipeDescription;
	public TMP_Text ingredientsText;
	public TMP_Text itemAmountText;
	public Button cookButton;
	public Button useButton;
	public Button suggestionButton;

	[Header("Recipes")]
	public List<Recipe> availableRecipes = new List<Recipe>();

	private Recipe currentRecipe;

	// Class to deserialize JSON recipe suggestions
	[System.Serializable]
	public class RecipeSuggestion
	{
		public string name;
		public string description;
	}

	// Wrapper class to handle JSON array parsing
	[System.Serializable]
	public class SuggestionWrapper
	{
		public RecipeSuggestion[] suggestions;
	}

	private void Awake()
	{
		Debug.Log("🔄 CookingSystem Awake() chạy!");
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OpenCookingPanel()
	{
		cookingPanel.SetActive(true);
		UIController.instance.staminaBarContainer.SetActive(false);
		PopulateRecipes();

		if (suggestionButton != null && suggestionButton.onClick.GetPersistentEventCount() == 0)
			suggestionButton.onClick.AddListener(ShowRecipeSuggestions);
	}

	public void CloseCookingPanel()
	{
		cookingPanel.SetActive(false);
		UIController.instance.staminaBarContainer.SetActive(true);
	}

	private void PopulateRecipes()
	{
		foreach (Transform child in recipeContainer)
		{
			Destroy(child.gameObject);
		}

		foreach (Recipe recipe in availableRecipes)
		{
			GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeContainer);
			Button button = buttonObj.GetComponent<Button>();

			buttonObj.transform.Find("RecipeIcon").GetComponent<Image>().sprite = recipe.recipeImage;
			buttonObj.transform.Find("RecipeName").GetComponent<TMP_Text>().text = recipe.recipeName;

			button.onClick.AddListener(() => SelectRecipe(recipe));
		}
	}

	private void SelectRecipe(Recipe recipe)
	{
		currentRecipe = recipe;

		selectedRecipeImage.sprite = recipe.recipeImage;
		selectedRecipeName.text = recipe.recipeName;
		selectedRecipeDescription.text = recipe.description;

		int itemAmount = CookingInventory.instance.GetItemAmount(recipe.resultItem);
		itemAmountText.text = $"Số lượng: {itemAmount}";

		string ingredientsList = "Nguyên liệu cần:\n";
		bool canCook = true;

		foreach (RecipeIngredient ingredient in recipe.ingredients)
		{
			int playerHas = CropController.instance.GetCropInfo(ingredient.cropType).cropAmount;
			ingredientsList += $"- {ingredient.cropType}: {playerHas}/{ingredient.amount}\n";

			if (playerHas < ingredient.amount)
				canCook = false;
		}

		ingredientsText.text = ingredientsList;
		cookButton.interactable = canCook;

		useButton.interactable = itemAmount > 0;
		useButton.onClick.RemoveAllListeners();
		useButton.onClick.AddListener(() => ConsumeFood(currentRecipe));
	}

	public void CookSelectedRecipe()
	{
		if (currentRecipe == null) return;

		foreach (RecipeIngredient ingredient in currentRecipe.ingredients)
		{
			CropController.instance.UseCrop(ingredient.cropType, ingredient.amount);
		}

		CookingInventory.instance.AddItem(currentRecipe.resultItem, currentRecipe.resultAmount);
		UIController.instance.ShowMessage($"Đã nấu thành công: {currentRecipe.recipeName}");
		AudioManager.instance.PlaySFX(8);
		SelectRecipe(currentRecipe);
	}

	public void ConsumeFood(Recipe recipe)
	{
		if (CookingInventory.instance.GetItemAmount(recipe.resultItem) <= 0)
		{
			UIController.instance.ShowMessage("Bạn không có món ăn này!");
			return;
		}

		CookingInventory.instance.RemoveItem(recipe.resultItem, 1);
		PlayerController.instance.currentStamina += recipe.staminaRestoreAmount;
		if (PlayerController.instance.currentStamina > PlayerController.instance.maxStamina)
			PlayerController.instance.currentStamina = PlayerController.instance.maxStamina;

		PlayerController.instance.UpdateStaminaUI();

		if (recipe.speedBoostAmount > 0)
			StartCoroutine(ApplySpeedBoost(recipe.speedBoostAmount, recipe.speedBoostDuration));

		if (recipe.toolEfficiencyBoost > 0)
			StartCoroutine(ApplyToolEfficiencyBoost(recipe.toolEfficiencyBoost, recipe.toolEfficiencyDuration));

		SelectRecipe(recipe);
		UIController.instance.ShowMessage($"Đã sử dụng: {recipe.recipeName}");
	}

	private IEnumerator ApplySpeedBoost(float amount, float duration)
	{
		float originalSpeed = PlayerController.instance.moveSpeed;
		PlayerController.instance.moveSpeed += amount;
		yield return new WaitForSeconds(duration);
		PlayerController.instance.moveSpeed = originalSpeed;
	}

	private IEnumerator ApplyToolEfficiencyBoost(float amount, float duration)
	{
		float originalStaminaUse = PlayerController.instance.staminaUsePerAction;
		PlayerController.instance.staminaUsePerAction -= amount;
		yield return new WaitForSeconds(duration);
		PlayerController.instance.staminaUsePerAction = originalStaminaUse;
	}

	public async Task<List<string>> GetRecipeSuggestions()
	{
		StringBuilder ingredients = new StringBuilder();

		foreach (CropController.CropType cropType in Enum.GetValues(typeof(CropController.CropType)))
		{
			int amount = CropController.instance.GetCropInfo(cropType).cropAmount;
			if (amount > 0)
			{
				ingredients.Append($"{cropType}: {amount}, ");
			}
		}

		string prompt = $"Given these ingredients: {ingredients}, suggest 3 recipe names " +
					   $"and their simple descriptions that would be suitable for a farming game. " +
					   $"Format as JSON array with 'name' and 'description' fields.";

		string response = await GeminiAPIClient.instance.SendRequest(prompt);
		return ParseRecipeJson(response);
	}

	private List<string> ParseRecipeJson(string json)
	{
		List<string> suggestions = new List<string>();
		try
		{
			string jsonArray = json.Trim();
			if (jsonArray.StartsWith("[") && jsonArray.EndsWith("]"))
			{
				RecipeSuggestion[] recipes = JsonUtility.FromJson<RecipeSuggestion[]>(jsonArray);
				foreach (var recipe in recipes)
				{
					suggestions.Add($"{recipe.name}: {recipe.description}");
				}
			}
			else
			{
				Debug.LogError("Invalid JSON format: not an array.");
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"Error parsing JSON: {e.Message}");
		}
		return suggestions;
	}

	public void ShowRecipeSuggestions()
    {
        if (UIController.instance != null)
            UIController.instance.ShowMessage("Đang tìm gợi ý công thức...");
        StartCoroutine(GetRecipeSuggestionsCoroutine());
    }

    private IEnumerator GetRecipeSuggestionsCoroutine()
    {
        if (GeminiAPIClient.instance != null)
        {
            StringBuilder ingredients = new StringBuilder();

            if (InventoryController.instance != null)
            {
                foreach (var item in InventoryController.instance.theItems)
                {
                    if (item != null && item.itemType == InventoryItem.ItemType.Crop)
                    {
                        ingredients.Append($"{item.itemName}: {item.numberHeld}, ");
                    }
                }
            }

            string prompt = $"Tôi có các nguyên liệu sau: {ingredients}. Hãy gợi ý 3 công thức nấu ăn phù hợp cho một game nông trại, mỗi công thức gồm tên và mô tả ngắn gọn.";

            var task = GeminiAPIClient.instance.SendRequest(prompt);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsCompletedSuccessfully)
            {
                string suggestions = task.Result;
                if (UIController.instance != null)
                    UIController.instance.ShowMessage($"Gợi ý công thức:\n\n{suggestions}");
            }
            else
            {
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("Không thể tạo gợi ý công thức.");
            }
        }
        else
        {
            if (UIController.instance != null)
                UIController.instance.ShowMessage("Không có kết nối API để tạo gợi ý công thức.");
        }
    }
}