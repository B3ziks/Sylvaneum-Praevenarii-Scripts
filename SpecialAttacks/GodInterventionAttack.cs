using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/GodInterventionAttack")]
public class GodInterventionAttack : SpecialAttack
{
    public float effectDuration = 10.0f; // Duration of the God Intervention effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        GodInterventionEffect interventionEffect = executor.GetMonoBehaviour().GetComponent<GodInterventionEffect>();
        if (interventionEffect == null)
        {
            Debug.LogError("GodInterventionEffect component not found on the executor!");
            return;
        }

        executor.GetMonoBehaviour().StartCoroutine(ApplyGodInterventionEffect(interventionEffect));
    }

    private IEnumerator ApplyGodInterventionEffect(GodInterventionEffect interventionEffect)
    {
        interventionEffect.StorageOriginalValues();
        // Activate the effect
        interventionEffect.ActivateEffect();

        // Wait for the duration of the effect
        yield return new WaitForSeconds(effectDuration);

        // Deactivate the effect
        interventionEffect.DeactivateEffect();
    }
}