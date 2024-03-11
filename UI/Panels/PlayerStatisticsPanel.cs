using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatisticsPanel : MonoBehaviour
{
    [SerializeField] private DefeatedEnemiesData defeatedEnemiesData;
    [SerializeField] private DataContainer dataContainer;
    [SerializeField] private TextMeshProUGUI favoriteCharacterText;
    [SerializeField] private TextMeshProUGUI totalEnemiesText;
    [SerializeField] private PlayerStatisticsManager statisticsManager;

    private void OnEnable()
    {
        UpdateStatisticsDisplay();
    }

    private void UpdateStatisticsDisplay()
    {
        // Update favorite character display
        favoriteCharacterText.text = $"{dataContainer.GetMostChosenCharacter()}";

        // Update total defeated enemies display
        totalEnemiesText.text = $"{statisticsManager.GetTotalDefeatedEnemies()}";
    }


}