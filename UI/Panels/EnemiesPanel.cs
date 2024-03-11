using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemiesPanel : MonoBehaviour
{
    [Header("Tables")]
    [SerializeField] private EnemiesTable enemiesTable;

    [Header("UI Elements")]
    [SerializeField] private GameObject enemyDisplayPrefab;
    [SerializeField] private Transform normalEnemiesContainer;
    [SerializeField] private Transform eliteEnemiesContainer;
    [SerializeField] private Transform bossEnemiesContainer;
    [SerializeField] private GameObject enemyDetailsPanel; // New panel for enemy details
    [SerializeField] private TextMeshProUGUI detailsNameText;
    [SerializeField] private TextMeshProUGUI detailsLocationText;
    [SerializeField] private Image detailsCreatureImage;
    [SerializeField] private Image detailsLocationImage;
    [SerializeField] private TextMeshProUGUI detailsDescriptionText;
    [SerializeField] private TextMeshProUGUI detailsKillCountText;
    //
    [SerializeField] private TextMeshProUGUI titleEnemiesTypeText;

    [Header("Buttons")]
    [SerializeField] private Button normalButton;
    [SerializeField] private Button eliteButton;
    [SerializeField] private Button eliteButtonRight;
    [SerializeField] private Button bossesButton;

    [Header("Defeat Data")]
    [SerializeField] private DefeatedEnemiesData defeatedEnemiesData;

    private void Start()
    {
        PopulateEnemies();

        normalButton.onClick.AddListener(() => DisplayEnemies(normalEnemiesContainer, "Normal"));
        eliteButton.onClick.AddListener(() => DisplayEnemies(eliteEnemiesContainer, "Elite"));
        eliteButtonRight.onClick.AddListener(() => DisplayEnemies(eliteEnemiesContainer, "Elite"));
        bossesButton.onClick.AddListener(() => DisplayEnemies(bossEnemiesContainer, "Boss"));

        DisplayEnemies(normalEnemiesContainer, "Normal");
    }

    private void PopulateEnemies()
    {
        foreach (var enemy in enemiesTable.enemies)
        {
            GameObject enemyObj = Instantiate(enemyDisplayPrefab);

            EnemyDisplay display = enemyObj.GetComponent<EnemyDisplay>();
            if (display != null)
            {
                display.Setup(enemy, defeatedEnemiesData);
                display.SetDetailsButtonListener(() => OpenDetailsPanel(enemy));
            }

            if (enemy.isBoss)
            {
                enemyObj.transform.SetParent(bossEnemiesContainer);
            }
            else if (enemy.isElite)
            {
                enemyObj.transform.SetParent(eliteEnemiesContainer);
            }
            else
            {
                enemyObj.transform.SetParent(normalEnemiesContainer);
            }
        }
    }

    public void OpenDetailsPanel(EnemyData enemy)
    {
        detailsNameText.text = enemy.Name;
        detailsCreatureImage.sprite = enemy.image;
        detailsLocationImage.sprite = enemy.enemyLocation.locationImage;
        detailsLocationText.text = enemy.enemyLocation.locationName;
        detailsDescriptionText.text = enemy.description;

        // Fetch and display the kill count
        int killCount = GetKillCountForEnemy(enemy.enemyId);
        detailsKillCountText.text = $"Caught: {killCount} times";

        enemyDetailsPanel.SetActive(true);
    }

    private int GetKillCountForEnemy(int enemyId)
    {
        if (defeatedEnemiesData.enemyDefeatCounts.TryGetValue(enemyId, out var count))
        {
            return count;
        }
        return 0; // Return 0 if no data is found for this enemy
    }

    private void DisplayEnemies(Transform enemiesContainer, string titleText)
    {
        normalEnemiesContainer.gameObject.SetActive(false);
        eliteEnemiesContainer.gameObject.SetActive(false);
        bossEnemiesContainer.gameObject.SetActive(false);

        enemiesContainer.gameObject.SetActive(true);
        titleEnemiesTypeText.text = titleText;
    }
}