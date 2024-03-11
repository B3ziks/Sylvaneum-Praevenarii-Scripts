using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoneGaze : MonoBehaviour
{
    [SerializeField] private DataContainer dataContainer;
    private Character targetCharacter;
    private SpriteRenderer characterSpriteRenderer;

    private void Start()
    {
        targetCharacter = GetComponent<Character>();

        // Use GetComponentsInChildren to find the SpriteRenderer, even if it's not on a direct child
        SpriteRenderer[] allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        characterSpriteRenderer = allSpriteRenderers.FirstOrDefault(sr => sr.gameObject.name == dataContainer.selectedCharacter.Name + "(Clone)");

        if (characterSpriteRenderer == null)
        {
            Debug.LogError("Character SpriteRenderer not found.");
        }
    }

    public void ActivateEffect(float duration)
    {
        if (characterSpriteRenderer != null)
        {
            characterSpriteRenderer.color = Color.gray;
            StartCoroutine(EffectCoroutine(duration));
        }
    }

    private IEnumerator EffectCoroutine(float duration)
    {
        if (targetCharacter != null)
        {
            float originalSpeed = targetCharacter.movementSpeed;
            targetCharacter.movementSpeed = 0;
            yield return new WaitForSeconds(duration);
            targetCharacter.movementSpeed = originalSpeed;
            if (characterSpriteRenderer != null)
            {
                characterSpriteRenderer.color = Color.white;
            }
        }
    }
}