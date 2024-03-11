using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryPanelHide2 : MonoBehaviour
{
    [SerializeField] SummaryWLContainer summaryWLContainer;
    [SerializeField] SummaryPanelWon summaryPanelWon;



    private void Awake()
    {
        summaryPanelWon = GetComponent<SummaryPanelWon>();

        if (summaryWLContainer.win == true)
        {
            summaryPanelWon.gameObject.SetActive(true);
        }
        else
        {
            summaryPanelWon.gameObject.SetActive(false);
        }

    }


}
