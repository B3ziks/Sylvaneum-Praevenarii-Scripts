using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourDifferentElementCombo : MonoBehaviour
{
    [Header("Combo Prefabs")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject earthPrefab;

    [Header("Combo Settings")]
    [SerializeField] private float comboDuration = 5f;
    [SerializeField] private float comboCooldown = 10f;
    [SerializeField] private float effectDistanceFromPlayer = 3f; // The distance each effect will spawn from the player.
    
    private bool isSkillActive = false;
    private bool isComboActive = false;
    private bool isInCooldown = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isInCooldown && isComboActive)
        {
            StartCoroutine(ActivateFourElementCombo());
        }
    }

    private IEnumerator ActivateFourElementCombo()
    {
        isSkillActive = true;
        isInCooldown = true;

        // Instantiate the prefabs and store their references
        GameObject fireEffect = Instantiate(firePrefab, transform.position, Quaternion.identity);
        GameObject iceEffect = Instantiate(icePrefab, transform.position, Quaternion.identity);
        GameObject lightningEffect = Instantiate(lightningPrefab, transform.position, Quaternion.identity);
        GameObject earthEffect = Instantiate(earthPrefab, transform.position, Quaternion.identity);

        // Store the start time and calculate the end time
        float startTime = Time.time;
        float endTime = startTime + comboDuration;

        while (Time.time < endTime)
        {
            // Continuously update the positions relative to the player
            Vector3 topPosition = transform.position + new Vector3(0, effectDistanceFromPlayer, 0);
            Vector3 bottomPosition = transform.position + new Vector3(0, -effectDistanceFromPlayer, 0);
            Vector3 leftPosition = transform.position + new Vector3(-effectDistanceFromPlayer, 0, 0);
            Vector3 rightPosition = transform.position + new Vector3(effectDistanceFromPlayer, 0, 0);

            fireEffect.transform.position = topPosition;
            iceEffect.transform.position = bottomPosition;
            lightningEffect.transform.position = leftPosition;
            earthEffect.transform.position = rightPosition;

            yield return null; // Wait until the next frame
        }

        // Deactivate the effects after the duration
        fireEffect.SetActive(false);
        iceEffect.SetActive(false);
        lightningEffect.SetActive(false);
        earthEffect.SetActive(false);

        isSkillActive = false;

        // Start the cooldown
        yield return new WaitForSeconds(comboCooldown);
        isInCooldown = false;
    }
    public void SetComboActiveState(bool state)
    {
        isComboActive = state;
    }
}