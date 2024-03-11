using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Special Attacks/Blinding Light Attack")]
public class BlindingLightAttack : SpecialAttack
{
    public float blindingDuration = 2.0f; // Duration of the blinding light in seconds

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Start coroutine to handle the blinding light effect
        executor.GetMonoBehaviour().StartCoroutine(BlindingLightSequence(executor));
    }

    private IEnumerator BlindingLightSequence(ISpecialAttackExecutor executor)
    {
        // Find the BlindingEffectScreen in the scene
        BlindingEffectScreen blindingEffectScreen = GameObject.FindObjectOfType<BlindingEffectScreen>();
        if (blindingEffectScreen == null)
        {
            Debug.LogError("BlindingEffectScreen is not found in the scene!");
            yield break;
        }

        Image blindingImage = blindingEffectScreen.GetComponent<Image>();
        if (blindingImage == null)
        {
            Debug.LogError("BlindingEffectScreen does not have an Image component!");
            yield break;
        }

        // Fade in the blinding light
        yield return executor.GetMonoBehaviour().StartCoroutine(FadeImage(blindingImage, 1.0f));

        // Wait for the duration of the blinding effect
        yield return new WaitForSeconds(blindingDuration);

        // Fade out the blinding light
        yield return executor.GetMonoBehaviour().StartCoroutine(FadeImage(blindingImage, 0.0f));
    }

    private IEnumerator FadeImage(Image image, float targetAlpha)
    {
        float speed = Mathf.Abs(image.color.a - targetAlpha) / 0.5f; // Half a second for fade in/out
        while (!Mathf.Approximately(image.color.a, targetAlpha))
        {
            float newAlpha = Mathf.MoveTowards(image.color.a, targetAlpha, speed * Time.deltaTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }
}