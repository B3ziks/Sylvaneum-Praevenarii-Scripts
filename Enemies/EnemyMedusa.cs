using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMedusa : Enemy
{
    private int maxDefenseStacks;
    private int currentDefenseStacks;

    public int CurrentDefenseStacks => currentDefenseStacks; // Public getter for current stacks

    private void Start()
    {
        // Initialize defense stacks
        maxDefenseStacks = 3; // You can adjust this as needed
        currentDefenseStacks = maxDefenseStacks;
    }

    public void ActivateDefense(int stacks)
    {
        maxDefenseStacks = stacks;
        currentDefenseStacks = stacks;
        // No need to call anything here, MedusaStoneDefenseAttack handles the figures
    }

    public override void TakeDamage(int damage)
    {
        if (currentDefenseStacks > 0)
        {
            currentDefenseStacks--;
            // MedusaStoneDefenseAttack script will handle the figure deactivation
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    // Optionally, if you want to reset the defense stacks
    public void ResetDefenseStacks()
    {
        currentDefenseStacks = maxDefenseStacks;
    }
}