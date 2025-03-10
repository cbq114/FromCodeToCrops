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
                        isWatered = block.isWatered,
                        // Thêm thông tin về cây trồng nếu có
                        cropType = block.currentCrop != null ? block.currentCrop.cropType.ToString() : "",
                        growthStage = block.currentCrop != null ? (int)block.currentCrop.currentGrowthStage : -1,
                        growthTime = block.currentCrop != null ? block.currentCrop.currentGrowthTime : 0f
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
            // Khởi tạo các dictionary serializable
            data.seedInventory = new SerializableDictionary();
            data.cropInventory = new SerializableDictionary();

            // Lưu từng loại cây trồng
            foreach (CropInfo crop in CropController.instance.cropList)
            {
                if (crop != null)
                {
                    data.seedInventory.AddPair(crop.cropType.ToString(), crop.seedAmount);
                    data.cropInventory.AddPair(crop.cropType.ToString(), crop.cropAmount);
                }
            }
        }

        // Lưu thông tin thú cưng
        if (PetSystem.instance != null)
        {
            data.petName = PetSystem.instance.petName;
            data.petAffection = PetSystem.instance.affectionLevel;
            data.petFoodCount = PetSystem.instance.foodCount;
        }
        else
        {
            // Lưu thức ăn thú cưng từ PlayerPrefs nếu không có instance
            data.petFoodCount = PlayerPrefs.GetInt("PetFoodCount", 0);
        }

        // Lưu thời gian và mùa
        if (TimeController.instance != null)
        {
            data.currentTime = TimeController.instance.currentTime;
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
            Debug.Log("Saved data: " + json);

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

                Debug.Log("Loading data: " + json);

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
                            // Reset block trước khi áp dụng dữ liệu mới
                            block.ResetBlock();
                            
                            if (tileData.isPlowed)
                                block.PloughSoil(false); // Không tiêu thể lực

                            if (tileData.isWatered)
                                block.WaterSoil(false); // Không tiêu thể lực

                            // Khôi phục cây trồng nếu có
                            if (!string.IsNullOrEmpty(tileData.cropType) && tileData.growthStage >= 0)
                            {
                                CropController.CropType cropType = (CropController.CropType)Enum.Parse(
                                    typeof(CropController.CropType), tileData.cropType);
                                
                                // Tạo cây trồng mới
                                block.PlantCrop(cropType, false); // Không tiêu thể lực và hạt giống
                                
                                if (block.currentCrop != null)
                                {
                                    // Thiết lập giai đoạn phát triển
                                    block.currentCrop.currentGrowthStage = (CropController.GrowthStage)tileData.growthStage;
                                    block.currentCrop.currentGrowthTime = tileData.growthTime;
                                    block.currentCrop.UpdateVisuals();
                                }
                            }
                        }
                    }
                }

                // Khôi phục tiền
                if (MoneyManager.instance != null)
                {
                    // Đặt lại tiền về 0 trước khi thêm
                    MoneyManager.instance.SetMoney(0);
                    MoneyManager.instance.AddMoney(data.money);
                }

                // Khôi phục kho hạt giống và nông sản
                if (CropController.instance != null)
                {
                    // Xóa dữ liệu hiện tại
                    CropController.instance.ResetInventory();
                    
                    // Sử dụng dictionary đã chuyển đổi
                    Dictionary<string, int> seedDict = data.seedInventory?.ToDictionary() ?? new Dictionary<string, int>();
                    Dictionary<string, int> cropDict = data.cropInventory?.ToDictionary() ?? new Dictionary<string, int>();

                    foreach (var pair in seedDict)
                    {
                        try
                        {
                            CropController.CropType cropType = (CropController.CropType)Enum.Parse(typeof(CropController.CropType), pair.Key);
                            CropController.instance.SetSeedAmount(cropType, pair.Value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Không thể chuyển đổi loại cây trồng: {pair.Key}. Lỗi: {e.Message}");
                        }
                    }

                    foreach (var pair in cropDict)
                    {
                        try
                        {
                            CropController.CropType cropType = (CropController.CropType)Enum.Parse(typeof(CropController.CropType), pair.Key);
                            CropController.instance.SetCropAmount(cropType, pair.Value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Không thể chuyển đổi loại nông sản: {pair.Key}. Lỗi: {e.Message}");
                        }
                    }
                    
                    // Cập nhật UI hiển thị kho
                    CropController.instance.UpdateInventoryUI();
                }

                // Khôi phục thông tin thú cưng
                if (PetSystem.instance != null)
                {
                    PetSystem.instance.petName = data.petName;
                    PetSystem.instance.affectionLevel = data.petAffection;
                    PetSystem.instance.foodCount = data.petFoodCount;
                    PetSystem.instance.UpdatePetUI();
                }
                else
                {
                    // Lưu vào PlayerPrefs nếu không có instance
                    PlayerPrefs.SetInt("PetFoodCount", data.petFoodCount);
                    
                    // Nếu có PetMenuController, cập nhật UI
                    PetMenuController petMenuCtrl = FindObjectOfType<PetMenuController>();
                    if (petMenuCtrl != null)
                    {
                        petMenuCtrl.ReloadPetFoodData();
                    }
                }

                // Khôi phục thời gian và mùa
                if (TimeController.instance != null)
                {
                    TimeController.instance.currentTime = data.currentTime;
                    TimeController.instance.currentDay = data.currentDay;
                    TimeController.instance.UpdateTimeUI();
                }

                if (SeasonSystem.instance != null)
                {
                    SeasonSystem.instance.currentSeason = (SeasonSystem.Season)data.currentSeason;
                    SeasonSystem.instance.UpdateSeasonUI();
                }

                Debug.Log("Game đã được tải thành công!");
                // Hiển thị thông báo cho người chơi
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("Game loaded successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("Lỗi khi tải game: " + e.Message);
                Debug.LogException(e);
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

// Các lớp StringIntPair và SerializableDictionary đã được chuyển sang SaveData.cs