using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletCollisionHandler : MonoBehaviour
{
    [SerializeField] private int minDamage = 50;
    [SerializeField] private int maxDamage = 150;
    [SerializeField] private float chargeTime = 0f; // You'll need to update this from the MechaGauntletWeapon script
    private MechaGauntletWeapon gauntletWeapon; // Reference to the weapon script

    private void Awake()
    {
        // Find the MechaGauntletWeapon component on the parent GameObject
        gauntletWeapon = GetComponentInParent<MechaGauntletWeapon>();
        if (!gauntletWeapon)
        {
            Debug.LogError("MechaGauntletWeapon component not found on parent!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable enemy = other.GetComponent<IDamageable>();
            if (enemy != null && gauntletWeapon)
            {
                // Calculate damage based on the weapon's data
                int damage = Mathf.CeilToInt(Mathf.Lerp(gauntletWeapon.weaponData.stats.damage, gauntletWeapon.weaponData.stats.damage * 3, gauntletWeapon.ChargeFraction));
                enemy.TakeDamage(damage);

                // Post damage message
                PostDamage(damage, other.transform.position);
            }
        }
    }

    private void PostDamage(int damage, Vector3 targetPosition)
    {
        // Use the MechaGauntletWeapon's method to get the message color based on element type
        Color messageColor = gauntletWeapon.GetMessageColor(gauntletWeapon.weaponData.stats.elementType);
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }

    // Call this method from the MechaGauntletWeapon script when charging starts
    public void UpdateChargeTime(float newChargeTime)
    {
        chargeTime = newChargeTime;
    }
}