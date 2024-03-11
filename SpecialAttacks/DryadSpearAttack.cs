using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Dryad Spear Attack")]
public class DryadSpearAttack : SpecialAttack
{
    public PoolObjectData spearObjectData;
    public float spearSpeed;
    public float knockbackForce;
    public float knockbackTimeWeight;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            return;
        }

        GameObject spear = poolManager.GetObject(spearObjectData);
        if (spear == null)
        {
            Debug.LogError("Spear object not available in pool!");
            return;
        }

        // Set spear position and rotation
        spear.transform.position = executor.transform.position;
        spear.transform.rotation = executor.transform.rotation;
        spear.SetActive(true);

        // Set spear movement and rotation
        Rigidbody2D spearRigidbody = spear.GetComponent<Rigidbody2D>();
        spearRigidbody.velocity = executor.transform.up * spearSpeed;

        DryadSpear spearScript = spear.GetComponent<DryadSpear>();
        if (spearScript != null)
        {
            spearScript.SetKnockbackParameters(knockbackForce, knockbackTimeWeight);
        }
    }
}

