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

    [Header("Upgrade Settings")]
    public float baseSpeed = 5f;
    public float speedIncreasePerUpgrade = 1f;
    public int baseCost = 100;
    public float costMultiplier = 1.5f;

    private int currentUpgradeLevel = 0;
    private int currentCost;
    private float timeScaleBeforePause;

    void Start()
    {
        currentCost = baseCost;
        UpdateUI();

        speedUpgradeButton.onClick.AddListener(PurchaseSpeedUpgrade);
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
        }
        else
        {
            Time.timeScale = timeScaleBeforePause;
        }
    }

    public void PurchaseSpeedUpgrade()
    {
        if (CanAffordUpgrade(currentCost))
        {
            DeductCurrency(currentCost);

            currentUpgradeLevel++;
            float newSpeed = baseSpeed + (speedIncreasePerUpgrade * currentUpgradeLevel);
            playerMovement.UpdateSpeed(newSpeed);

            currentCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentUpgradeLevel));

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        float currentSpeed = baseSpeed + (speedIncreasePerUpgrade * currentUpgradeLevel);
        currentSpeedText.text = $"Current Speed: {currentSpeed:F1}";
        upgradeCostText.text = $"Upgrade Cost: {currentCost}";
    }

    private bool CanAffordUpgrade(int amount)
    {
        if (GameManager.Instance.currency >= amount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DeductCurrency(int amount)
    {
        GameManager.Instance.currency -= amount;
    }
}

