using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Chakram : Projectile
{
    Vector3 chakramDirection;
    private bool isReturning = false;
    private PlayerMove playerMove;
    private float timeSinceFirstHit = -1f;  // Time since the first enemy was hit.

    protected void Awake()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        chakramDirection = new Vector3(dir_x, dir_y).normalized;
        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected override void Update()
    {
        base.Update();
        Move();

        // Check if it's time to return after hitting the first enemy
        if (timeSinceFirstHit >= 0)  // If an enemy has been hit
        {
            timeSinceFirstHit += Time.deltaTime;
            if (timeSinceFirstHit >= 0.5f)  // Return after 1 second of hitting the first enemy
            {
                StartReturning();
            }
        }

        if (Time.frameCount % 6 == 0)
        {
            HitDetection();
        }
    }

    public override void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);
                numOfHits -= 1;

                // Start the countdown for returning after hitting the first enemy
                if (timeSinceFirstHit == -1f)
                {
                    timeSinceFirstHit = 0f;
                }

                if (numOfHits <= 0)
                {
                    StartReturning();
                    return;
                }
            }
            else if (c.CompareTag("Obstacle") && !isReturning)
            {
                StartReturning();
                return;
            }
        }
    }

    private void StartReturning()
    {
        if (!isReturning)
        {
            UnityEngine.Debug.Log("Chakram is now returning!");
            isReturning = true;
            GetComponent<Collider2D>().enabled = false;
            direction = (playerMove.transform.position - transform.position).normalized;
        }
    }

    private void Move()
    {
        if (isReturning)
        {
            chakramDirection = (playerMove.transform.position - transform.position).normalized;

            if (Vector3.Distance(transform.position, playerMove.transform.position) <= 1f)
            {
                transform.position = playerMove.transform.position;
                StartCoroutine(DelayBeforeDestroy());
                return;
            }
        }

        transform.position += chakramDirection * speed * Time.deltaTime;
    }

    private IEnumerator DelayBeforeDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        DestroyProjectile();
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            StartReturning();
        }
    }

    protected override void DestroyProjectile()
    {
        // Stop any ongoing delay before destroy coroutines
        StopCoroutine(DelayBeforeDestroy());

        // Check if the chakram is returning before proceeding with the destroy/pool logic
        if (!isReturning) return;

        if (poolMember == null)
        {
            Destroy(gameObject);
        }
        else
        {
            poolMember.ReturnToPool();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isReturning = false;
        transform.rotation = Quaternion.identity;
        chakramDirection = Vector3.zero;
        timeSinceFirstHit = -1f;
        GetComponent<Collider2D>().enabled = true;
    }
}