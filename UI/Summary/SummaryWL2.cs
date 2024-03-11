using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryWL2 : MonoBehaviour
{
    [SerializeField] SummaryWLContainer summaryWLContainer;



    public void ChangeScenePanel()
    {
        summaryWLContainer.win = true;
    }
}
