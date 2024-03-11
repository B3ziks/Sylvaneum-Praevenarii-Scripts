using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] TimerContainer timerContainer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);

        text.text = minutes.ToString() + ":" + seconds.ToString("00");
        string test = minutes.ToString() + ":" + seconds.ToString("00");
        timerContainer.time  = DateTime.Parse(test);
        timerContainer.stime = test;

    }

}
