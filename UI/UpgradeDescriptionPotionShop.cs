using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeDescriptionPotionShop : MonoBehaviour
{
    public TMP_Text descriptionText;

    public void ShowDescription(string description)
    {
        descriptionText.text = description;
    }

    public void HideDescription()
    {
        descriptionText.text = "Good luck with your next hunt! \n Bring me something to eat when you come back, okay?";
    }
}
