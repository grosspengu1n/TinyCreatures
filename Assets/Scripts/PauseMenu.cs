using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PauseMenu : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button saveButton;
    public Button loadButton;
    public Button quitButton;

    [Header("Optional UI Elements")]
    public TextMeshProUGUI saveStatusText;
    public TextMeshProUGUI currentStatsText;

    private bool isPaused = false;
    private float timeScaleBeforePause;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        saveButton.onClick.AddListener(SaveGame);
        loadButton.onClick.AddListener(LoadGame);
        quitButton.onClick.AddListener(QuitGame);

        UpdateStatsDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        UpdateStatsDisplay();
    }

    void Resume()
    {
        Time.timeScale = timeScaleBeforePause;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    void SaveGame()
    {
        try
        {
            GameManager.Instance.SaveGame();
            ShowSaveStatus("Game saved successfully!", Color.green);
        }
        catch (Exception e)
        {
            ShowSaveStatus("Failed to save game!", Color.red);
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    void LoadGame()
    {
        try
        {
            GameManager.Instance.LoadGame();
            ShowSaveStatus("Game loaded successfully!", Color.green);
            UpdateStatsDisplay();
        }
        catch (Exception e)
        {
            ShowSaveStatus("Failed to load game!", Color.red);
            Debug.LogError($"Load failed: {e.Message}");
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void UpdateStatsDisplay()
    {
        if (currentStatsText != null)
        {
            string statsText = "Current Stats:\n";
            statsText += $"Currency: {GameManager.Instance.currency}\n";

            foreach (var upgrade in GameManager.Instance.upgrades)
            {
                statsText += $"{upgrade.name}: Level {upgrade.level} " +
                            $"(Value: {upgrade.CurrentValue:F1})\n";
            }

            currentStatsText.text = statsText;
        }
    }

    void ShowSaveStatus(string message, Color color)
    {
        if (saveStatusText != null)
        {
            saveStatusText.text = message;
            saveStatusText.color = color;

            CancelInvoke(nameof(ClearSaveStatus));
            Invoke(nameof(ClearSaveStatus), 2f);
        }
    }

    void ClearSaveStatus()
    {
        if (saveStatusText != null)
        {
            saveStatusText.text = "";
        }
    }
}