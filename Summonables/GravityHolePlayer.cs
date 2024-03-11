using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHolePlayer : MonoBehaviour
{
    public float pullForce = 5f;
    public float pullRadius = 3f;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy")) // Ensure this matches your enemy tag
        {
            Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 forceDirection = (transform.position - collider.transform.position).normalized;
                enemyRb.AddForce(forceDirection * pullForce);
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateAfterDuration());
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(5f); // Adjust duration as needed
        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
