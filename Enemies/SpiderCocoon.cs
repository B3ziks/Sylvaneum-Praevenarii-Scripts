using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCocoon : MonoBehaviour
{
    private Character targetCharacter;

    private void Awake()
    {
        // Assuming the SpiderCocoon is a child of the character GameObject
        targetCharacter = GetComponentInParent<Character>();
        gameObject.SetActive(false); // Cocoon starts as inactive
    }

    public void ActivateStun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        if (targetCharacter != null)
        {
            // Store original movement speed and set to 0 to apply stun
            float originalSpeed = targetCharacter.movementSpeed;
            targetCharacter.movementSpeed = 0;

            yield return new WaitForSeconds(duration);

            // After the duration, restore original movement speed and deactivate cocoon
            targetCharacter.movementSpeed = originalSpeed;
            gameObject.SetActive(false);
        }
    }
}