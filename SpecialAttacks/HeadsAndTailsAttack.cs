using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Special Attacks/HeadsAndTailsAttack")]
public class HeadsAndTailsAttack : SpecialAttack
{
    public float spinDuration = 2.0f; // Duration of the spinning animation
    public RatBuffStatus ratBuffStatus; // Assign this through the inspector

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Find the container game object that holds the heads and tails images
        HeadsAndTailsScreen container = GameObject.FindObjectOfType<HeadsAndTailsScreen>();
        if (container == null)
        {
            Debug.LogError("HeadsAndTailsContainer is not found in the scene!");
            return;
        }
   
    
    // Activate the container game object
    container.SpinningImage.SetActive(false);
        container.HeadsImage.SetActive(false);
        container.TailsImage.SetActive(false);

        // Attempt to find the RatBuffStatus on the boss GameObject at runtime
        RatBuffStatus ratBuffStatus = executor.GetMonoBehaviour().GetComponent<RatBuffStatus>() ?? FindObjectOfType<RatBuffStatus>();
        if (ratBuffStatus != null)
        {
            // Continue with existing logic
            executor.GetMonoBehaviour().StartCoroutine(HeadsOrTailsSequence(executor, container, ratBuffStatus));
        }
        else
        {
            Debug.LogError("RatBuffStatus component not found on the executor!");
        }
    }

    private IEnumerator HeadsOrTailsSequence(ISpecialAttackExecutor executor, HeadsAndTailsScreen container, RatBuffStatus ratBuffStatus)
    {
        // Activate the spinning animation
        container.SpinningImage.gameObject.SetActive(true);
        container.HeadsImage.gameObject.SetActive(false);
        container.TailsImage.gameObject.SetActive(false);

        // Wait for the spinning to finish
        yield return new WaitForSeconds(spinDuration);

        // Randomly choose the outcome and display the corresponding image
        bool isHeads = UnityEngine.Random.Range(0, 2) == 0;
        container.SpinningImage.gameObject.SetActive(false);
        if (isHeads)
        {
            container.HeadsImage.gameObject.SetActive(true);
            // Hide the result after a short delay
            yield return new WaitForSeconds(1.0f);
            container.HeadsImage.gameObject.SetActive(false);
            // If it's heads, no buff is applied
        }
        else
        {
            container.TailsImage.gameObject.SetActive(true);
            // If it's tails, apply the rat buff
            ratBuffStatus.StoreOriginalValues();
            ratBuffStatus.ActivateEffect();
            // Hide the result after a short delay
            yield return new WaitForSeconds(1.0f);
            container.TailsImage.gameObject.SetActive(false);
            // Wait for the duration of the rat buff effect
            yield return new WaitForSeconds(ratBuffStatus.effectDuration);

            // Deactivate the rat buff effect
            ratBuffStatus.DeactivateEffect();
        }

     
    }
}