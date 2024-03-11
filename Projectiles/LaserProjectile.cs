using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public LaserWeapon laserWeapon; // Assign this in the inspector

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            laserWeapon.HandleLaserHit(collider);
        }
    }
}
