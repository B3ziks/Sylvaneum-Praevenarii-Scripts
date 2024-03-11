using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryReset : MonoBehaviour
{
    [SerializeField] StageCoinsContainer stageCoinsContainer;

    public void UpdateCoins()
    {
        stageCoinsContainer.coins = 0;
    }
}
