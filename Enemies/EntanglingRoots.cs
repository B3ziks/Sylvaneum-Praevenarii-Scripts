using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntanglingRoots : MonoBehaviour
{
    public float rootDuration = 2f; // Duration for which the character is rooted
    public float rootCooldown = 2f; // Time before the character can be rooted again

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();
        if (character != null && !character.IsRooted)
        {
            StartCoroutine(RootCharacter(character));
        }
    }

    private IEnumerator RootCharacter(Character character)
    {
        character.IsRooted = true;
        float originalSpeed = character.movementSpeed;
        character.movementSpeed = 0; // Set movement speed to 0 to root the character

        yield return new WaitForSeconds(rootDuration);

        character.movementSpeed = originalSpeed; // Restore original movement speed
        StartCoroutine(RootCooldown(character));
    }

    private IEnumerator RootCooldown(Character character)
    {
        yield return new WaitForSeconds(rootCooldown);
        character.IsRooted = false;
    }
}