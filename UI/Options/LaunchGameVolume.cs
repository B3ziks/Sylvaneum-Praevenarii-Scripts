using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LaunchGameVolume : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private Slider soundSlider;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider musicSlider;

    [SerializeField] OptionsContainer optionsContainer;

    private void Start()
    {
        InitializeAudio(soundSlider, soundSource, optionsContainer.soundVolume);
        InitializeAudio(musicSlider, musicSource, optionsContainer.musicVolume);
    }

    private void Update()
    {
        UpdateAudio(soundSlider, soundSource, optionsContainer.soundVolume);
        UpdateAudio(musicSlider, musicSource, optionsContainer.musicVolume);
    }

    private void InitializeAudio(Slider slider, AudioSource source, float volume)
    {
        slider.value = volume;
        source.volume = volume;
    }

    private void UpdateAudio(Slider slider, AudioSource source, float volume)
    {
        slider.value = volume;
        source.volume = volume;
    }
}