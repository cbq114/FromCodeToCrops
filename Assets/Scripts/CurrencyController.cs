using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }


    public float currentMoney;

    private void Start()
    {
        UIController.instance.UpdateMoneyText(currentMoney);
    }

    public void SpendMoney(float amountToSpend)
    {
        currentMoney -= amountToSpend;

        UIController.instance.UpdateMoneyText(currentMoney);
    }
   
    public void AddMoney(float amountToAdd)
    {
        currentMoney += amountToAdd;

        UIController.instance.UpdateMoneyText(currentMoney);
    }

    public bool CheckMoney(float amount)
    {
        if(currentMoney >= amount)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
