using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboEncyclopediaPanel : MonoBehaviour
{
    [SerializeField] private ComboTable comboTable;
    [SerializeField] private GameObject comboDisplayPrefab;
    [SerializeField] private Transform comboContainer;
    [SerializeField] private TextMeshProUGUI descriptionPanelText;
    [SerializeField] private DataContainer dataContainer; // Reference to the DataContainer

    private void Start()
    {
        LoadDiscoveredCombos();
        DisplayDiscoveredCombos();
    }

    private void LoadDiscoveredCombos()
    {
        if (dataContainer == null)
        {
            UnityEngine.Debug.LogError("DataContainer is not assigned in ComboEncyclopediaPanel.");
            return;
        }

        foreach (var discoveredCombo in dataContainer.discoveredCombos)
        {
            var comboData = comboTable.combos.Find(c => c.comboType == discoveredCombo.comboType);
            if (comboData != null)
            {
                comboData.isDiscovered = discoveredCombo.isDiscovered;
            }
        }
    }

    private void DisplayDiscoveredCombos()
    {
        foreach (var comboData in comboTable.combos)
        {
            if (comboData.isDiscovered)
            {
                GameObject comboObj = Instantiate(comboDisplayPrefab, comboContainer);
                ComboDisplay display = comboObj.GetComponent<ComboDisplay>();
                if (display != null)
                {
                    display.Setup(comboData);
                    display.SetDescriptionPanelText(descriptionPanelText);
                }
            }
        }
    }
}