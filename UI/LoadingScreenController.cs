using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private GameObject AnimationImage; // Animator for the bat animation.

    public void ShowLoadingScreen()
    {
        loadingScreenPanel.SetActive(true);
        AnimationImage.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        loadingScreenPanel.SetActive(false);
    }
}