using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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

    [Header("Recipes")]
    public List<Recipe> availableRecipes = new List<Recipe>();
    
    private Recipe currentRecipe;
    
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
    }
    
    public void CloseCookingPanel()
    {
        cookingPanel.SetActive(false);
        UIController.instance.staminaBarContainer.SetActive(true);
    }
    
    private void PopulateRecipes()
    {
        // Xóa các nút công thức cũ
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Tạo nút cho mỗi công thức
        foreach (Recipe recipe in availableRecipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set up button UI
            buttonObj.transform.Find("RecipeIcon").GetComponent<Image>().sprite = recipe.recipeImage;
            buttonObj.transform.Find("RecipeName").GetComponent<TMP_Text>().text = recipe.recipeName;
            
            // Add click listener
            button.onClick.AddListener(() => SelectRecipe(recipe));
        }
    }
    
    private void SelectRecipe(Recipe recipe)
    {
        currentRecipe = recipe;
        
        selectedRecipeImage.sprite = recipe.recipeImage;
        selectedRecipeName.text = recipe.recipeName;
        selectedRecipeDescription.text = recipe.description;

        // Hiển thị số lượng món ăn trong kho
        int itemAmount = CookingInventory.instance.GetItemAmount(recipe.resultItem);
        itemAmountText.text = $"Số lượng: {itemAmount}";


        // Hiển thị các nguyên liệu cần thiết
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

        // Cập nhật nút sử dụng (chỉ có thể sử dụng nếu số lượng > 0)
        useButton.interactable = itemAmount > 0;

        useButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(() => ConsumeFood(currentRecipe));
    }
    
    public void CookSelectedRecipe()
    {
        if (currentRecipe == null) return;
        
        // Trừ nguyên liệu
        foreach (RecipeIngredient ingredient in currentRecipe.ingredients)
        {
            CropController.instance.UseCrop(ingredient.cropType, ingredient.amount);
        }

        // Thêm món ăn vào kho đồ
        CookingInventory.instance.AddItem(currentRecipe.resultItem, currentRecipe.resultAmount);

        // Hiển thị thông báo thành công
        UIController.instance.ShowMessage($"Đã nấu thành công: {currentRecipe.recipeName}");
        
        // Play sound effect
        AudioManager.instance.PlaySFX(8); // Giả sử 8 là âm thanh nấu ăn
        
        // Cập nhật UI
        SelectRecipe(currentRecipe); // Cập nhật lại UI để hiển thị số nguyên liệu mới
    }
    
    public void ConsumeFood(Recipe recipe)
    {
        if (CookingInventory.instance.GetItemAmount(recipe.resultItem) <= 0)
        {
            UIController.instance.ShowMessage("Bạn không có món ăn này!");
            return;
        }

        // Giảm số lượng món ăn trong kho
        CookingInventory.instance.RemoveItem(recipe.resultItem, 1);

        // Hồi phục stamina
        PlayerController.instance.currentStamina += recipe.staminaRestoreAmount;
        if (PlayerController.instance.currentStamina > PlayerController.instance.maxStamina)
            PlayerController.instance.currentStamina = PlayerController.instance.maxStamina;
            
        PlayerController.instance.UpdateStaminaUI();
        
        // Áp dụng hiệu ứng tăng tốc nếu có
        if (recipe.speedBoostAmount > 0)
            StartCoroutine(ApplySpeedBoost(recipe.speedBoostAmount, recipe.speedBoostDuration));
            
        // Áp dụng hiệu ứng tăng hiệu quả công cụ nếu có
        if (recipe.toolEfficiencyBoost > 0)
            StartCoroutine(ApplyToolEfficiencyBoost(recipe.toolEfficiencyBoost, recipe.toolEfficiencyDuration));
            
        // Play sound effect
        //AudioManager.instance.PlaySFX(9); // Giả sử 9 là âm thanh ăn

        SelectRecipe(recipe);
        UIController.instance.ShowMessage($"Đã sử dụng: {recipe.recipeName}");
    }

    // Coroutines để áp dụng các hiệu ứng tạm thời
    private System.Collections.IEnumerator ApplySpeedBoost(float amount, float duration)
    {
        float originalSpeed = PlayerController.instance.moveSpeed;
        PlayerController.instance.moveSpeed += amount;
        
        yield return new WaitForSeconds(duration);
        
        PlayerController.instance.moveSpeed = originalSpeed;
    }
    
    private System.Collections.IEnumerator ApplyToolEfficiencyBoost(float amount, float duration)
    {
        // Lưu giá trị tiêu thụ stamina ban đầu
        float originalStaminaUse = PlayerController.instance.staminaUsePerAction;
        
        // Giảm lượng stamina tiêu thụ (hiệu quả hơn)
        PlayerController.instance.staminaUsePerAction -= amount;
        
        yield return new WaitForSeconds(duration);
        
        // Khôi phục về giá trị ban đầu
        PlayerController.instance.staminaUsePerAction = originalStaminaUse;
    }
}