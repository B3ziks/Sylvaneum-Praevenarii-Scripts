using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EngineerMechaCombo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool isComboActive = false;

    private bool isMechaActive;
    private bool isInCooldown = false;
    [SerializeField] private float mechaDuration = 5f;
    [SerializeField] private float mechaCooldown = 10f;
    [SerializeField] private MechaWeapon mechaWeapon; // The MechaWeapon script attached to the child GameObject

    private Character playerCharacter;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerCharacter = GetComponent<Character>();

        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator not found on the child object.");
        }
        if (playerCharacter == null)
        {
            UnityEngine.Debug.LogError("Character script not found on this object.");
        }
        if (mechaWeapon == null) UnityEngine.Debug.LogError("MechaWeapon script not found on the child object.");

    }
    private void Awake()
    {

    }
    private void Update()
    {
        if (isComboActive && Input.GetKeyDown(KeyCode.R) && !isMechaActive && !isInCooldown)
        {
            StartCoroutine(ActivateMechaForDuration());
        }
    }

    public void SetComboActiveState(bool state)
    {
        isComboActive = state;
    }

    private IEnumerator ActivateMechaForDuration()
    {
        ActivateMecha();
        yield return new WaitForSeconds(mechaDuration);
        DeactivateMecha();
        isInCooldown = true;
        yield return new WaitForSeconds(mechaCooldown);
        isInCooldown = false;
    }

    private void ActivateMecha()
    {
        if (animator != null)
        {
            animator.SetBool("isMecha", true);
            isMechaActive = true;
            mechaWeapon.EnableWeapon(); // This function should enable the mecha's weapon system

        }
        else
        {
            UnityEngine.Debug.LogError("Animator is null. Cannot activate Mecha.");
        }
    }

    private void DeactivateMecha()
    {
        if (animator != null)
        {
            animator.SetBool("isMecha", false);
            isMechaActive = false;
            mechaWeapon.DisableWeapon(); // This function should disable the mecha's weapon system

        }
        else
        {
            UnityEngine.Debug.LogError("Animator is null. Cannot deactivate Mecha.");
        }
    }
}
