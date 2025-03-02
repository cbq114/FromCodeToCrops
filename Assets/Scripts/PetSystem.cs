using System.Collections;
using UnityEngine;

public class PetSystem : MonoBehaviour
{
    public static PetSystem instance;

    [Header("Pet Settings")]
    public GameObject petPrefab;
    public GameObject activePet;
    public float followSpeed = 3f;
    public float minDistanceToPlayer = 1.5f;
    public float maxDistanceToPlayer = 3f;

    [Header("Pet Stats")]
    public string petName = "Buddy";
    public int affectionLevel = 0;
    public int maxAffectionLevel = 5;

    [Header("Pet Abilities")]
    public float staminaBoostPercentage = 0f;
    public float movementSpeedBoost = 0f;
    public bool canFindItems = false;

    [Header("Visual Feedback")]
    public GameObject affectionParticle;
    public Sprite[] affectionIcons;
    public Animator petAnimator;
    private Transform playerTransform;
    private SpriteRenderer petRenderer;
    private float nextSearchTime;
    private float petIdleTimer;
    private Vector3 lastPosition; // Theo dõi vị trí trước đó của pet
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerController.instance == null)
        {
            Debug.LogError("PlayerController.instance là null!");
            return;
        }

        playerTransform = PlayerController.instance.transform;
        LoadPetData();
        SpawnPet();
        UpdatePetAbilities();
        lastPosition = activePet.transform.position; // Khởi tạo vị trí ban đầu
    }

    void Update()
    {
        if (activePet != null && playerTransform != null)
        {
            FollowPlayer();
            CheckMovementAndDirection(); // Kiểm tra di chuyển và hướng

            if (canFindItems && Time.time > nextSearchTime)
            {
                CheckForItems();
            }
        }
    }

    private void FollowPlayer()
    {
        if (activePet == null || playerTransform == null) return;

        // Xác định vị trí mục tiêu (cách người chơi minDistanceToPlayer về phía bên trái)
        Vector3 targetPos = playerTransform.position + new Vector3(-minDistanceToPlayer, 0, 0);
        float distanceToPlayer = Vector3.Distance(activePet.transform.position, playerTransform.position);

        // Điều chỉnh tốc độ nếu pet quá xa
        float currentSpeed = followSpeed;
        if (distanceToPlayer > maxDistanceToPlayer)
        {
            currentSpeed *= 2f;
        }

        // Di chuyển pet
        Vector3 newPos = Vector3.MoveTowards(activePet.transform.position, targetPos, currentSpeed * Time.deltaTime);
        activePet.transform.position = newPos;
    }

    private void CheckMovementAndDirection()
    {
        // Kiểm tra xem pet có di chuyển hay không
        bool isMoving = Vector3.Distance(activePet.transform.position, lastPosition) > 0.001f;
        if (petAnimator != null)
        {
            petAnimator.SetBool("IsMoving", isMoving);
        }

        // Đồng bộ hướng của pet với hướng di chuyển của người chơi
        if (PlayerController.instance != null && petRenderer != null)
        {
            Vector2 playerVelocity = PlayerController.instance.theRB.linearVelocity;
            if (playerVelocity.x != 0)
            {
                petRenderer.flipX = playerVelocity.x < 0; // Lật sprite nếu người chơi di chuyển sang trái
            }
        }

        // Cập nhật vị trí trước đó
        lastPosition = activePet.transform.position;

        if (!isMoving)
        {
            petIdleTimer += Time.deltaTime;
            if (petIdleTimer > 5f)
            {
                int randomAction = Random.Range(0, 3);
                if (randomAction == 0 && petAnimator != null)
                {
                    petAnimator.SetTrigger("PlayIdle");
                }
                petIdleTimer = 0;
            }
        }
        else
        {
            petIdleTimer = 0;
        }
    }

    private void SpawnPet()
    {
        if (petPrefab != null && activePet == null && playerTransform != null)
        {
            Vector3 spawnPosition = playerTransform.position + new Vector3(1f, 0f, 0f);
            activePet = Instantiate(petPrefab, spawnPosition, Quaternion.identity);

            // Thêm PetInteraction script nếu chưa có
            if (activePet.GetComponent<PetInteraction>() == null)
            {
                activePet.AddComponent<PetInteraction>();
            }

            // Đảm bảo có Collider để phát hiện clicks
            if (activePet.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D col = activePet.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.5f;
            }

            Transform spriteTransform = activePet.transform.Find("Sprite");
            if (spriteTransform != null)
            {
                petAnimator = spriteTransform.GetComponent<Animator>();
                petRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            }
            else
            {
                Debug.LogError("Không tìm thấy GameObject con tên 'Sprite' trong prefab!");
            }

            if (petAnimator == null)
            {
                petAnimator = activePet.GetComponentInChildren<Animator>();
                if (petAnimator == null)
                    Debug.LogError("Không tìm thấy Animator trong pet prefab!");
            }
        }
    }
    public void IncreasePetAffection(int amount)
    {
        affectionLevel = Mathf.Min(affectionLevel + amount, maxAffectionLevel);
        SavePetData();
        UpdatePetAbilities();

        if (affectionParticle != null && activePet != null)
        {
            Instantiate(affectionParticle, activePet.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (UIController.instance != null)
        {
            UIController.instance.ShowMessage($"Tình cảm với {petName} tăng lên! (Cấp {affectionLevel}/{maxAffectionLevel})");
        }
    }

    public void UpdatePetAbilities()
    {
        staminaBoostPercentage = affectionLevel * 5f;
        movementSpeedBoost = affectionLevel * 0.1f;
        canFindItems = affectionLevel >= 3;

        if (PlayerController.instance != null)
        {
            PlayerController.instance.moveSpeed = 5f + movementSpeedBoost;
        }
    }

    private void CheckForItems()
    {
        if (Random.value < 0.2f)
        {
            int itemType = Random.Range(0, 3);
            string itemName = "";
            switch (itemType)
            {
                case 0:
                    CropController.CropType randomCrop = (CropController.CropType)Random.Range(0, System.Enum.GetValues(typeof(CropController.CropType)).Length);
                    CropController.instance.AddSeed(randomCrop, 1);
                    itemName = randomCrop.ToString();
                    break;
                case 1:
                    int coins = Random.Range(1, 5);
                    MoneyManager.instance.AddMoney(coins);
                    itemName = coins + " xu";
                    break;
                case 2:
                    float staminaBoost = Random.Range(5f, 10f);
                    PlayerController.instance.currentStamina = Mathf.Min(PlayerController.instance.currentStamina + staminaBoost, PlayerController.instance.maxStamina);
                    PlayerController.instance.UpdateStaminaUI();
                    itemName = "Củ cà rốt nhỏ (+" + staminaBoost + " stamina)";
                    break;
            }

            if (UIController.instance != null)
            {
                UIController.instance.ShowMessage($"{petName} đã tìm thấy: {itemName}!");
            }

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(5);
        }

        nextSearchTime = Time.time + Random.Range(60f, 180f);
    }

    public void PetInteraction()
    {
        if (petAnimator != null)
            petAnimator.SetTrigger("Happy");

        if (Random.value < 0.3f)
            IncreasePetAffection(1);
    }

    public void FeedPet()
    {
        IncreasePetAffection(1);

        if (petAnimator != null)
            petAnimator.SetTrigger("Eat");

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(7);
    }

    private void SavePetData()
    {
        PlayerPrefs.SetString("PetName", petName);
        PlayerPrefs.SetInt("PetAffection", affectionLevel);
        PlayerPrefs.Save();
    }

    private void LoadPetData()
    {
        if (PlayerPrefs.HasKey("PetName"))
        {
            petName = PlayerPrefs.GetString("PetName");
            affectionLevel = PlayerPrefs.GetInt("PetAffection");
        }
        else
        {
            petName = "Buddy";
            affectionLevel = 0;
            SavePetData();
        }
    }
}