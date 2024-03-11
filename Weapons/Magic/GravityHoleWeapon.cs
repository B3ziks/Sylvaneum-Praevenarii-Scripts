using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHoleWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData gravityHoleData;

    public override void Attack()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-coordinate is consistent

        GameObject gravityHole = poolManager.GetObject(gravityHoleData);
        if (gravityHole != null)
        {
            gravityHole.transform.position = mousePosition;
            gravityHole.SetActive(true);
        }
        else
        {
            Debug.LogError("Gravity Hole object not available in pool!");
        }

        // Reset the timer
        timer = weaponData.stats.timeToAttack;
    }
}