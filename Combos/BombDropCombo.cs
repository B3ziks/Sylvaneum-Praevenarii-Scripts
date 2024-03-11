using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BombDropCombo : MonoBehaviour
{
    public PoolObjectData bombPoolData; // Assign this with the prefab data in the inspector
    private PoolManager poolManager;

    private bool isExplosiveComboActivated = false; // This variable controls the activation of the combo
    [SerializeField]
    private KeyCode comboKey = KeyCode.R; // The key to activate the bomb drop

    private Camera mainCamera;
    private float lastBombTime = 0f; // Time since the last bomb was dropped
    public float bombCooldown = 10f; // Cooldown duration in seconds

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>(); // Find the PoolManager in the scene
        mainCamera = Camera.main; // Cache the main camera
    }

    private void Update()
    {
        // Check if the combo is active and the combo key is pressed and cooldown has passed
        if (isExplosiveComboActivated && Input.GetKeyDown(comboKey) && Time.time - lastBombTime >= bombCooldown)
        {
            DropBomb();
            lastBombTime = Time.time; // Reset the cooldown timer
        }
    }

    private void DropBomb()
    {

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 spawnPosition = new Vector2(mousePosition.x, mainCamera.transform.position.y); // Start from above the mouse position

        // Get a bomb from the pool and place it at the spawn position
        GameObject bomb = poolManager.GetObject(bombPoolData);
        if (bomb != null)
        {
            bomb.transform.position = spawnPosition;
            bomb.SetActive(true);

            // Bomb-specific falling and explosion logic would go here or in a separate script on the bomb prefab
        }
        else
        {
            UnityEngine.Debug.LogError("BombDropCombo: Failed to get bomb object from pool.");
        }
    }

    // Call this method from your ComboManager when the appropriate combo is activated
    public void SetExplosiveComboActive(bool isActive)
    {
        isExplosiveComboActivated = isActive;
    }
}