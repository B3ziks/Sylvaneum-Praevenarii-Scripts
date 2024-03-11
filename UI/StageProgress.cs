using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StageProgress : MonoBehaviour
{
    StageTime stageTime;
    private float progressUpdateTimer;
    [SerializeField] float progressUpdateTime = 30f; // Time interval for progress update
    [SerializeField] float progressIncrement = 0.2f; // Increment amount for each progress update

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
        progressUpdateTimer = 0f;
    }

    public float Progress { get; private set; } = 1f; // Starting at 1

    private void Update()
    {
        progressUpdateTimer += Time.deltaTime;

        if (progressUpdateTimer >= progressUpdateTime)
        {
            Progress += progressIncrement;
            progressUpdateTimer = 0f; // Reset the timer

            UnityEngine.Debug.Log($"Progress updated: {Progress}");
        }
    }
    public void ResetProgress()
    {
        Progress = 1f; // Reset progress to starting value
        progressUpdateTimer = 0f; // Reset the timer
        UnityEngine.Debug.Log("StageProgress Reset to 1");

    }
}