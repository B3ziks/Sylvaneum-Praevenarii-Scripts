using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Need this for UI components

public class CharacterSelectionPanel : MonoBehaviour
{
    public CharactersTable charactersTable;
    public GameObject characterIconPrefab;
    public Transform charactersPanel;
    public DataContainer dataContainer; // Reference to your DataContainer
    public CharacterDetailsPanel characterDetailsPanelPrefab; // Reference to the CharacterDetailsPanel prefab

    private void Start()
    {
        foreach (var character in charactersTable.characters)
        {
            GameObject charIcon = Instantiate(characterIconPrefab, charactersPanel);
            CharacterIconDisplay display = charIcon.GetComponent<CharacterIconDisplay>();
            if (display != null)
            {
                display.Setup(character);
            }
        }
    }
      private void OnDisable()
    {
        CharacterIconDisplay.ResetCharacterSelection();
    }
  
}