using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

public class TotemSpiritCombo : MonoBehaviour
{
    public PoolObjectData spiritPoolData;
    public float circleRadius = 2.0f;
    public float circleSpeed = 5f;
    private PoolManager poolManager;
    [SerializeField] private List<GameObject> totems = new List<GameObject>();
    [SerializeField] private List<GameObject> spirits = new List<GameObject>();
    private PoolMember poolMember;
    private float comboDuration = 5f;
    private float comboCooldown = 10f;
    [SerializeField] private bool isComboActive = false;
    private bool isActive = false;

    private bool isInCooldown = false;
    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }
    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isInCooldown && isComboActive)
        {
            if (!isActive)
            {
                StartCoroutine(ActivateCombo());
            }
            else
            {
                UnityEngine.Debug.Log("Combo is already active.");
            }
        }
    }

    private IEnumerator ActivateCombo()
    {
        UnityEngine.Debug.Log("Activating Combo");
        isActive = true;
        SpawnSpirits();
        yield return new WaitForSeconds(comboDuration);
        DeactivateAllSpirits();
        StartCoroutine(ComboCooldown());
    }


    private void SpawnSpirits()
    {
        if (totems.Count == 0)
        {
            UnityEngine.Debug.Log("Totems list is empty.");
            return;
        }

        foreach (GameObject totem in totems)
        {
            GameObject spirit = poolManager.GetObject(spiritPoolData);
            if (spirit != null)
            {
                spirit.transform.position = totem.transform.position + new Vector3(circleRadius, 0, 0);
                spirit.SetActive(true);
                spirits.Add(spirit);
                StartCoroutine(CircleTotem(spirit, totem.transform));
                UnityEngine.Debug.Log("Spawned spirit at: " + totem.transform.position);
            }
            else
            {
                UnityEngine.Debug.LogError("Unable to spawn spirit from pool.");
            }
        }
    }

    private IEnumerator CircleTotem(GameObject spirit, Transform totem)
    {
        UnityEngine.Debug.Log("Starting CircleTotem coroutine for spirit: " + spirit.name);
        float angle = 0;
        while (isActive)
        {
            Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * circleRadius;
            spirit.transform.position = totem.position + offset;
            angle += circleSpeed * Time.deltaTime;
            if (angle >= 360f) angle -= 360f;
            yield return null;
        }
        spirit.SetActive(false);
        spirits.Remove(spirit);
       UnityEngine.Debug.Log("Deactivated spirit: " + spirit.name);
    }

    private IEnumerator ComboCooldown()
    {
        isInCooldown = true;
        UnityEngine.Debug.Log("Combo is now in cooldown.");
        yield return new WaitForSeconds(comboCooldown);
        isInCooldown = false;
        isActive = false;
        UnityEngine.Debug.Log("Combo cooldown finished. Ready to be activated again.");
    }
    // This method is intended to be called externally to set the combo state.
    public void SetComboActiveState(bool active)
    {
        isComboActive = active; // Set the combo as active/inactive.
    }
    // Method to clear all totems and their effects
    public void ClearTotems()
    {
        foreach (GameObject totem in totems)
        {
            Destroy(totem); // Destroy the totem GameObject
        }
        totems.Clear(); // Clear the list of totems

        DeactivateAllSpirits(); // Deactivate all associated spirits
    }
    private void DeactivateAllSpirits()
    {
        foreach (GameObject spirit in spirits)
        {
            if (spirit != null)
            {
                spirit.SetActive(false); // Deactivate the spirit
                //poolMember.ReturnObject(spirit); // Return the spirit to the pool (if you're using pooling)
            }
        }
        spirits.Clear(); // Clear the list of spirits
    }

    public void AddTotem(GameObject newTotem)
    {
        if (!totems.Contains(newTotem))
        {
            totems.Add(newTotem);
        }
    }

    public void RemoveTotem(GameObject totem)
    {
        totems.Remove(totem);
    }
}