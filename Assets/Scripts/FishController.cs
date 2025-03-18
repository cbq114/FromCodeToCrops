using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public static FishController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum FishType
    {
        Salmon,
        Tuna,
        Catfish
    }

    public List<FishInfo> fishList = new List<FishInfo>();

    public FishInfo GetFishInfo(FishType fishToGet)
    {
        foreach (FishInfo info in fishList)
        {
            if (info.fishType == fishToGet)
            {
                return info;
            }
        }
        return null;
    }

    public void AddFish(FishType fishToAdd)
    {
        foreach (FishInfo info in fishList)
        {
            if (info.fishType == fishToAdd)
            {
                info.fishAmount++;
                return;
            }
        }
    }

    public void RemoveFish(FishType fishType, int amount)
    {
        foreach (FishInfo info in fishList)
        {
            if (info.fishType == fishType)
            {
                info.fishAmount = Mathf.Max(0, info.fishAmount - amount);
                return;
            }
        }
    }


    public void SetFishAmount(FishType fishType, int amount)
    {
        foreach (FishInfo info in fishList)
        {
            if (info.fishType == fishType)
            {
                info.fishAmount = amount;
                break;
            }
        }
    }
}

[System.Serializable]
public class FishInfo
{
    public FishController.FishType fishType;
    public Sprite fishSprite;
    public int fishAmount;
    public float fishPrice;
}
