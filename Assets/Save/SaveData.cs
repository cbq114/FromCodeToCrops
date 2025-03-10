using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSave
{
    [Serializable]
    public class StringIntPair
    {
        public string key;
        public int value;
    }

    [Serializable]
    public class SerializableDictionary
    {
        public List<StringIntPair> pairs = new List<StringIntPair>();

        public void AddPair(string key, int value)
        {
            pairs.Add(new StringIntPair { key = key, value = value });
        }

        public Dictionary<string, int> ToDictionary()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var pair in pairs)
            {
                result[pair.key] = pair.value;
            }
            return result;
        }
    }

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