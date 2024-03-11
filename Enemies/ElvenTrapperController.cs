using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElvenTrapperController : EliteEnemyController
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CircleCollider2D detectionCollider;
    [SerializeField]
    private float detectionRange = 3f;

    private bool isObstacle = true;
    private Enemy enemyComponent; // Reference to the Enemy component

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        ConfigureCollider();

        // Get the Enemy component
        enemyComponent = GetComponent<Enemy>();
    }

    private void ConfigureCollider()
    {
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        }

        detectionCollider.radius = detectionRange;
        detectionCollider.isTrigger = true;
    }

    protected override void Update()
    {
        base.Update();

        if (!isObstacle)
        {
            animator.SetBool("isObstacle", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && isObstacle)
        {
            isObstacle = false;
            if (enemyComponent != null)
            {
                enemyComponent.stats.moveSpeed = 2f;
                enemyComponent.stats.timeToAttack = 1f;
            }
        }
    }

    private void OnEnable()
    {
        isObstacle = true;
        if (enemyComponent != null)
        {
            enemyComponent.stats.moveSpeed = 0f;
            enemyComponent.stats.timeToAttack = 99f;
        }
    }

    protected override void ExecuteSpecialAttack(SpecialAttack attack)
    {
        if (!isObstacle)
        {
            base.ExecuteSpecialAttack(attack);
        }
    }
}
