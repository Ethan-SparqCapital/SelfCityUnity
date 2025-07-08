using LifeCraft.Core;
using LifeCraft.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using LifeCraft.UI;

namespace LifeCraft.Core
{
    /// <summary>
    /// Main game manager that coordinates all game systems and handles overall game flow.
    /// This is the central controller for the entire game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Systems")]
        [SerializeField] private CityBuilder cityBuilder;
        [SerializeField] private UnlockSystem unlockSystem;
        [SerializeField] private QuestManager questManager;
    //    [SerializeField] private SelfCareManager selfCareManager;
    //    [SerializeField] private WeatherSystem weatherSystem;

        [Header("UI")]
        [SerializeField] private UIManager uiManager;

        [Header("Game State")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private bool isGamePaused = false;

        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<bool> OnGamePaused;

        // Singleton instance
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            // Singleton pattern
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Start the game
            StartGame();
        }

        private void Update()
        {
            HandleInput();
            UpdateGameSystems();
        }

        /// <summary>
        /// Initialize all game systems
        /// </summary>
        private void InitializeGame()
        {
            // Find or create other systems
            if (cityBuilder == null)
                cityBuilder = FindFirstObjectByType<CityBuilder>();
            
            if (unlockSystem == null)
                unlockSystem = FindFirstObjectByType<UnlockSystem>();
            
            if (questManager == null)
                questManager = FindFirstObjectByType<QuestManager>();
            
            //if (selfCareManager == null)
                //selfCareManager = FindFirstObjectByType<SelfCareManager>();
            
            //if (weatherSystem == null)
                //weatherSystem = FindFirstObjectByType<WeatherSystem>();

            // Find UI manager
            if (uiManager == null)
                uiManager = FindFirstObjectByType<UIManager>();

            // Load saved game data
            LoadGameData();
        }

        /// <summary>
        /// Start the game
        /// </summary>
        private void StartGame()
        {
            SetGameState(GameState.Playing);
            
            // Initialize systems
            if (cityBuilder != null)
                cityBuilder.enabled = true;
            
            if (questManager != null)
                questManager.Initialize();
            
            //if (selfCareManager != null)
                //selfCareManager.Initialize();

            Debug.Log("Game started successfully!");
        }

        /// <summary>
        /// Handle input
        /// </summary>
        private void HandleInput()
        {
            // Pause/Resume
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            // Save game
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGame();
            }

            // Load game
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadGame();
            }
        }

        /// <summary>
        /// Update all game systems
        /// </summary>
        private void UpdateGameSystems()
        {
            if (isGamePaused) return;

            // Update weather
            //if (weatherSystem != null)
                //weatherSystem.UpdateWeather();

            // Update quests
            if (questManager != null)
                questManager.UpdateQuests();

            // Update self-care
            //if (selfCareManager != null)
                //selfCareManager.UpdateSelfCare();
        }

        /// <summary>
        /// Set the current game state
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentGameState != newState)
            {
                GameState previousState = currentGameState;
                currentGameState = newState;

                OnGameStateChanged?.Invoke(newState);

                Debug.Log($"Game state changed from {previousState} to {newState}");
            }
        }

        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            SetPaused(!isGamePaused);
        }

        /// <summary>
        /// Set pause state
        /// </summary>
        public void SetPaused(bool paused)
        {
            isGamePaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            
            OnGamePaused?.Invoke(paused);

            if (uiManager != null)
                uiManager.SetUIInteractable(!paused);

            Debug.Log($"Game {(paused ? "paused" : "resumed")}");
        }

        /// <summary>
        /// Save the current game
        /// </summary>
        public void SaveGame()
        {
            try
            {
                // Save resource data
                if (ResourceManager.Instance != null) ResourceManager.Instance.SaveResources();

                // Save city data
                if (cityBuilder != null)
                {
                    var cityData = cityBuilder.GetSaveData();
                    // TODO: Save to file
                }

                // Save unlock data
                if (unlockSystem != null)
                {
                    var unlockData = unlockSystem.GetSaveData();
                    // TODO: Save to file
                }

                // Save quest data
                if (questManager != null)
                    questManager.SaveQuests();

                // Save self-care data
                //if (selfCareManager != null)
                    //selfCareManager.SaveSelfCareData();

                Debug.Log("Game saved successfully!");
                
                if (uiManager != null)
                    uiManager.ShowNotification("Game saved!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                
                if (uiManager != null)
                    uiManager.ShowNotification("Failed to save game!");
            }
        }

        /// <summary>
        /// Load a saved game
        /// </summary>
        public void LoadGame()
        {
            try
            {
                // Load resource data
                if (ResourceManager.Instance != null) ResourceManager.Instance.LoadResources();

                // Load city data
                if (cityBuilder != null)
                {
                    // TODO: Load from file
                    // var cityData = LoadCityData();
                    // cityBuilder.LoadCity(cityData);
                }

                // Load unlock data
                if (unlockSystem != null)
                {
                    // TODO: Load from file
                    // var unlockData = LoadUnlockData();
                    // unlockSystem.LoadSaveData(unlockData);
                }

                // Load quest data
                //if (questManager != null)
                    //questManager.LoadQuests();

                // Load self-care data
                //if (selfCareManager != null)
                    //selfCareManager.LoadSelfCareData();

                Debug.Log("Game loaded successfully!");
                
                if (uiManager != null)
                    uiManager.ShowNotification("Game loaded!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                
                if (uiManager != null)
                    uiManager.ShowNotification("Failed to load game!");
            }
        }

        /// <summary>
        /// Load game data (called on initialization)
        /// </summary>
        private void LoadGameData()
        {
            // Load saved data if it exists
            // For now, just initialize with default values
            Debug.Log("Loading game data...");
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            SaveGame();
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        public void RestartGame()
        {
            SaveGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Get current game state
        /// </summary>
        public GameState CurrentGameState => currentGameState;

        /// <summary>
        /// Check if game is paused
        /// </summary>
        public bool IsGamePaused => isGamePaused;

        /// <summary>
        /// Get city builder
        /// </summary>
        public CityBuilder CityBuilder => cityBuilder;

        /// <summary>
        /// Get unlock system
        /// </summary>
        public UnlockSystem UnlockSystem => unlockSystem;

        /// <summary>
        /// Get quest manager
        /// </summary>
        public QuestManager QuestManager => questManager;

        /// <summary>
        /// Get self-care manager
        /// </summary>
        //public SelfCareManager SelfCareManager => selfCareManager;

        /// <summary>
        /// Get UI manager
        /// </summary>
        public UIManager UIManager => uiManager;
    }

    /// <summary>
    /// Game states
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Building,
        Quest,
        Settings,
        GameOver
    }
} 