using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Globalization;


public class AudioSettingsController : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioMixer soundMixer;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private TextMeshProUGUI soundValueText;

    [Header("Music")]
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private TextMeshProUGUI musicValueText;

    [SerializeField] private AudioMixMode MixMode;
    [SerializeField] OptionsContainer optionsContainer;
    [SerializeField] private AudioClip soundFeedbackClip; // Assign this in the inspector
    private Coroutine soundTestCoroutine;
    private bool isPanelInteracted = false; // Flag to track user interaction

    private GameSettingsManager gameSettingsManager;

    private void Awake()
    {
        isPanelInteracted = false; // Initialize the flag

        gameSettingsManager = FindObjectOfType<GameSettingsManager>();
        if (!gameSettingsManager)
        {
            Debug.LogError("No GameSettingsManager found in the scene!");
        }

        // Loading will now be handled by GameSettingsManager
        ApplyLoadedAudioSettings();
    }

    public void OnChangeSoundSlider(float value)
    {
        optionsContainer.soundVolume = value;
        UpdateAudio(soundValueText, soundSource, soundMixer, value, "SoundVolume");

        if (isPanelInteracted) // Check if the panel has been interacted with
        {
            if (soundTestCoroutine != null)
            {
                StopCoroutine(soundTestCoroutine);
            }
            soundTestCoroutine = StartCoroutine(PlaySoundAfterDelay());
        }
        else
        {
            // The panel has not been interacted with, so set the flag to true
            isPanelInteracted = true;
        }
    }

    private IEnumerator PlaySoundAfterDelay()
    {
        // Wait for a short delay to see if the user has stopped adjusting the slider
        yield return new WaitForSeconds(0.25f);

        if (soundSource != null && soundFeedbackClip != null && isPanelInteracted)
        {
            soundSource.PlayOneShot(soundFeedbackClip); // Play the sound effect
        }
    }

    public void OnChangeMusicSlider(float Value)
    {
        optionsContainer.musicVolume = Value;
        UpdateAudio(musicValueText, musicSource, musicMixer, Value, "MusicVolume");
    }
    // Call this method when the options panel is opened
    public void OnOpenOptionsPanel()
    {
        isPanelInteracted = false; // Reset the flag when the panel is opened
    }
    private void UpdateAudio(TextMeshProUGUI text, AudioSource source, AudioMixer mixer, float Value, string parameterName)
    {
        NumberFormatInfo percentageFormat = new NumberFormatInfo { PercentPositivePattern = 1, PercentNegativePattern = 1 };
        text.SetText($"{Value.ToString("P2", percentageFormat)}");

        switch (MixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                source.volume = Value;
                break;
            case AudioMixMode.LinearMixerVolume:
                mixer.SetFloat(parameterName, (-80 + Value * 100));
                break;
            case AudioMixMode.LogrithmicMixerVolume:
                mixer.SetFloat(parameterName, Mathf.Log10(Value) * 20);
                break;
        }
    }

    // This function will apply the audio settings from the OptionsContainer
    public void ApplyLoadedAudioSettings()
    {
        OnChangeSoundSlider(optionsContainer.soundVolume);
        OnChangeMusicSlider(optionsContainer.musicVolume);
    }

    public enum AudioMixMode
    {
        LinearAudioSourceVolume,
        LinearMixerVolume,
        LogrithmicMixerVolume
    }
}