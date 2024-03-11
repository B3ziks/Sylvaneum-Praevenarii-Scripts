using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Required for the IPointerEnterHandler and IPointerExitHandler interfaces

public class EnemyDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private GameObject stampImage; // GameObject for the stamp image

    private EnemyData enemyData;
    private TextMeshProUGUI descriptionPanelText;

    public void Setup(EnemyData enemyData, DefeatedEnemiesData defeatedEnemiesData)
    {
        this.enemyData = enemyData;
        enemyImage.sprite = enemyData.image;
        if (enemyNameText != null)
        {
            enemyNameText.text = enemyData.Name;
        }

        // Check if the enemy has been defeated and show the stamp if so
        if (defeatedEnemiesData.GetDefeatCount(enemyData.enemyId) > 0)
        {
            stampImage.SetActive(true);
        }
        else
        {
            stampImage.SetActive(false);
        }
    }

    public void SetDescriptionPanelText(TextMeshProUGUI descriptionPanelText)
    {
        this.descriptionPanelText = descriptionPanelText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanelText != null && enemyData != null)
        {
            descriptionPanelText.text = enemyData.description;
        }
    }
    public void SetDetailsButtonListener(UnityEngine.Events.UnityAction action)
    {
        Button detailsButton = GetComponentInChildren<Button>(); // Assuming there's only one button
        if (detailsButton != null)
        {
            detailsButton.onClick.AddListener(action);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanelText != null)
        {
            descriptionPanelText.text = "";
        }
    }
}