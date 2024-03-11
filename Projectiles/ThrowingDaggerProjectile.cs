using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingDaggerProjectile : MonoBehaviour
{
    public float attackArea = 0.5f;
    [SerializeField] float speed;
    public int damage = 5;
    public int numOfHits = 1;

    List<IDamageable> enemiesHit;

    float ttl = 6f;

    void Update()
    {
        MoveTowardsMouse();

        if (Time.frameCount % 6 == 0)
        {
            HitDetection();
        }
        TimerToLive();
    }

    private void MoveTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the target position is on the same Z-plane as the projectile.

        // Move the projectile towards the mouse position.
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, speed * Time.deltaTime);
    }

    private void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            if (numOfHits > 0)
            {
                IDamageable enemy = c.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    if (CheckRepeatHit(enemy) == false)
                    {
                        PostDamage(damage, transform.position);
                        enemiesHit.Add(enemy);
                        enemy.TakeDamage(damage);
                        numOfHits -= 1;
                    }
                }

            }
            else
            {
                break;
            }
        }
        if (numOfHits <= 0)
        {
            Destroy(gameObject);
        }
    }

    private bool CheckRepeatHit(IDamageable enemy)
    {
        if (enemiesHit == null) { enemiesHit = new List<IDamageable>(); }
        return enemiesHit.Contains(enemy);
    }

    private void TimerToLive()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0f)
        {
            Destroy(gameObject);
        }
    }

    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), worldPosition);
    }
}