using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryPanelHide : MonoBehaviour
{
    [SerializeField] SummaryWLContainer summaryWLContainer;
    [SerializeField] SummaryPanelLose summaryPanelLose;



    private void Awake()
    {
        summaryPanelLose = GetComponent<SummaryPanelLose>();

        if (summaryWLContainer.win == true)
        {
            summaryPanelLose.gameObject.SetActive(false);
        }
        else
        {
            summaryPanelLose.gameObject.SetActive(true);
        }
    }


}
