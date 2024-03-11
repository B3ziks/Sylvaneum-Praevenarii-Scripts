using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurtle : Enemy
{
    [SerializeField] private int maxShieldStacks = 5;
    private int currentShieldStacks;
    private GameObject shieldGameObject; // GameObject for the shield's visual representation

    protected void Start()
    {
        shieldGameObject = transform.Find("TurtleShield")?.gameObject;
        if (shieldGameObject == null)
        {
            Debug.LogError("TurtleShield GameObject not found!");
        }
        else
        {
            shieldGameObject.SetActive(false); // Start with the shield deactivated
        }
    }

    public void ActivateShield()
    {
        if (shieldGameObject != null)
        {
            shieldGameObject.SetActive(true);
            currentShieldStacks = maxShieldStacks;
        }
    }

    public override void TakeDamage(int damage)
    {
        if (currentShieldStacks > 0)
        {
            // Shield absorbs the hit
            currentShieldStacks--;
            if (currentShieldStacks <= 0)
            {
                DeactivateShield();
            }
        }
        else
        {
            // Normal damage processing
            base.TakeDamage(damage);
        }
    }

    private void DeactivateShield()
    {
        if (shieldGameObject != null)
        {
            shieldGameObject.SetActive(false);
        }
    }

    // Other overridden or additional methods...
}