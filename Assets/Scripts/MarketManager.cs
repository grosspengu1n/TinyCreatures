using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketManager : MonoBehaviour
{
    [Header("Player References")]
    public Movement playerMovement;

    [Header("UI Elements")]
    public GameObject upgradeMenu;
    public Button speedUpgradeButton;
    public TextMeshProUGUI currentSpeedText;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI currencyText;

    private float timeScaleBeforePause;

    void Start()
    {
        UpdateUI();

        speedUpgradeButton.onClick.AddListener(() => PurchaseUpgrade("Speed"));

        GameManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
        GameManager.Instance.OnUpgradeChanged += OnUpgradeChanged;

        GameManager.Instance.OnGameLoaded += OnGameLoaded;

        ApplyCurrentSpeed();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrencyChanged -= UpdateCurrencyUI;
            GameManager.Instance.OnUpgradeChanged -= OnUpgradeChanged;
            GameManager.Instance.OnGameLoaded -= OnGameLoaded;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleUpgradeMenu();
        }
    }

    public void ToggleUpgradeMenu()
    {
        bool isOpening = !upgradeMenu.activeSelf;
        upgradeMenu.SetActive(isOpening);

        if (isOpening)
        {
            timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            UpdateUI();
        }
        else
        {
            Time.timeScale = timeScaleBeforePause;
        }
    }

    public void PurchaseUpgrade(string upgradeName)
    {
        if (GameManager.Instance.TryPurchaseUpgrade(upgradeName))
        {
            if (upgradeName == "Speed")
            {
                ApplyCurrentSpeed();
            }
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        UpdateCurrencyUI(GameManager.Instance.currency);
        UpdateSpeedUI();
    }

    private void UpdateCurrencyUI(int currency)
    {
        currencyText.text = $"Currency: {currency}";
    }

    private void UpdateSpeedUI()
    {
        float currentSpeed = GameManager.Instance.GetUpgradeValue("Speed");
        int nextCost = GameManager.Instance.GetUpgradeNextCost("Speed");

        currentSpeedText.text = $"Current Speed: {currentSpeed:F1}";
        upgradeCostText.text = $"Upgrade Cost: {nextCost}";
    }

    private void OnUpgradeChanged(string upgradeName)
    {
        if (upgradeName == "Speed")
        {
            UpdateSpeedUI();
        }
    }

    private void ApplyCurrentSpeed()
    {
        float currentSpeed = GameManager.Instance.GetUpgradeValue("Speed");
        playerMovement.UpdateSpeed(currentSpeed);
    }
    public void ResetUpgrades()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DeleteSaveFile();
            GameManager.Instance.InitializeUpgrades();
            ApplyCurrentSpeed();
            UpdateUI();
        }
    }
    private void OnGameLoaded()
    {
        Debug.Log("Game loaded - updating market manager");
        UpdateUI();
        ApplyCurrentSpeed();
    }
}

