using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSmoke : MonoBehaviour
{
    private WeaponBase weaponReference;

    public void SetWeaponReference(WeaponBase weapon)
    {
        weaponReference = weapon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && weaponReference != null)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Here we are directly using the weaponReference to get the damage and element type
                int damage = weaponReference.weaponData.stats.damage;
                ElementType elementType = weaponReference.weaponData.stats.elementType;

                // Call a method to handle the damage display
                PostDamage(damage, elementType, enemy.transform.position);

                // Then apply the damage to the enemy
                enemy.TakeDamage(damage);
            }
        }
    }

    // This method posts the damage message with the appropriate color
    public void PostDamage(int damage, ElementType elementType, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(elementType);
        // Replace MessageSystem with your actual message system class
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }

    // This method returns the color based on the element type
    private Color GetMessageColor(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return new Color(1f, 0.8f, 0f, 1f); // Brighter orange
            case ElementType.Poison:
                return Color.green;
            case ElementType.Ice:
                return Color.cyan;
            case ElementType.Lightning:
                return Color.yellow;
            default:
                return new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray for default
        }
    }
}