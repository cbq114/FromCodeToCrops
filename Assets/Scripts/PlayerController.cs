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
    public enum ToolType
    {
        plough,
        wateringCan,
        seeds,
        basket
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
        }

        if (toolWaitCounter > 0)
        {
            toolWaitCounter -= Time.deltaTime;
            theRB.linearVelocity = Vector2.zero;
        }
        else
        {
            //theRB.linearVelocity = new Vector2(moveSpeed, 0f);
            theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;


            if (theRB.linearVelocity.x < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (theRB.linearVelocity.x > 0f)
            {
                transform.localScale = Vector3.one;
            }
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
    }

void UseTool()
    {
        GrowBlock block = null;

        //block = FindFirstObjectByType<GrowBlock>();

        //block.PloughSoil();

        block = GridController.instance.GetBlock(toolIndicator.position.x - .5f, toolIndicator.position.y - .5f);

        toolWaitCounter = toolWaitTime;

        if(block != null)
        {
            switch(currentTool)
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

    public void SwitchSeed(CropController.CropType newSeed)
    {
        seedCropType = newSeed;
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
}
