using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SummaryTime : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] TimerContainer timerContainer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() { 
    text.text=timerContainer.stime;
    }
}
