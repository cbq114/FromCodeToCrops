using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;
    
    private int currentMoney = 100; // Giá trị mặc định
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }
    
    public bool SpendMoney(int amount)
    {
        if (HasEnoughMoney(amount))
        {
            currentMoney -= amount;
            UIController.instance.UpdateMoneyText(currentMoney);
            return true;
        }
        return false;
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UIController.instance.UpdateMoneyText(currentMoney);
    }
    
    public int GetMoney()
    {
        return currentMoney;
    }
}