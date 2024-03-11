using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PermanentUpgradeDescriptionManager : MonoBehaviour
{
    public static PermanentUpgradeDescriptionManager instance;

    public TMP_Text descriptionTextBlacksmith;
    public TMP_Text descriptionTextPotionShop;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ShowDescription(string description)
    {
        if (descriptionTextBlacksmith.gameObject.activeInHierarchy)
        {
            descriptionTextBlacksmith.text = description;
        }
        else if (descriptionTextPotionShop.gameObject.activeInHierarchy)
        {
            descriptionTextPotionShop.text = description;
        }
    }

    public void HideDescription()
    {
        // Check which text object should be active and set the default message accordingly
        if (descriptionTextBlacksmith.gameObject.activeInHierarchy)
        {
            descriptionTextBlacksmith.text = "What else do you need? \n If you are not buying, get out.";
        }
        else if (descriptionTextPotionShop.gameObject.activeInHierarchy)
        {
            descriptionTextPotionShop.text = "Good luck with your next hunt! \n Bring me something to eat when you come back, okay?";
        }
    }
}