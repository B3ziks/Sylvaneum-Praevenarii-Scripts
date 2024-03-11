using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
    [SerializeField] GameObject spearObject;

    [SerializeField] Vector2 attackSize = new Vector2(4f, 2f);

    private void Start()
    {
        spearObject.SetActive(false);
    }

    public override void Attack()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetDirection(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        // Position the spear relative to the player and in the direction of the mouse
        Vector3 directionToMouse = (mousePosition - transform.position).normalized;

        // Adjust the offset (you can modify the multiplier for a greater/lesser offset)
        float offset = 3.0f;
        spearObject.transform.position = transform.position + directionToMouse * ((attackSize.x / 2) + offset);

        StartCoroutine(AttackProcess());
    }

    void SetDirection(float dir_x, float dir_y)
    {
        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        spearObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Flip the spear's orientation based on the direction of the mouse relative to the character
        if (dir_x > 0 && spearObject.transform.localScale.x < 0)
        {
            Vector3 scale = spearObject.transform.localScale;
            scale.x *= -1;
            spearObject.transform.localScale = scale;
        }
        else if (dir_x < 0 && spearObject.transform.localScale.x > 0)
        {
            Vector3 scale = spearObject.transform.localScale;
            scale.x *= -1;
            spearObject.transform.localScale = scale;
        }
    }

    IEnumerator AttackProcess()
    {
        for (int i = 0; i < weaponData.stats.numberOfAttacks; i++)
        {
            spearObject.SetActive(true);

            Collider2D[] colliders;
            colliders = Physics2D.OverlapBoxAll(spearObject.transform.position, attackSize, 0f); // You can directly check for overlaps at the spear's position without adjusting it based on direction

            ApplyDamage(colliders);

            spearObject.transform.localScale = new Vector3(Mathf.Abs(spearObject.transform.localScale.x), attackSize.y, spearObject.transform.localScale.z); // Rescale spearObject based on attackSize

            yield return new WaitForSeconds(0.3f);
            spearObject.SetActive(false);
        }
    }
}