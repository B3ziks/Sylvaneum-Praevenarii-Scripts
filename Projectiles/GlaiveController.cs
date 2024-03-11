using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlaiveController : MonoBehaviour
{
    private Transform targetTransform;
    private float radius;
    private float angle;
    public int damage;

    private float lastAttackTime; // To store the time of the last attack
    public float attackCooldown = 0.3f; // Cooldown duration between attacks

    public void Initialize(Transform target, float radius)
    {
        this.targetTransform = target;
        this.radius = radius;

        // Calculate the initial angle based on the glaive's position relative to the player
        Vector3 relativePos = transform.position - targetTransform.position;
        angle = Mathf.Atan2(relativePos.y, relativePos.x);
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // Increase the angle to make the glaive move in a circle around the player
        angle += Time.deltaTime;

        Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + new Vector3(targetTransform.position.x, targetTransform.position.y, 0);
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check if enough time has passed since the last attack
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Enemy enemyScript = other.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(damage);
                    PostDamage(damage, enemyScript.transform.position); // Calling PostDamage after dealing damage

                    lastAttackTime = Time.time; // Update the last attack time
                }
            }
        }
    }

    public void PostDamage(int damage, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition);
    }
}