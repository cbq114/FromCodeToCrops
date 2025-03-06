using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameSave;
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

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

    void Start()
    {
        // Tự động tải game khi khởi động scene
        if (File.Exists(Application.persistentDataPath + "/savegame.json"))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Lưu vị trí người chơi
        if (PlayerController.instance != null)
        {
            data.playerPosX = PlayerController.instance.transform.position.x;
            data.playerPosY = PlayerController.instance.transform.position.y;
            data.playerStamina = PlayerController.instance.currentStamina;
        }

        // Lưu trạng thái nông trại
        if (GridController.instance != null)
        {
            data.farmTiles = new List<FarmTileData>();
            // Sửa: Dùng blocksList thay vì GetAllBlocks
            for (int y = 0; y < GridController.instance.blockRows.Count; y++)
            {
                for (int x = 0; x < GridController.instance.blockRows[y].blocks.Count; x++)
                {
                    GrowBlock block = GridController.instance.blockRows[y].blocks[x];

                    FarmTileData tileData = new FarmTileData
                    {
                        x = Mathf.FloorToInt(block.transform.position.x),
                        y = Mathf.FloorToInt(block.transform.position.y),
						isPlowed = (block.currentStage == GrowBlock.GrowthStage.ploughed),
						isWatered = block.isWatered
                        // Thêm thông tin về cây trồng nếu có
                    };
                    data.farmTiles.Add(tileData);
                }
            }
        }

        // Lưu tiền
        if (MoneyManager.instance != null)
        {
            data.money = MoneyManager.instance.GetMoney();
        }

        // Lưu kho hạt giống và nông sản
        if (CropController.instance != null)
        {
            // Sửa: Khởi tạo các dictionary serializable
            data.seedInventory = new SerializableDictionary();
            data.cropInventory = new SerializableDictionary();

            // Sửa: Cú pháp foreach đúng
            foreach (CropInfo crop in CropController.instance.cropList)
            {
                data.seedInventory.AddPair(crop.cropType.ToString(), crop.seedAmount);
                data.cropInventory.AddPair(crop.cropType.ToString(), crop.cropAmount);
            }
        }

        // Lưu thông tin thú cưng
        if (PetSystem.instance != null)
        {
            data.petName = PetSystem.instance.petName;
            data.petAffection = PetSystem.instance.affectionLevel;
        }

        // Lưu thức ăn thú cưng
        if (FindObjectOfType<PetMenuController>() != null)
        {
            data.petFoodCount = PlayerPrefs.GetInt("PetFoodCount", 0);
        }

        // Lưu thời gian và mùa
        if (TimeController.instance != null)
        {
			data.timeOfDay = TimeController.instance.currentTime;
			data.currentDay = TimeController.instance.currentDay;
        }

        if (SeasonSystem.instance != null)
        {
            data.currentSeason = (int)SeasonSystem.instance.currentSeason;
        }

        try
        {
            // Chuyển đổi sang JSON và lưu file
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);

            Debug.Log("Game đã được lưu thành công!");

            // Hiển thị thông báo cho người chơi
            if (UIController.instance != null)
                UIController.instance.ShowMessage("Game saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi khi lưu game: " + e.Message);
        }
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // Khôi phục vị trí người chơi
                if (PlayerController.instance != null)
                {
                    PlayerController.instance.transform.position = new Vector3(data.playerPosX, data.playerPosY, 0);
                    PlayerController.instance.currentStamina = data.playerStamina;
                    PlayerController.instance.UpdateStaminaUI();
                }

                // Khôi phục nông trại
                if (GridController.instance != null)
                {
                    foreach (FarmTileData tileData in data.farmTiles)
                    {
                        // Sửa: Sử dụng GetBlockAt thay vì GetBlock
                        GrowBlock block = GridController.instance.GetBlock(tileData.x, tileData.y);
                        if (block != null)
                        {
                            if (tileData.isPlowed)
                                block.PloughSoil(); // Không tiêu thể lực

                            if (tileData.isWatered)
                                block.WaterSoil();

                            // Khôi phục cây trồng nếu có
                        }
                    }
                }

                // Khôi phục tiền
                if (MoneyManager.instance != null)
                {
                    // Sửa: Sử dụng AddMoney thay vì gán trực tiếp
                    MoneyManager.instance.AddMoney(data.money - MoneyManager.instance.GetMoney());
                }

                // Khôi phục kho hạt giống và nông sản
                if (CropController.instance != null)
                {
                    // Sử dụng dictionary đã chuyển đổi
                    Dictionary<string, int> seedDict = data.seedInventory.ToDictionary();
                    Dictionary<string, int> cropDict = data.cropInventory.ToDictionary();

                    foreach (var pair in seedDict)
                    {
                        CropController.CropType cropType = (CropController.CropType)Enum.Parse(typeof(CropController.CropType), pair.Key);
                        CropController.instance.SetSeedAmount(cropType, pair.Value);
                    }

                    foreach (var pair in cropDict)
                    {
                        CropController.CropType cropType = (CropController.CropType)Enum.Parse(typeof(CropController.CropType), pair.Key);
                        CropController.instance.SetCropAmount(cropType, pair.Value);
                    }
                }

                // Khôi phục thông tin thú cưng
                if (PetSystem.instance != null)
                {
                    PetSystem.instance.petName = data.petName;
                    PetSystem.instance.affectionLevel = data.petAffection;
                }

                // Khôi phục thức ăn thú cưng
                if (FindObjectOfType<PetMenuController>() != null)
                {
                    PlayerPrefs.SetInt("PetFoodCount", data.petFoodCount);
                    PetMenuController petMenuCtrl = FindObjectOfType<PetMenuController>();
                    if (petMenuCtrl != null)
                    {
                        // Nếu muốn giữ các phương thức private, bạn có thể tạo một phương thức public mới:
                        petMenuCtrl.ReloadPetFoodData(); // Thêm phương thức này vào PetMenuController
                    }

                }

                // Khôi phục thời gian và mùa
                if (TimeController.instance != null)
                {
                    TimeController.instance.currentTime = data.timeOfDay;
                    TimeController.instance.currentDay = data.currentDay;
                    UIController.instance.UpdateTimeText(TimeController.instance.currentTime);
                }

                if (SeasonSystem.instance != null)
                {
                    SeasonSystem.instance.currentSeason = (SeasonSystem.Season)data.currentSeason;
                }

                Debug.Log("Game đã được tải thành công!");
                // Hiển thị thông báo cho người chơi
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("Game loaded successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("Lỗi khi tải game: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Không tìm thấy file lưu game!");
            if (UIController.instance != null)
                UIController.instance.ShowMessage("Save game file not found!");
        }
    }
}

// Thêm các lớp hỗ trợ serialization Dictionary
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

// Sửa SaveData để sử dụng SerializableDictionary
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
    public float timeOfDay;
}