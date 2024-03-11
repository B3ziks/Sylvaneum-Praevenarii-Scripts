using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTime : MonoBehaviour
{
    public static StageTime instance;

    public float time;
    TimerUI timerUI;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        timerUI = FindObjectOfType<TimerUI>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        timerUI.UpdateTime(time);
    }
    // Add this method to reset the timer
    public void ResetTimer()
    {
        time = 0;
        timerUI.UpdateTime(time);
    }

}
