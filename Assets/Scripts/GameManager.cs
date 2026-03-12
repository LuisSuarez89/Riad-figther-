using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Text timerText;
    [SerializeField] private Text bestTimeText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Combustible")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float normalFuelConsumption = 4f;
    [SerializeField] private float turboFuelConsumption = 11f;

    [Header("Tiempo")]
    [SerializeField] private string bestTimeKey = "BestSurvivalTime";

    public bool IsGameOver { get; private set; }
    public float CurrentFuel { get; private set; }
    public bool TurboEnabled { get; private set; }

    private float survivalTime;
    private float bestTime;

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
        UpdateFuelUI();
        UpdateTimerUI();
        UpdateBestTimeUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsGameOver)
        {
            return;
        }

        survivalTime += Time.deltaTime;
        ConsumeFuel();
        UpdateTimerUI();

        if (CurrentFuel <= 0f)
        {
            TriggerGameOver();
        }
    }

    public void SetTurbo(bool enabled)
    {
        TurboEnabled = enabled;
    }

    public void AddFuel(float amount)
    {
        if (IsGameOver)
        {
            return;
        }

        CurrentFuel = Mathf.Clamp(CurrentFuel + amount, 0f, maxFuel);
        UpdateFuelUI();
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
            PlayerPrefs.Save();
        }

        UpdateBestTimeUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
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
            bestTimeText.text = $"Récord: {bestTime:0.0}s";
        }
    }
}
