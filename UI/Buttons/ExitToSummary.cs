using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToSummary : MonoBehaviour
{
    [SerializeField] private DataContainer dataContainer;

    public void GoToSummary()
    {
        CharacterIconDisplay.ResetCharacterSelection();
        dataContainer.ResetSelectedCharacter();
        SceneManager.LoadScene("SummaryScreen");


    }



}
