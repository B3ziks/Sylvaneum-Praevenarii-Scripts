using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileMultishoot : EnemyProjectile
{
    public void Initialize(Vector2 direction, float speed)
    {
        SetCustomDirection(direction, speed);
    }

    private void SetCustomDirection(Vector2 direction, float speed)
    {
        rb.velocity = direction * speed;
        SetDirection(direction.x, direction.y);
    }

    protected override void Update()
    {
        // You can choose to either handle the motion here (similar to the base class) or keep it empty if the base behavior isn't required.
    }
}