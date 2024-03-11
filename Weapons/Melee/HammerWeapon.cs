using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HammerWeapon : WeaponBase
{
    [SerializeField] GameObject groundHoleEffect; // Prefab for the ground hole effect. It should be set as a child of the weapon in the scene.

    [SerializeField] float attackRadius = 2f; // Area of effect as a circle
    [SerializeField] float effectDistanceFromCharacter = 2.0f; // This determines how far from the character the effect will appear.

    public override void Attack()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        Vector3 effectPosition = transform.position + directionToMouse * effectDistanceFromCharacter;

        // Set the rotation of the groundHoleEffect here, right before activation
        SetEffectRotation(mouseWorldPosition);

        StartCoroutine(AttackProcess(effectPosition));
    }

    private void SetEffectRotation(Vector3 mouseWorldPosition)
    {
        // Calculate the angle between the effect and the mouse
        Vector2 direction = mouseWorldPosition - groundHoleEffect.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        groundHoleEffect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator AttackProcess(Vector3 effectPosition)
    {
        groundHoleEffect.transform.position = effectPosition;
        groundHoleEffect.transform.localScale = new Vector3(attackRadius, attackRadius, 1f);

        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(effectPosition, attackRadius);

        ApplyDamage(colliders);

      //  UnityEngine.Debug.Log("Setting groundHoleEffect to Active");
        groundHoleEffect.SetActive(true);

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);

       // UnityEngine.Debug.Log("Setting groundHoleEffect to Inactive");
    }
}
