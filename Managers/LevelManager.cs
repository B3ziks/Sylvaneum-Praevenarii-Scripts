using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private GameplayStageData[] levels;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameplayStageData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
        {
            // Game Over or Loop levels
            currentLevelIndex = 0;
        }
        // Load the next level (using SceneManagement or any other method you prefer)
    }
}