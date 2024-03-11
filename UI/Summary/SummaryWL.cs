using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryWL : MonoBehaviour
{

    [SerializeField] SummaryWLContainer summaryWLContainer;

  

    public void ChangeScenePanel()
    {
        summaryWLContainer.win = false;
    }
}
