using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PetSystem : MonoBehaviour
{
    public static PetSystem instance;

    [Header("Following Settings")]
    public bool isFollowing = true;

    [Header("Pet Settings")]
    public GameObject petPrefab;
    public GameObject activePet;
    public float followSpeed = 3f;
    public float minDistanceToPlayer = 1.5f;
    public float maxDistanceToPlayer = 3f;
    public string petType = "Dog"; // Added pet type

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

    [Header("Exploration Settings")]
    [SerializeField] private List<Transform> favoriteSpots = new List<Transform>();
    [SerializeField] private float exploreChance = 0.3f;

    private Transform playerTransform;
    private SpriteRenderer petRenderer;
    private float nextSearchTime;
    private float petIdleTimer;
    private Vector3 lastPosition;
    private bool isExploring = false;
    private Vector3 exploreTarget;

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
        lastPosition = activePet.transform.position;
    }

    private void Update()
    {
        if (activePet != null && playerTransform != null)
        {
            if (isFollowing && !isExploring)
            {
                FollowPlayer();
                CheckMovementAndDirection();
                if (Random.value < exploreChance * Time.deltaTime)
                    StartCoroutine(ExploreAround());
            }
            else if (isExploring)
            {
                ExploreArea();
            }
            else
            {
                if (petAnimator != null)
                {
                    petAnimator.SetBool("IsMoving", false);
                    petIdleTimer += Time.deltaTime;
                    if (petIdleTimer > 3f)
                    {
                        petAnimator.SetTrigger("PlayIdle");
                        petIdleTimer = 0;
                    }
                }
            }

            if (canFindItems && Time.time > nextSearchTime &&
                CropController.instance != null &&
                MoneyManager.instance != null &&
                PlayerController.instance != null)
            {
                CheckForItems();
            }
        }
    }

    private void FollowPlayer()
    {
        if (activePet == null || playerTransform == null) return;
        if (!isFollowing) return;

        Vector3 targetPos = playerTransform.position + new Vector3(-minDistanceToPlayer, 0, 0);
        float distanceToPlayer = Vector3.Distance(activePet.transform.position, playerTransform.position);

        float currentSpeed = followSpeed;
        if (distanceToPlayer > maxDistanceToPlayer)
            currentSpeed *= 2f;

        Vector3 newPos = Vector3.MoveTowards(activePet.transform.position, targetPos, currentSpeed * Time.deltaTime);
        activePet.transform.position = newPos;
    }

    private void CheckMovementAndDirection()
    {
        bool isMoving = Vector3.Distance(activePet.transform.position, lastPosition) > 0.001f;
        if (petAnimator != null)
            petAnimator.SetBool("IsMoving", isMoving);

        if (PlayerController.instance != null && petRenderer != null)
        {
            Vector2 playerVelocity = PlayerController.instance.theRB.linearVelocity;
            if (playerVelocity.x != 0)
                petRenderer.flipX = playerVelocity.x < 0;
        }

        lastPosition = activePet.transform.position;

        if (!isMoving)
        {
            petIdleTimer += Time.deltaTime;
            if (petIdleTimer > 5f)
            {
                int randomAction = Random.Range(0, 3);
                if (randomAction == 0 && petAnimator != null)
                    petAnimator.SetTrigger("PlayIdle");
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

            if (activePet.GetComponent<PetInteraction>() == null)
                activePet.AddComponent<PetInteraction>();

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
            Instantiate(affectionParticle, activePet.transform.position + Vector3.up * 0.5f, Quaternion.identity);

        if (UIController.instance != null)
            UIController.instance.ShowMessage($"Tình cảm với {petName} tăng lên! (Cấp {affectionLevel}/{maxAffectionLevel})");
    }

    public void UpdatePetAbilities()
    {
        staminaBoostPercentage = affectionLevel * 5f;
        movementSpeedBoost = affectionLevel * 0.1f;
        canFindItems = affectionLevel >= 3;

        if (PlayerController.instance != null)
            PlayerController.instance.moveSpeed = 5f + movementSpeedBoost;
    }

    private void CheckForItems()
    {
        if (CropController.instance == null || MoneyManager.instance == null ||
            PlayerController.instance == null || UIController.instance == null)
        {
            nextSearchTime = Time.time + 5f;
            return;
        }

        if (Random.value < 0.2f)
        {
            int itemType = Random.Range(0, 3);
            string itemName = "";
            switch (itemType)
            {
                case 0:
                    if (CropController.instance != null)
                    {
                        CropController.CropType randomCrop = (CropController.CropType)Random.Range(0, System.Enum.GetValues(typeof(CropController.CropType)).Length);
                        CropController.instance.AddSeed(randomCrop, 1);
                        itemName = randomCrop.ToString();
                    }
                    break;
                case 1:
                    if (MoneyManager.instance != null)
                    {
                        int coins = Random.Range(1, 5);
                        MoneyManager.instance.AddMoney(coins);
                        itemName = coins + " xu";
                    }
                    break;
                case 2:
                    if (PlayerController.instance != null)
                    {
                        float staminaBoost = Random.Range(5f, 10f);
                        PlayerController.instance.currentStamina = Mathf.Min(PlayerController.instance.currentStamina + staminaBoost, PlayerController.instance.maxStamina);
                        PlayerController.instance.UpdateStaminaUI();
                        itemName = "Củ cà rốt nhỏ (+" + staminaBoost + " stamina)";
                    }
                    break;
            }

            if (UIController.instance != null)
                UIController.instance.ShowMessage($"{petName} đã tìm thấy: {itemName}!");

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(5);
        }

        nextSearchTime = Time.time + Random.Range(60f, 180f);
    }

    public void PetInteraction()
    {
        // Tăng tình cảm như hiện tại
        IncreasePetAffection(1);


        // Hiệu ứng âm thanh
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(5);
    }

    public void FeedPet()
    {
        // Tăng tình cảm như hiện tại
        IncreasePetAffection(2);


        // Hiệu ứng âm thanh
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(5);
    }

    private void SavePetData()
    {
        PlayerPrefs.SetString("PetName", petName);
        PlayerPrefs.SetInt("PetAffection", affectionLevel);
        PlayerPrefs.Save();
    }

    private void LoadPetData()
    {
        if (PlayerPrefs.HasKey("PetIsFollowing"))
            isFollowing = PlayerPrefs.GetInt("PetIsFollowing") == 1;

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

    public void ToggleFollowing()
    {
        isFollowing = !isFollowing;
        PlayerPrefs.SetInt("PetIsFollowing", isFollowing ? 1 : 0);
        PlayerPrefs.Save();

        if (UIController.instance != null)
            UIController.instance.ShowMessage(isFollowing ?
                $"{petName} sẽ đi theo bạn." :
                $"{petName} sẽ ở yên tại chỗ.");


    }

    private IEnumerator ExploreAround()
    {
        isExploring = true;
        float exploreDistance = Random.Range(2f, 5f);
        float randomAngle = Random.Range(0, 360f) * Mathf.Deg2Rad;
        exploreTarget = playerTransform.position + new Vector3(
            Mathf.Cos(randomAngle) * exploreDistance,
            Mathf.Sin(randomAngle) * exploreDistance,
            0f
        );

        yield return new WaitForSeconds(Random.Range(3f, 8f));
        isExploring = false;


    }

    private void ExploreArea()
    {
        Vector3 newPos = Vector3.MoveTowards(activePet.transform.position, exploreTarget, followSpeed * 0.7f * Time.deltaTime);
        activePet.transform.position = newPos;

        bool isMoving = Vector3.Distance(activePet.transform.position, lastPosition) > 0.001f;
        if (petAnimator != null)
            petAnimator.SetBool("IsMoving", isMoving);

        if (petRenderer != null)
            petRenderer.flipX = (exploreTarget.x < activePet.transform.position.x);

        lastPosition = activePet.transform.position;
    }

}