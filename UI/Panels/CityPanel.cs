using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CityPanel : MonoBehaviour
{
    [Header("Building Buttons")]
    [SerializeField] private GameObject BountyGuildButton;
    [SerializeField] private GameObject BlacksmithButton;
    [SerializeField] private GameObject PotionShopButton;

    [Header("Arrow Images")]
    [SerializeField] private GameObject arrowUp;
    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;

    [Header("Text Display")]
    [SerializeField] private TMP_Text infoText;

    private void Start()
    {
        // Ensure all arrows are not visible initially
        arrowUp.SetActive(false);
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);
    }

    // Method called from EventTrigger on hover enter
    public void OnBuildingHoverEnter(GameObject buildingButton)
    {
        if (buildingButton == BountyGuildButton)
        {
            infoText.text = "Go to Bounty Hunter Guild";
            arrowUp.SetActive(true);
            arrowLeft.SetActive(false);
            arrowRight.SetActive(false);
        }
        else if (buildingButton == BlacksmithButton)
        {
            infoText.text = "Go to Blacksmith Forge";
            arrowLeft.SetActive(true);
            arrowUp.SetActive(false);
            arrowRight.SetActive(false);
        }
        else if (buildingButton == PotionShopButton)
        {
            infoText.text = "Go to Potion Shop";
            arrowRight.SetActive(true);
            arrowLeft.SetActive(false);
            arrowUp.SetActive(false);
        }
    }
    // Method called from EventTrigger on hover exit
    public void OnBackButtonEnter()
    {
        infoText.text = "Back To Menu";

    }
    // Method called from EventTrigger on hover exit
    public void OnBuildingHoverExit()
    {
        infoText.text = "";
        arrowUp.SetActive(false);
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);
    }
}