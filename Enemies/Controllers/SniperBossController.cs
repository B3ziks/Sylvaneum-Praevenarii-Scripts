using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SniperBossController : BossController
{
    private Transform sniperRifleTransform; // The transform of the sniper rifle child

    private void Start()
    {
        base.Start();

        // Try to find the sniper child of this boss
        sniperRifleTransform = transform.Find("sniper(Clone)/sniperRifle");
        if (sniperRifleTransform == null)
        {
            UnityEngine.Debug.LogWarning("sniperRifle not found under sniper(Clone) for SniperBossController.");
        }
    }

    protected override void Update()
    {
        base.Update(); // Make sure to call the base update to retain boss behaviors

        // Rotate the sniper rifle towards the player
        PointRifleAtTarget(GetPlayerTransform());
    }

    public void PointRifleAtTarget(Transform target)
    {
        if (sniperRifleTransform == null || target == null) return;

        Vector3 directionToTarget = target.position - sniperRifleTransform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Check if the target is on the left side
        if (target.position.x < sniperRifleTransform.position.x)
        {
            angle += 180f; // Add 180 degrees to the angle
        }

        sniperRifleTransform.rotation = Quaternion.Euler(0, 0, angle);
    }


}