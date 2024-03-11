using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Special Attacks/BerserkStatusAttack")]
public class BerserkStatusAttack : SpecialAttack
{
    public float effectDuration = 5.0f; // Duration of the Berserk effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        BerserkStatus berserkStatus = executor.GetMonoBehaviour().GetComponent<BerserkStatus>();
        if (berserkStatus == null)
        {
            Debug.LogError("BerserkStatus component not found on the executor!");
            return;
        }

        executor.GetMonoBehaviour().StartCoroutine(ApplyBerserkEffect(berserkStatus));
    }

    private IEnumerator ApplyBerserkEffect(BerserkStatus berserkStatus)
    {
        berserkStatus.StoreOriginalValues();
        // Activate the effect
        berserkStatus.ActivateEffect();

        // Wait for the duration of the effect
        yield return new WaitForSeconds(effectDuration);

        // Deactivate the effect
        berserkStatus.DeactivateEffect();
    }
}