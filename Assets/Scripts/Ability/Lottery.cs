using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lottery : MonoBehaviour
{
    public static Lottery Instance { get; private set; }

    public AbilityRepository abilityRepository; // Reference to the AbilityRepository
    public AbilityHandle playerAbilityHandle;   // Reference to the player's AbilityHandle
    public CardSlot[] lotteryCards;            // UI card slots for displaying the abilities
    public Button okayButton;

    private AbilityUIImages abilityUIImages;   // Reference to the Ability UI system

    private const int MaxActiveAbilities = 4;  // Limit for active abilities
    private const int MaxPassiveAbilities = 4; // Limit for passive abilities
    private const int PoolSize = 8;            // Size of the rotating ability pool

    private List<BaseAbility> activePool = new List<BaseAbility>();  // Pool for active abilities
    private List<BaseAbility> passivePool = new List<BaseAbility>(); // Pool for passive abilities

    private Queue<int> levelUpQueue = new Queue<int>(); // Queue to hold level-ups
    private bool isProcessingLottery = false;          // Flag to prevent concurrent processing

    private Lottery() { } // Private constructor to prevent instantiation from outside

    private void Start()
    {
        InitializeSingleton();
        FindDependencies();
        InitializeAbilityPools();
        okayButton.onClick.AddListener(OnOkayButtonClicked);
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist through scenes
        }
        else
        {
            Destroy(gameObject);  // Prevent multiple instances
        }
    }

    private void FindDependencies()
    {
        abilityRepository = FindObjectOfType<AbilityRepository>();
        abilityUIImages = FindObjectOfType<AbilityUIImages>();

        if (abilityRepository == null)
        {
            Debug.LogError("AbilityRepository not found in the scene!");
        }

        if (abilityUIImages == null)
        {
            Debug.LogError("AbilityUIImages not found in the scene!");
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerAbilityHandle = player.GetComponent<PlayerHandle>().GetAbilityHandle();
            if (playerAbilityHandle == null)
            {
                Debug.LogError("Player does not have an AbilityHandle component!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void InitializeAbilityPools()
    {
        foreach (BaseAbility ability in abilityRepository.GetAbilities())
        {
            if (!playerAbilityHandle.HasAbility(ability)) // Exclude abilities the player already has
            {
                if (ability.IsActive)
                {
                    activePool.Add(ability);
                }
                else
                {
                    passivePool.Add(ability);
                }
            }
        }

        TrimPool(activePool);
        TrimPool(passivePool);
    }

    private void TrimPool(List<BaseAbility> pool)
    {
        while (pool.Count > PoolSize)
        {
            pool.RemoveAt(Random.Range(0, pool.Count));
        }
    }

    public void EnqueueLevelUp(int level)
    {
        levelUpQueue.Enqueue(level); // Add level-up to the queue
        Debug.Log($"Level-up queued for level {level}. Queue size: {levelUpQueue.Count}");

        // Start processing if not already in progress
        if (!isProcessingLottery)
        {
            StartCoroutine(ProcessLevelUpQueue());
        }
    }

    private IEnumerator ProcessLevelUpQueue()
    {
        isProcessingLottery = true;

        while (levelUpQueue.Count > 0)
        {
            int level = levelUpQueue.Dequeue(); // Get the next level-up
            Debug.Log($"Processing level-up for level {level}. Remaining in queue: {levelUpQueue.Count}");

            // Process the lottery for this level-up
            yield return StartCoroutine(ConductLottery());

            Debug.Log($"Finished level-up for level {level}");
        }

        isProcessingLottery = false;
        Debug.Log("Finished processing level-up queue.");
    }

    public IEnumerator ConductLottery()
    {
        HashSet<BaseAbility> selectedAbilities = new HashSet<BaseAbility>();

        if (playerAbilityHandle.GetActiveAbilities().Count >= MaxActiveAbilities &&
            playerAbilityHandle.GetPassiveAbilities().Count >= MaxPassiveAbilities)
        {
            // Both active and passive slots are full; only select from the player's current abilities
            selectedAbilities.UnionWith(GetPlayerAbilities());
        }
        else
        {
            // Select abilities from dynamic pools without duplicates
            selectedAbilities.UnionWith(GetRandomAbilitiesFromPool(activePool, 2));
            selectedAbilities.UnionWith(GetRandomAbilitiesFromPool(passivePool, 1));
        }

        yield return StartCoroutine(DisplayAbilitiesSequentially(new List<BaseAbility>(selectedAbilities)));
    }

    private List<BaseAbility> GetPlayerAbilities()
    {
        List<BaseAbility> playerAbilities = new List<BaseAbility>();
        playerAbilities.AddRange(playerAbilityHandle.GetActiveAbilities());
        playerAbilities.AddRange(playerAbilityHandle.GetPassiveAbilities());
        return playerAbilities;
    }

    private List<BaseAbility> GetRandomAbilitiesFromPool(List<BaseAbility> pool, int count)
    {
        HashSet<int> selectedIndices = new HashSet<int>();
        List<BaseAbility> randomAbilities = new List<BaseAbility>();

        while (selectedIndices.Count < count && selectedIndices.Count < pool.Count)
        {
            int randomIndex = Random.Range(0, pool.Count);
            if (selectedIndices.Add(randomIndex))
            {
                randomAbilities.Add(pool[randomIndex]);
            }
        }

        return randomAbilities;
    }

    private IEnumerator DisplayAbilitiesSequentially(List<BaseAbility> selectedAbilities)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0f;

        EventManager<object>.TriggerEvent(EventKey.SHOW_LEVELUP_UI, null);

        for (int i = 0; i < lotteryCards.Length; i++)
        {
            if (i < selectedAbilities.Count && selectedAbilities[i] != null)
            {
                lotteryCards[i].SetAbility(selectedAbilities[i]);
            }
        }

        // Wait for the player to select an ability
        yield return new WaitUntil(() => !EventManager<object>.IsEventActive(EventKey.SHOW_LEVELUP_UI));

        Time.timeScale = 1f;
        Debug.Log("Ability selection completed.");
    }

    private void OnOkayButtonClicked()
    {
        BaseAbility chosenAbility = GetSelectedAbility();
        if (chosenAbility != null)
        {
            HandleSelectedAbility(chosenAbility);
            EventManager<object>.TriggerEvent(EventKey.HIDE_LEVELUP_UI, null);
            EventManager<int>.EndEvent(EventKey.UPDATE_PLAYER_LEVEL);

        }
    }

    private BaseAbility GetSelectedAbility()
    {
        foreach (CardSlot card in lotteryCards)
        {
            if (card.IsCardSelected())
            {
                return card.ability;
            }
        }
        return null;
    }

    private void HandleSelectedAbility(BaseAbility chosenAbility)
    {
        var abilityHandle = PlayerManager.Instance.User.GetAbilityHandle();
        BaseAbility existingAbility = abilityHandle.storedAbilities.Find(a => a.Name == chosenAbility.Name);

        if (existingAbility != null)
        {
            existingAbility.LevelUp();
            Debug.Log($"{chosenAbility.Name} leveled up!");
        }
        else
        {
            AbilityManager.Instance.AddAbility(chosenAbility);
            RegisterAbilityInUI(chosenAbility);
            Debug.Log($"{chosenAbility.Name} added to abilities.");
        }
    }

    private void RegisterAbilityInUI(BaseAbility ability)
    {
        // Construct the resource path based on the ability name
        string resourcePath = $"AbilityUI/{ability.Name}";

        // Load the sprite from the Resources folder
        Sprite abilitySprite = Resources.Load<Sprite>(resourcePath);

        if (abilitySprite != null)
        {
            abilityUIImages.RegisterAbilityUI(ability, abilitySprite);
            Debug.Log($"Registered ability UI for {ability.Name} with sprite at {resourcePath}.");
        }
        else
        {
            Debug.LogError($"Sprite for ability '{ability.Name}' not found at {resourcePath}. Ensure the sprite exists in the Resources folder.");
        }
    }
}