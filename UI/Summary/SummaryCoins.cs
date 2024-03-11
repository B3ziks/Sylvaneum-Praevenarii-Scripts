using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SummaryCoins : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] StageCoinsContainer stageCoinsContainer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = stageCoinsContainer.coins.ToString();
    }
}
