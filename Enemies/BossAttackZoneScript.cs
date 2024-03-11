using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BossAttackZoneScript : MonoBehaviour
{
    private EnemyBossRat bossScript;

    private void Start()
    {
        // Getting the reference to the parent's script
        bossScript = GetComponentInParent<EnemyBossRat>();
        if (bossScript == null)
        {
            UnityEngine.Debug.LogError("Boss script not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossScript.SetPlayerInZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossScript.SetPlayerInZone(false);
           // bossScript.SetIsAttacking(false);

        }
    }
}
