using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SpikeBallCombo : MonoBehaviour
{
    [SerializeField] public Animator animator;
    public bool isComboActive =false; // By default, the combo is active

    private bool isSpikeBallActive;
    private bool isInCooldown = false;
    [SerializeField] private int damageValue = 100; // Adjust damage as needed
    [SerializeField] private float spikeBallDuration = 5f;  // 5 seconds duration
    [SerializeField] private float spikeBallCooldown = 10f; // 10 seconds cooldown

    private Character playerCharacter; // To reference the armor value
    private BoxCollider2D boxCollider; // To reference the box collider

    public void SetComboActiveState(bool state)
    {
        isComboActive = state;
    }
    private void Start()
    {
        // Get the Animator from the child GameObject
        animator = GetComponentInChildren<Animator>();

        // Get the Character script to reference the armor
        playerCharacter = GetComponent<Character>();
        // Get the BoxCollider2D component
        boxCollider = GetComponent<BoxCollider2D>();
        // Safety check
        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator not found on the child object.");
        }

        if (playerCharacter == null)
        {
            UnityEngine.Debug.LogError("Character script not found on this object.");
        }
    }

    private void Update()
    {
        if (isComboActive && Input.GetKeyDown(KeyCode.R) && !isSpikeBallActive && !isInCooldown)
        {
            StartCoroutine(ActivateSpikeBallForDuration());
        }
    }

    private IEnumerator ActivateSpikeBallForDuration()
    {
        ActivateSpikeBall();
        yield return new WaitForSeconds(spikeBallDuration);
        DeactivateSpikeBall();
        isInCooldown = true;
        yield return new WaitForSeconds(spikeBallCooldown);
        isInCooldown = false;
    }

    private void ActivateSpikeBall()
    {
        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator is null!");
        }
        else if (!animator.isActiveAndEnabled)
        {
            UnityEngine.Debug.LogError("Animator is not active or enabled!");
        }
        else
        {
            // Set the parameter
            animator.SetBool("isSpikeBall", true);
        }
        if (animator != null)
        {
            isSpikeBallActive = true;
            playerCharacter.armor += 50;

        }
        else
        {
            UnityEngine.Debug.LogError("Animator is null. Cannot activate SpikeBall.");
        }

        // Set isTrigger to true for the box collider
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }

    private void DeactivateSpikeBall()
    {
        if (animator != null)
        {
            animator.SetBool("isSpikeBall", false);
            isSpikeBallActive = false;
            playerCharacter.armor -= 50;

            // Set isTrigger to true for the box collider
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }

            // Check if there are obstacles close by before deactivating isTrigger
            Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, 1f); // Change 1f to an appropriate radius
            if (hitCollider && hitCollider.CompareTag("Obstacle"))
            {
                // If obstacle detected, delay the deactivation and push player away from the obstacle
                StartCoroutine(DelayedDeactivate(hitCollider.transform.position));
            }
            else
            {
                CompleteDeactivation();
            }
        }
    }

    private IEnumerator DelayedDeactivate(Vector2 obstaclePosition)
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds or adjust as needed

        Vector2 pushDirection = (transform.position - (Vector3)obstaclePosition).normalized;
        float pushForce = 2f; // Adjust as needed
        GetComponent<Rigidbody2D>().AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

        // After pushing, wait a little longer then finalize deactivation
        yield return new WaitForSeconds(0.5f);
        CompleteDeactivation();
    }

    private void CompleteDeactivation()
    {
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSpikeBallActive && collision.CompareTag("Enemy"))
        {
            int damageAfterArmor = damageValue + (100 + (100 + playerCharacter.armor));
            collision.GetComponent<IDamageable>().TakeDamage(damageAfterArmor);
            // Assuming you still want to show the damage after armor reduction
            PostDamage(damageAfterArmor, collision.transform.position); 
        }
    }
    public void PostDamage(int damage, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition);
    }
}