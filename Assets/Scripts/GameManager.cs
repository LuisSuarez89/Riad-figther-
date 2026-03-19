using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Text timerText;
    [SerializeField] private Text bestTimeText;
    [SerializeField] private Text distanceText;
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Combustible")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float normalFuelConsumption = 4f;
    [SerializeField] private float turboFuelConsumption = 11f;
    [SerializeField] private float crashFuelPenalty = 18f;

    [Header("Puntaje")]
    [SerializeField] private float distanceScoreMultiplier = 1.6f;

    [Header("Tiempo")]
    [SerializeField] private string bestTimeKey = "BestSurvivalTime";
    [SerializeField] private string bestDistanceKey = "BestDistance";

    public bool IsGameOver { get; private set; }
    public float CurrentFuel { get; private set; }
    public bool TurboEnabled { get; private set; }
    public float SurvivalTime => survivalTime;
    public float DistanceTravelled => distanceTravelled;
    public float DifficultyMultiplier => 1f + Mathf.Clamp(survivalTime / 45f, 0f, 1.5f);

    private float survivalTime;
    private float bestTime;
    private float distanceTravelled;
    private float bestDistance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CurrentFuel = maxFuel;
        bestTime = PlayerPrefs.GetFloat(bestTimeKey, 0f);
        bestDistance = PlayerPrefs.GetFloat(bestDistanceKey, 0f);
        UpdateFuelUI();
        UpdateTimerUI();
        UpdateBestTimeUI();
        UpdateDistanceUI();
        UpdateStatusUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.touchCount > 0)
            {
                RestartGame();
            }

            return;
        }

        survivalTime += Time.deltaTime;
        ConsumeFuel();
        UpdateTimerUI();
        UpdateDistanceUI();
        UpdateStatusUI();

        if (CurrentFuel <= 0f)
        {
            TriggerGameOver();
        }
    }

    public void SetTurbo(bool enabled)
    {
        TurboEnabled = enabled;
        UpdateStatusUI();
    }

    public void RegisterTravel(float forwardSpeed)
    {
        if (IsGameOver)
        {
            return;
        }

        distanceTravelled += Mathf.Max(0f, forwardSpeed) * distanceScoreMultiplier * Time.deltaTime;
    }

    public void AddFuel(float amount)
    {
        if (IsGameOver)
        {
            return;
        }

        CurrentFuel = Mathf.Clamp(CurrentFuel + amount, 0f, maxFuel);
        UpdateFuelUI();
        UpdateStatusUI();
    }

    public void ApplyCrashPenalty()
    {
        if (IsGameOver)
        {
            return;
        }

        CurrentFuel = Mathf.Max(0f, CurrentFuel - crashFuelPenalty);
        UpdateFuelUI();
        UpdateStatusUI();

        if (CurrentFuel <= 0f)
        {
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        if (survivalTime > bestTime)
        {
            bestTime = survivalTime;
            PlayerPrefs.SetFloat(bestTimeKey, bestTime);
        }

        if (distanceTravelled > bestDistance)
        {
            bestDistance = distanceTravelled;
            PlayerPrefs.SetFloat(bestDistanceKey, bestDistance);
        }

        PlayerPrefs.Save();
        UpdateBestTimeUI();
        UpdateStatusUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ConsumeFuel()
    {
        float currentConsumption = TurboEnabled ? turboFuelConsumption : normalFuelConsumption;
        CurrentFuel = Mathf.Max(0f, CurrentFuel - currentConsumption * Time.deltaTime);
        UpdateFuelUI();
    }

    private void UpdateFuelUI()
    {
        if (fuelSlider != null)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = CurrentFuel;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Tiempo: {survivalTime:0.0}s";
        }
    }

    private void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
        {
            bestTimeText.text = $"Récord: {bestTime:0.0}s | Distancia: {bestDistance:0}m";
        }
    }

    private void UpdateDistanceUI()
    {
        if (distanceText != null)
        {
            distanceText.text = $"Distancia: {distanceTravelled:0}m";
        }
    }

    private void UpdateStatusUI()
    {
        if (statusText == null)
        {
            return;
        }

        if (IsGameOver)
        {
            statusText.text = "Game Over - toca la pantalla o presiona R para reiniciar";
            return;
        }

        string speedMode = TurboEnabled ? "Turbo" : "Cruise";
        statusText.text = $"Modo: {speedMode} | Dificultad x{DifficultyMultiplier:0.0}";
    }
}
