using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectStageButton : MonoBehaviour
{
    public StageData stageData;
    public GameObject comingSoonImage; // Reference to the Coming Soon image
    public Button playButton; // Reference to the Play button

    private void Start()
    {
        // Ensure that the method is linked to the UI Button’s onClick event.
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if (stageData.isComingSoon)
        {
            // If the stage is marked as "Coming Soon"
            comingSoonImage.SetActive(true); // Show the Coming Soon image
            playButton.interactable = false; // Disable the Play button
            LevelSelectPanel.Instance.StageButtonClick(stageData);

        }
        else
        {
            // If the stage is available, proceed with the button click action
            comingSoonImage.SetActive(false); // Show the Coming Soon image
            playButton.interactable = true; // Disable the Play button
            LevelSelectPanel.Instance.StageButtonClick(stageData);

        }
    }
}