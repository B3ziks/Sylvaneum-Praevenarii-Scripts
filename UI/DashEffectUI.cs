using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashEffectUI : MonoBehaviour
{
    // Reference to your number Text object within the prefab
    [SerializeField] private TextMeshProUGUI numberText;

    public void SetDashCount(int count)
    {
        if (numberText != null)
        {
            numberText.text = count.ToString();
        }

        // Additional logic to visually represent dash availability can go here
    }
}