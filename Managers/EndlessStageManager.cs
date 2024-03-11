using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class EndlessStageManager : MonoBehaviour
{
    [SerializeField] private StageData currentStageData; // Reference to current stage data
    [SerializeField] private TextMeshProUGUI highScoreText; // Reference to the HighScore UI Text component
    [SerializeField] private DataContainer dataContainer; // Reference to your DataContainer

    private string currentStageName;
    private int currentScore = 0;
    private int highScore = 0;

    public static EndlessStageManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private IEnumerator Start()
    {
        highScoreText.gameObject.SetActive(false); // Hide the high score text by default

        yield return new WaitUntil(() => StagesManager.instance.CurrentStageData != null);
        currentStageData = StagesManager.instance.CurrentStageData;

        if (currentStageData == null)
        {
            UnityEngine.Debug.LogError("CurrentStageData is null. Unable to set currentStageName.");
            yield break; // Exit if there is no current stage data
        }

        currentStageName = currentStageData.stageID;

        if (currentStageName == "GameplayStageEndless")
        {
            InitializeEndlessStage();
        }
    }

    private void InitializeEndlessStage()
    {
        LoadHighScore(); // Load high score from DataContainer
        highScoreText.gameObject.SetActive(true); // Show the high score text
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    public void EnemyKilled(EnemyData enemyData)
    {
        // Check if we are in the endless stage before updating the score
        if (currentStageName == "GameplayStageEndless")
        {
            int scoreToAdd = enemyData.isElite ? 1000 : 100;
            currentScore += scoreToAdd;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                SaveHighScore(); // Save new high score to DataContainer
            }

            UpdateHighScoreUI();
        }
        else
        {
            return;
            UnityEngine.Debug.LogWarning("EnemyKilled called from a non-endless stage.");
        }
    }

    private void UpdateHighScoreUI()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    private void LoadHighScore()
    {
        // Load the high score from DataContainer
        highScore = dataContainer.GetHighScore();
    }

    private void SaveHighScore()
    {
        // Save the high score to DataContainer
        dataContainer.SaveHighScore(highScore);
    }
}