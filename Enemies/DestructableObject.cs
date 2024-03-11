using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamageable
{
    public void Stun(float stun)
    {
        // currently does nothing
    }

    public void Knockback(Vector3 vector, float force, float timeWeight)
    {
        // currently does nothing
    }

    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
        GetComponent<DropOnDestroy>().CheckDrop();
    }

    public void ApplyElementalEffect(ElementType type, float potency,int damageOverTime)
    {

    }


}