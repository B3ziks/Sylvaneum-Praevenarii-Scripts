using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRatController : BossController
{
    private Animator animator;  // Animator to handle the animations
    private bool isWalking = false;  // Track if the boss rat is walking
    private bool isAttacking = false;  // Track if the boss rat is attacking

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator component not found on BossRat!");
        }
    }

    protected override void Update()
    {
        base.Update();

    }

  

    // You can override the ExecuteSpecialAttack or other methods if needed to
    // set the isWalking or isAttacking flags based on the logic you want.
}