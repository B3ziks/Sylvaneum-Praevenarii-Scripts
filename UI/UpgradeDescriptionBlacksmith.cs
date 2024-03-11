using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeDescriptionBlacksmith : MonoBehaviour
{
    public TMP_Text descriptionText;

    public void ShowDescription(string description)
    {
        descriptionText.text = description;
    }

    public void HideDescription()
    {
        descriptionText.text = "What else do you need? \n If you are not buying, get out.";
    }
}
