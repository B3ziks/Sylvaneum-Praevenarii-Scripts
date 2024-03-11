 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlickWeapon : WeaponBase
{
    [SerializeField] GameObject NuclearArea;
    public float attackAreaSize = 3f;

      void Start () {

        shouldRotateTowardsMouse = false;

        //nuclearArea = GetComponent<NuclearArea>();
        NuclearArea.SetActive (false);
     }

    public override void Attack()
    {

        NuclearArea.SetActive(true);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackAreaSize);
        ApplyDamage(colliders);
    }
}
