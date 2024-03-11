using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinPanel : MonoBehaviour
{
    public static GameWinPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false); // Hide the panel right after setting the instance
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}