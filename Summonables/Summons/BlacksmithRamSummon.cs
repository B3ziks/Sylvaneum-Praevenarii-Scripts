using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithRamSummon : MonoBehaviour
{
    [Header("Player and Follow")]
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 6f;
    private Animator animator;


    [Header("Weapon Enhancement Data")]
    public WeaponBase weaponBaseReference;
    private float enhancementCooldown;
    private WeaponManager weaponManager;

    private void Start()
    {
        animator = GetComponent<Animator>();

        enhancementCooldown = weaponBaseReference.weaponData.stats.timeToAttack;

        // Find the player transform and weapon manager
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {
        FollowPlayerIfTooFar();
        enhancementCooldown -= Time.deltaTime;
        if (enhancementCooldown <= 0f)
        {
            EnhanceWeapons();
            enhancementCooldown = weaponBaseReference.weaponData.stats.timeToAttack * 3;
        }
    }

    private void FollowPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)directionToPlayer * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);  // <-- Set isAttacking to false here
            FlipSpriteBasedOnDirection(directionToPlayer);
        }
    }

    private void FlipSpriteBasedOnDirection(Vector2 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face right
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face left
        }
    }

    private void EnhanceWeapons()
    {
        if (weaponManager != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);  // <-- Set isAttacking to false here

            // Call a method in the WeaponManager to enhance all weapons
            weaponManager.EnhanceAllWeaponsDamageTemporarily();
        }
    }
}
