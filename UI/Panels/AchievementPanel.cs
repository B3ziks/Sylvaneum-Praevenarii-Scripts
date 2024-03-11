using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPanel : MonoBehaviour
{
    [SerializeField] private AchievementsTable achievementsTable;
    [SerializeField] private GameObject achievementDisplayPrefab;
    [SerializeField] private Transform achievementsContainer;
    [SerializeField] private TextMeshProUGUI descriptionPanelText;  // reference to the global description panel

    private void Start()
    {
        foreach (var achievement in achievementsTable.achievements)
        {
            GameObject achievementObj = Instantiate(achievementDisplayPrefab, achievementsContainer);
            AchievementDisplay display = achievementObj.GetComponent<AchievementDisplay>();
            if (display != null)
            {
                display.Setup(achievement);
                display.SetDescriptionPanelText(descriptionPanelText);  // pass the reference
            }
        }
    }
}