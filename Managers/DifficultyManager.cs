using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DifficultyManager : MonoBehaviour
{
    public DataContainer dataContainer;
    public Button normalButton;
    public Button hardButton;
    public Button insaneButton;
    private Difficulty currentDifficulty = Difficulty.Normal;
    public static DifficultyManager Instance;
    public UnityEvent OnDifficultyChanged = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        normalButton.onClick.AddListener(() => SetDifficulty(Difficulty.Normal));
        hardButton.onClick.AddListener(() => SetDifficulty(Difficulty.Hard));
        insaneButton.onClick.AddListener(() => SetDifficulty(Difficulty.Insane));
    }

    private void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
        dataContainer.SetDifficulty(newDifficulty);
        UnityEngine.Debug.Log("Difficulty Set to: " + newDifficulty.ToString());
        OnDifficultyChanged.Invoke();
    }

    public Difficulty GetCurrentDifficulty() // Add this method
    {
        UnityEngine.Debug.Log("Getting current difficulty: " + currentDifficulty.ToString());
        return currentDifficulty;
    }

    public void ResetDifficulty()
    {
        SetDifficulty(Difficulty.Normal);
    }
   
}
