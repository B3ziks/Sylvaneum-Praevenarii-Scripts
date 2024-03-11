using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [System.Serializable]
    public class SongSet
    {
        public List<AudioClip> songs;  // List of songs per stage
    }

    public AudioSource audioSource;
    public List<SongSet> songSets;  // List of song sets
    private int currentSongIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        int stageNumber = GetCurrentGameplayStageNumber();
        if (stageNumber != -1) // If a valid GameplayStage number is found
        {
            currentSongIndex = 0;
            PlayMusicForStage(stageNumber);
        }
    }

    int GetCurrentGameplayStageNumber()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            int stageNumber;
            if (int.TryParse(scene.name.Replace("GameplayStage", ""), out stageNumber))
            {
                return stageNumber;
            }
        }
        return -1;  // Returns -1 if no valid GameplayStage is found
    }

    void PlayMusicForStage(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < songSets.Count)
        {
            // Stop the current audio if it's playing.
            audioSource.Stop();

            // Set the clip to the beginning of the desired song.
            audioSource.clip = songSets[stageIndex].songs[currentSongIndex];
            audioSource.time = 0;  // Rewind the song to the start.

            // Start playing with fade in.
            StartCoroutine(PlayWithFadeIn());
        }
    }

    IEnumerator PlayWithFadeIn(float fadeDuration = 1.0f)
    {
        audioSource.volume = 0;
        audioSource.Play();
        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            int currentStage = GetCurrentGameplayStageNumber();
            if (currentStage != -1)
            {
                currentSongIndex = (currentSongIndex + 1) % songSets[currentStage].songs.Count;
                PlayMusicForStage(currentStage);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged; // Unsubscribe to avoid potential issues
    }
    // Add this method to restart the music
    public void RestartMusic()
    {
        int currentStage = GetCurrentGameplayStageNumber();
        if (currentStage != -1)
        {
            currentSongIndex = 0; // Reset the song index
            PlayMusicForStage(currentStage); // Play music from the beginning
        }
    }
}