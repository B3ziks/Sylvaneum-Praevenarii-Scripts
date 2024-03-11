using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);

    void Stun(float stun);

    void Knockback(Vector3 vector, float force, float timeWeight);

    void ApplyElementalEffect(ElementType type, float potency, int damageOverTime);

}
