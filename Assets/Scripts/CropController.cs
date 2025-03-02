using System.Collections.Generic;
using UnityEngine;

public class CropController : MonoBehaviour
{
    public static CropController instance;

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

    public enum CropType
    {
        pumpkin,
        lettuce,
        carrot,
        hay,
        potato,
        strawberry,
        tomato,
        avocado
    }

    public List<CropInfo> cropList = new List<CropInfo>();


    public CropInfo GetCropInfo(CropType cropToGet)
    {
        int position = -1;

        for (int i = 0; i < cropList.Count; i++)
        {
            if (cropList[i].cropType == cropToGet)
            {
                position = i;
            }
        }

        if (position >= 0)
        {
            return cropList[position];
        }
        else
        {
            return null;
        }
    }

    public void UseSeed(CropType seedToUse)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == seedToUse)
            {
                info.seedAmount--;
            }
        }
    }

    public void AddCrop(CropType cropToAdd)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == cropToAdd)
            {
                info.cropAmount++;
            }
        }
    }

    public void AddSeed(CropType seedToAdd, int amount)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == seedToAdd)
            {
                info.seedAmount += amount;
            }
        }
    }

    public void RemoveCrop(CropType cropToRemove)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == cropToRemove)
            {
                info.cropAmount = 0;
            }

        }
    }

    public void UseCrop(CropType cropToUse, int amount)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == cropToUse)
            {
                info.cropAmount -= amount;
            }
        }
    }
    public void SetSeedAmount(CropType cropType, int amount)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == cropType)
            {
                info.seedAmount = amount;
                break;
            }
        }
    }

    public void SetCropAmount(CropType cropType, int amount)
    {
        foreach (CropInfo info in cropList)
        {
            if (info.cropType == cropType)
            {
                info.cropAmount = amount;
                break;
            }
        }
    }
}

[System.Serializable]
public class CropInfo
{
    public CropController.CropType cropType;
    public Sprite finalCrop, seedType, planted, growStage1, growStage2, ripe;

    public int seedAmount, cropAmount;

    [Range(0f, 100f)]
    public float growthFailChance;

    public float seedPrice, cropPrice;
}
