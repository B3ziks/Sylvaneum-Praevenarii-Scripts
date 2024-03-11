using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockStageCondition : MonoBehaviour
{
    [SerializeField] private DataContainer container;
    [SerializeField] private FlagsTable flagTable;
    [SerializeField] private string unlockFlag;
    public GameWinPanel winPanel;
    private PauseManager pauseManager;

    private static bool bossDefeated = false;
    public static bool BossDefeated
    {
        get { return bossDefeated; }
        private set { bossDefeated = value; }
    }

    private const string MainMenuSceneName = "MainMenu"; // If it's a constant name
    private const string EssentialSceneName = "Essential"; // If it's a constant name


    private bool IsGameScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene != MainMenuSceneName;
    }

   

    private void OnEnable()
    {
        Enemy.BossKilled += OnBossKilled;
    }

    private void OnDisable()
    {
        Enemy.BossKilled -= OnBossKilled;
    }

    private void OnBossKilled()
    {
        UnityEngine.Debug.Log("Boss killed event triggered!");

        BossDefeated = true;

        if (IsGameScene())
        {
            Flag flag = flagTable.GetFlag(unlockFlag);
            if (flag != null)
            {
                flag.state = true;
                container.UpdateCompletedStages(flagTable);
                GameData data = new GameData();
                container.SaveData(ref data);
            }

          

            container.ResetDifficultyToNormal();
        }
    }
}
