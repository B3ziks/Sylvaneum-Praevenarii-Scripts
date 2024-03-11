using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/ArachneTrueFormAttack")]
public class ArachneTrueFormAttack : SpecialAttack
{
    public float effectDuration = 10.0f; // Duration of the Arachne True Form effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        ArachneTrueFormEffect trueFormEffect = executor.GetMonoBehaviour().GetComponent<ArachneTrueFormEffect>();
        if (trueFormEffect == null)
        {
            Debug.LogError("ArachneTrueFormEffect component not found on the executor!");
            return;
        }

        // Reset the initial delay to ensure the next uses of this attack follow the regular cooldown
        this.initialDelay = 0;
        executor.GetMonoBehaviour().StartCoroutine(ApplyArachneTrueFormEffect(trueFormEffect));
    }

    private IEnumerator ApplyArachneTrueFormEffect(ArachneTrueFormEffect trueFormEffect)
    {
        trueFormEffect.ActivateTrueForm();

        // Wait for the duration of the effect
        yield return new WaitForSeconds(effectDuration);

        trueFormEffect.DeactivateTrueForm();
    }
}