using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public int level;
        public float baseValue;
        public float increasePerLevel;
        public int baseCost;
        public float costMultiplier;

        public float CurrentValue => baseValue + (increasePerLevel * level);
        public int NextUpgradeCost => Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    [System.Serializable]
    public class SaveData
    {
        public int currency;
        public List<UpgradeSaveData> upgrades = new List<UpgradeSaveData>();
    }

    [System.Serializable]
    public class UpgradeSaveData
    {
        public string name;
        public int level;
    }

    [Header("Player Stats")]
    public int currency = 0;

    [Header("Upgrades")]
    public List<Upgrade> upgrades = new List<Upgrade>();

    public event Action<int> OnCurrencyChanged;
    public event Action<string> OnUpgradeChanged;
    public event Action OnGameLoaded;

    private string SavePath => Path.Combine(Application.persistentDataPath, "gamesave.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeUpgrades()
    {
        if (upgrades.Count == 0)
        {
            upgrades.Add(new Upgrade
            {
                name = "Speed",
                level = 0,
                baseValue = 5f,
                increasePerLevel = 1f,
                baseCost = 10,
                costMultiplier = 1.5f
            });

        }

        LoadGame();
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(currency);
        SaveGame();
    }

    public bool TryPurchaseUpgrade(string upgradeName)
    {
        Upgrade upgrade = upgrades.Find(u => u.name == upgradeName);
        if (upgrade == null) return false;

        int cost = upgrade.NextUpgradeCost;
        if (currency >= cost)
        {
            currency -= cost;
            upgrade.level++;

            OnCurrencyChanged?.Invoke(currency);
            OnUpgradeChanged?.Invoke(upgradeName);

            SaveGame();
            return true;
        }
        return false;
    }

    public float GetUpgradeValue(string upgradeName)
    {
        Upgrade upgrade = upgrades.Find(u => u.name == upgradeName);
        return upgrade?.CurrentValue ?? 0f;
    }

    public int GetUpgradeLevel(string upgradeName)
    {
        Upgrade upgrade = upgrades.Find(u => u.name == upgradeName);
        return upgrade?.level ?? 0;
    }

    public int GetUpgradeNextCost(string upgradeName)
    {
        Upgrade upgrade = upgrades.Find(u => u.name == upgradeName);
        return upgrade?.NextUpgradeCost ?? 0;
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            currency = currency,
            upgrades = upgrades.Select(u => new UpgradeSaveData
            {
                name = u.name,
                level = u.level
            }).ToList()
        };

        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game saved successfully to {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                currency = saveData.currency;

                foreach (var savedUpgrade in saveData.upgrades)
                {
                    var upgrade = upgrades.Find(u => u.name == savedUpgrade.name);
                    if (upgrade != null)
                    {
                        upgrade.level = savedUpgrade.level;
                    }
                }

                OnCurrencyChanged?.Invoke(currency);
                foreach (var upgrade in upgrades)
                {
                    OnUpgradeChanged?.Invoke(upgrade.name);
                }

                OnGameLoaded?.Invoke();

                Debug.Log($"Game loaded successfully from {SavePath}");
            }
            else
            {
                Debug.Log("No save file found, starting new game");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            throw;
        }
    }

    public void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted successfully");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }
}
