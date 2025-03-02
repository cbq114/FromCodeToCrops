using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSave
{
    [Serializable]
    public class SaveData
    {
        // Thông tin người chơi
        public float playerPosX;
        public float playerPosY;
        public float playerStamina;

        // Hệ thống nông trại
        public List<FarmTileData> farmTiles = new List<FarmTileData>();

        // Dữ liệu vật phẩm
        public int money;
        public SerializableDictionary seedInventory;
        public SerializableDictionary cropInventory;

        // Dữ liệu thú cưng
        public string petName;
        public int petAffection;
        public int petFoodCount;

        // Thời gian và mùa
        public int currentDay;
        public int currentSeason;
        public float currentTime;
    }

    [Serializable]
    public class FarmTileData
    {
        public int x;
        public int y;
        public bool isPlowed;
        public bool isWatered;
        public string cropType;
        public int growthStage;
        public float growthTime;
    }
}