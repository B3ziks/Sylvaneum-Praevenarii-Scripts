using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
    [SerializeField] private DataContainer dataContainer;

    public void BackToMenu()
    {
        CharacterIconDisplay.ResetCharacterSelection();
        if (dataContainer != null)
        {
            dataContainer.ResetSelectedCharacter();
        }

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
        }
        dataContainer.ResetDifficultyToNormal();
        SceneManager.LoadScene("Town");

        Coins coinsComponent = GetComponent<Coins>();
        if (coinsComponent != null)
        {
            coinsComponent.ResetStageCoins();
        }
    }
}
