using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterGameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    [SerializeField] GameObject weaponParent;
    [SerializeField] PauseManager pauseManager;

    [Header("Game Over Sound")]
    public AudioClip gameOverSound; // Reference to the audio clip (MP3 or WAV)
    public AudioSource audioSource; // Reference to the audio source component

    private void Start()
    {
       // audioSource = GetComponent<AudioSource>(); // Get the audio source component
        if (audioSource == null)
        {
            // If AudioSource doesn't exist, add one
          //  audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void GameOver()
    {
        UnityEngine.Debug.Log("Game Over");

        // Play the game over sound
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        GetComponent<PlayerMove>().enabled = false;
        pauseManager = FindObjectOfType<PauseManager>();
        gameOverPanel.SetActive(true);
        pauseManager.PauseGame();
        weaponParent.SetActive(false);
    }
}