using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlamDamage : MonoBehaviour
{
    public int damage = 0;
    public float effectRadius = 5f;
    public float damageCooldown = 0.5f;  // Duration in seconds before the same player can take damage again.
    private bool isCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCooldown)
        {
            Character target = other.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(damage);
                StartCoroutine(StartCooldown());
            }
        }
    }
    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(damageCooldown);
        isCooldown = false;
    }

}