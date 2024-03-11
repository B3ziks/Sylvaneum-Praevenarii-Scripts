using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure to include the TextMeshPro namespace

public class StatusBar : MonoBehaviour
{
    private Slider hpSlider;
    private Character character;
    [SerializeField] private TextMeshProUGUI healthText; // Reference to the TextMeshProUGUI component

    // Start is called before the first frame update
    void Start()
    {
        hpSlider = GetComponent<Slider>();
        if (hpSlider == null)
        {
            Debug.LogWarning("StatusBar: Missing Slider component!");
            return;
        }

        character = FindObjectOfType<Character>();
        if (character == null)
        {
            Debug.LogWarning("StatusBar: Character not found in the scene!");
            return;

        }

        // Make sure the healthText is assigned
        if (healthText == null)
        {
            Debug.LogWarning("StatusBar: Missing TextMeshPro component!");
            return;

        }

        // Initialize maxValue of Slider to 1 for percentage representation
        if (hpSlider != null && character != null && healthText != null)
        {
            hpSlider.maxValue = 1;
            UpdateHealthBar();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        if (hpSlider != null && healthText != null)
        {
            float currentHealth = character.currentHp;
            float maxHealth = character.maxHp;
            hpSlider.value = currentHealth / maxHealth;

            // Update the TextMeshPro text to display current and max health
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
