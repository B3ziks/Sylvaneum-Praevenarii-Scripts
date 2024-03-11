using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : WeaponBase
{
    // Start is called before the first frame update
   // [SerializeField] float timeToAttack = 4f;
    //float timer;

    [SerializeField] GameObject leftWhipObject;
    [SerializeField] GameObject rightWhipObject;


    [SerializeField] Vector2 attackSize = new Vector2(4f,2f);
    //[SerializeField] int whipDamage = 1; 
    


  

    private void Start()
    {
        leftWhipObject.SetActive(false);
        rightWhipObject.SetActive(false);
        shouldRotateTowardsMouse = false;

    }

    public override void Attack()
    {
        StartCoroutine(AttackProcess());
    }

    IEnumerator AttackProcess()
    {
        for (int i =0; i< weaponData.stats.numberOfAttacks; i++)
        {
            if (playerMove.lastHorizontalCoupledVector > 0)
            {
                rightWhipObject.SetActive(true);
                Collider2D[] colliders = Physics2D.OverlapBoxAll(rightWhipObject.transform.position, attackSize, 0f);
                ApplyDamage(colliders);

            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(leftWhipObject.transform.position, attackSize, 0f);
                ApplyDamage(colliders);
                leftWhipObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
