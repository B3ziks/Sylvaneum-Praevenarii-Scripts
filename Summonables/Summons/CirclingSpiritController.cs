using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingSpiritController : MonoBehaviour, IPoolMember
{
    private Transform targetTransform;
    private float radius;
    private float angle;
    public int damage;
    private PoolManager poolManager; // Reference to the pool manager
    private PoolMember poolMember;

    public void Initialize(Transform target, float radius, PoolManager poolManager, float startingAngle)
    {
        this.targetTransform = target;
        this.radius = radius;
        this.angle = startingAngle; // Set the initial angle here
        this.poolManager = poolManager;
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // This will rotate the spirit over time
        angle += Time.deltaTime; // You can adjust the rotation speed by multiplying with a speed factor

        Vector3 positionOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = targetTransform.position + positionOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemyScript = other.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                PostDamage(damage, enemyScript.transform.position); // Calling PostDamage after dealing damage

                // Immediately deactivate this spirit
                DeactivateSpirit();
            }
        }
    }
    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }
    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    // Define a delegate type for the event
    public delegate void SpiritEventHandler(GameObject spirit);

    // Define the OnDeactivate event using the delegate
    public event SpiritEventHandler OnDeactivate;

    // A method that triggers the OnDeactivate event
    protected void TriggerDeactivation()
    {
        if (OnDeactivate != null)
        {
            OnDeactivate(gameObject);
        }
    }

    // Modify the DeactivateSpirit method to call TriggerDeactivation
    private void DeactivateSpirit()
    {
        TriggerDeactivation(); // This will notify any subscribers that the spirit is deactivating
        poolMember.ReturnToPool(); // Return the spirit to the pool
    }

    public void PostDamage(int damage, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition);
    }
}