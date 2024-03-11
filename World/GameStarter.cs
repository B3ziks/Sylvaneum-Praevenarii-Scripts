using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private DataContainer dataContainer; // Reference to DataContainer
    [SerializeField] private GameObject loadingScreenPanel; // Reference to the loading screen panel
   // [SerializeField] private GameObject animationImage; // Reference to the animation image
    [SerializeField] private Slider progressBar; // Reference to the progress bar slider
    [SerializeField] private TextMeshProUGUI loadingText; // Reference to a Text component to display loading progress

    public void StartGame()
    {
        loadingScreenPanel.SetActive(true);
        //animationImage.SetActive(true);

        if (LevelSelectPanel.Instance.SelectedStageData != null && dataContainer.selectedCharacter != null)
        {
            StartCoroutine(LoadGameScenes());
        }
        else
        {
            Debug.LogWarning("No stage or character selected!");
            // Hide loading elements if the game cannot start
            loadingScreenPanel.SetActive(false);
           // animationImage.SetActive(false);
        }
    }

    private IEnumerator LoadGameScenes()
    {
        // Update character selection count
        dataContainer.IncrementCharacterSelection(dataContainer.selectedCharacter.Name, 1);

        // Load essential scene asynchronously
        yield return StartCoroutine(LoadSceneAsync("Essential"));

        // Load selected stage asynchronously
        yield return StartCoroutine(LoadSceneAsync(LevelSelectPanel.Instance.SelectedStageData.stageID));


        // Optionally, unload the main menu scene
        yield return StartCoroutine(UnloadSceneAsync("Town"));

        // Generate the map
        MapGenerator mapGenerator = new GameObject("MapGenerator").AddComponent<MapGenerator>();
        mapGenerator.SetGameplayStageData(LevelSelectPanel.Instance.SelectedStageData.gameplayData);
        mapGenerator.GenerateMap();

        // Hide loading elements after loading is done
        loadingScreenPanel.SetActive(false);
       // animationImage.SetActive(false);
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        UnityEngine.AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully unloads
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        UnityEngine.AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            // Calculate progress as a value between 0 and 1
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Update the progress bar
            progressBar.value = progress;

            // Update loading text
            loadingText.text = "" + (progress * 100f).ToString("F0") + "%";

            yield return null;
        }
    }
}