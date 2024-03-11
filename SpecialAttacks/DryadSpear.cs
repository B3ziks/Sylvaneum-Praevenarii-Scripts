using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DryadSpear : MonoBehaviour
{
    private float knockbackForce;
    private float knockbackTimeWeight;

    public void SetKnockbackParameters(float force, float timeWeight)
    {
        knockbackForce = force;
        knockbackTimeWeight = timeWeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMove playerMove = collision.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerMove.Knockback(knockbackDirection, knockbackForce, knockbackTimeWeight);
            }

            // Deactivate or return spear to the pool
            gameObject.SetActive(false);
        }
    }
}