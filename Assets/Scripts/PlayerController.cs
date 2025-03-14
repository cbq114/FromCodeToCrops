using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

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

    public Rigidbody2D theRB;
    public float moveSpeed;

    public InputActionReference moveInput, actionInput;

    public Animator anim;
    public InventoryController theIC;

    [Header("Pet Interaction")]
    public PetMenuController petMenuController;
    public float petInteractionDistance = 2f; // Khoảng cách để tương tác với pet
    private bool isNearPet = false;
    public enum ToolType
    {
        plough,
        wateringCan,
        seeds,
        basket,
        fishingRod
    }
    public ToolType currentTool;

    public float toolWaitTime = .5f;
    private float toolWaitCounter;

    public Transform toolIndicator;
    public float toolRange = 3f;

    public CropController.CropType seedCropType;

    [Header("Stamina System")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f; // Hồi phục mỗi giờ game
    public float staminaUsePerAction = 5f; // Thể lực tiêu tốn khi làm việc
    public FishingManager fishingManager;
    public bool isWaitingForFish { get; set; } = false;
    public GameObject fishingRodVisual;

    private bool isInFishingArea = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.instance.SwitchTool((int)currentTool);

        UIController.instance.SwitchSeed(seedCropType);

        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    // Update is called once per frame
    void Update()
    {

        if (UIController.instance != null)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (UIController.instance.theIC != null)
            {
                if (UIController.instance.theIC.gameObject.activeSelf == true)
                {
                    theRB.linearVelocity = Vector2.zero;

                    return;
                }
            }

            if (UIController.instance.theShop != null)
            {
                if (UIController.instance.theShop.gameObject.activeSelf == true)
                {
                    theRB.linearVelocity = Vector2.zero;

                    return;
                }
            }

            if (UIController.instance.pauseScreen != null)
            {
                if (UIController.instance.pauseScreen.gameObject.activeSelf == true)
                {
                    theRB.linearVelocity = Vector2.zero;

                    return;
                }
            }

            if (petMenuController != null && petMenuController.petMenuPanel != null)
            {
                if (petMenuController.petMenuPanel.activeSelf)
                {
                    theRB.linearVelocity = Vector2.zero;
                    return;
                }
            }
        }

        if (isWaitingForFish || toolWaitCounter > 0)
        {
            theRB.linearVelocity = Vector2.zero;
            if (toolWaitCounter > 0)
            {
                toolWaitCounter -= Time.deltaTime;
                if (toolWaitCounter < 0)
                    toolWaitCounter = 0;
            }

            // Emergency escape - allow player to regain control with ESC key
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("Emergency control override!");
                isWaitingForFish = false;
                toolWaitCounter = 0;

                // Hide any tool visuals if needed
                if (fishingRodVisual != null)
                    fishingRodVisual.SetActive(false);
            }

            return;
        }
        else
        {
            theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;

            if (theRB.linearVelocity.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
            else if (theRB.linearVelocity.x > 0f)
                transform.localScale = Vector3.one;
        }
        bool hasSwitchedTool = false;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentTool++;

            if ((int)currentTool >= 4)
            {
                currentTool = ToolType.plough;
            }

            hasSwitchedTool = true;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentTool = ToolType.plough;

            hasSwitchedTool = true;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentTool = ToolType.wateringCan;

            hasSwitchedTool = true;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentTool = ToolType.seeds;

            hasSwitchedTool = true;
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentTool = ToolType.basket;

            hasSwitchedTool = true;
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            currentTool = ToolType.fishingRod;
            hasSwitchedTool = true;

            // Hiển thị nhân vật cầm cần câu

            if (isInFishingArea)
            {
                if (fishingRodVisual != null)
                    fishingRodVisual.SetActive(true);
            }
            else
            {
                // Show notification if not in fishing area
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("You must be near a fishing spot!");

                // Optional: Play error sound
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(7);

                // Make sure fishing rod is not visible
                if (fishingRodVisual != null)
                    fishingRodVisual.SetActive(false);
            }
        }

        if (hasSwitchedTool == true)
        {
            //FindFirstObjectByType<UIController>().SwitchTool((int)currentTool);

            UIController.instance.SwitchTool((int)currentTool);
        }



        anim.SetFloat("speed", theRB.linearVelocity.magnitude);

        if (GridController.instance != null)
        {

            if (actionInput.action.WasPressedThisFrame())
            {
                UseTool();
            }

            toolIndicator.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            toolIndicator.position = new Vector3(toolIndicator.position.x, toolIndicator.position.y, 0f);

            if (Vector3.Distance(toolIndicator.position, transform.position) > toolRange)
            {
                Vector2 direction = toolIndicator.position - transform.position;
                direction = direction.normalized * toolRange;
                toolIndicator.position = transform.position + new Vector3(direction.x, direction.y, 0f);
            }

            toolIndicator.position = new Vector3(Mathf.FloorToInt(toolIndicator.position.x) + .5f,
                Mathf.FloorToInt(toolIndicator.position.y) + .5f,
                0f);
        }
        else
        {
            toolIndicator.position = new Vector3(0f, 0f, -20f);
        }

        // Hồi phục thể lực theo thời gian
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime * TimeController.instance.timeSpeed;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            UpdateStaminaUI();
        }

        CheckPetProximity();
        // Thêm tương tác thú cưng
        if (isNearPet && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (petMenuController != null)
                petMenuController.TogglePetMenu();
            else
                Debug.LogWarning("Pet Menu Controller chưa được gán trong Inspector!");
        }

        // Giữ lại tính năng nhấn P để mở menu thú cưng từ xa
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (petMenuController != null)
                petMenuController.TogglePetMenu();
            else
                Debug.LogWarning("Pet Menu Controller chưa được gán trong Inspector!");
        }
    }

    void UseTool()
    {
        GrowBlock block = null;

        block = GridController.instance.GetBlock(toolIndicator.position.x - .5f, toolIndicator.position.y - .5f);

        toolWaitCounter = toolWaitTime;

        if (block != null)
        {
            // Kiểm tra stamina trước khi thực hiện hành động
            if (!UseStamina(staminaUsePerAction))
            {
                // Không đủ stamina, hiển thị thông báo
                if (UIController.instance != null)
                    UIController.instance.ShowMessage("Not fit enough!");

                // Phát âm thanh thông báo lỗi nếu có
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(7); // Giả sử 7 là âm thanh lỗi

                return; // Không thực hiện hành động nếu không đủ stamina
            }

            switch (currentTool)
            {
                case ToolType.plough:
                    block.PloughSoil();
                    anim.SetTrigger("usePlough");
                    break;

                case ToolType.wateringCan:
                    block.WaterSoil();
                    anim.SetTrigger("useWateringCan");
                    break;

                case ToolType.seeds:
                    if (CropController.instance.GetCropInfo(seedCropType).seedAmount > 0)
                    {
                        block.PlantCrop(seedCropType);
                        //CropController.instance.UseSeed(seedCropType);
                    }
                    break;

                case ToolType.basket:
                    block.HarvestCrop();
                    break;
            }
        }
    }
    public void EnterFishingArea()
    {
        isInFishingArea = true;
    }
    public void SwitchSeed(CropController.CropType newSeed)
    {
        seedCropType = newSeed;
    }
    public void ExitFishingArea()
    {
        isInFishingArea = false;

        // Cancel fishing if active
        if (isWaitingForFish && fishingManager != null)
        {
            fishingManager.CancelFishing();
            isWaitingForFish = false;
        }
    }
    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            UpdateStaminaUI();
            return true;
        }
        return false; // Không đủ thể lực
    }

    public void UpdateStaminaUI()
    {
        // Cập nhật UI thể lực
        if (UIController.instance != null)
        {
            UIController.instance.UpdateStaminaBar(currentStamina, maxStamina);
        }
    }
    public void ConsumeItem(Item item)
    {
        if (item.isConsumable && item.recipe != null)
        {
            CookingSystem.instance.ConsumeFood(item.recipe);
        }
    }
    public void ApplyPetBonuses()
    {
        if (PetSystem.instance != null)
        {
            // Hồi thể lực nhanh hơn dựa trên tình cảm với thú cưng
            float staminaBonus = PetSystem.instance.staminaBoostPercentage / 100f * staminaRegenRate;
            float actualRegenRate = staminaRegenRate + staminaBonus;

            // Áp dụng hồi phục thể lực nhanh hơn
            if (currentStamina < maxStamina)
            {
                currentStamina += actualRegenRate * Time.deltaTime;

                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;

                UpdateStaminaUI();
            }
        }
    }

    private void CheckPetProximity()
    {
        if (PetSystem.instance != null && PetSystem.instance.activePet != null)
        {
            float distanceToPet = Vector3.Distance(transform.position, PetSystem.instance.activePet.transform.position);
            isNearPet = distanceToPet <= petInteractionDistance;

            // Hiển thị gợi ý nếu ở gần pet
            if (UIController.instance != null && UIController.instance.interactionHint != null)
            {
                if (isNearPet)
                {
                    UIController.instance.interactionHint.SetActive(true);
                    UIController.instance.interactionHintText.text = "Press E to interact with pets";
                }
                else if (UIController.instance.interactionHint.activeSelf)
                {
                    UIController.instance.interactionHint.SetActive(false);
                }
            }
        }
    }
    public void StartFishing()
    {

        // Check if in fishing area first
        if (!isInFishingArea)
        {
            if (UIController.instance != null)
                UIController.instance.ShowMessage("You must be near a fishing spot!");

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(7);
            return;
        }
        if (!UseStamina(staminaUsePerAction))
        {
            if (UIController.instance != null)
                UIController.instance.ShowMessage("You're too tired to fish!");

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(7);
            return;
        }

        anim.SetTrigger("fishing");
        if (fishingManager != null)
        {
            isWaitingForFish = true;
            fishingManager.StartFishing();
        }
        else
        {
            Debug.LogWarning("Fishing Manager reference is missing!");
        }
    }
}
