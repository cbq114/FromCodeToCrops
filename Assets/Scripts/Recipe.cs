using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Farming Game/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public Sprite recipeImage;
    public string description;
    
    [Header("Ingredients")]
    public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    
    [Header("Result")]
    public Item resultItem;
    public int resultAmount = 1;
    
    [Header("Effects")]
    public float staminaRestoreAmount = 20f;
    public float speedBoostAmount = 0f;
    public float speedBoostDuration = 0f;
    public float toolEfficiencyBoost = 0f;
    public float toolEfficiencyDuration = 0f;
}

[System.Serializable]
public class RecipeIngredient
{
    public enum IngredientType { Crop, Fish }

    public IngredientType ingredientType;
    public CropController.CropType cropType;
    public FishController.FishType fishType;
    public int amount = 1;
}