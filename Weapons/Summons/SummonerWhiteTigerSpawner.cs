using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerWhiteTigerSpawner : SummonerSpawnerBase
{
    public override void Attack()
    {
        base.Attack(); // Call the base class implementation

        // Additional logic specific to SummonerWolfSpawner can go here
    }

    protected override void InitializeSummon(GameObject summonInstance)
    {

        // Additional wolf-specific initializations.
        WhiteTigerSummon wolfSummon = summonInstance.GetComponent<WhiteTigerSummon>();
        if (wolfSummon != null)
        {
            wolfSummon.weaponBaseReference = this;
            // Perform additional initializations if necessary.
        }
        else
        {
            Debug.LogError("[SummonerWolfSpawner] WolfSummon component not found on summonInstance!");
        }

        // Initialize the SummonComboBuff if it's present on the wolf.
        SummonComboBuff comboBuff = summonInstance.GetComponent<SummonComboBuff>();
        if (comboBuff != null)
        {
            comboBuff.SetWeaponBaseReference(this);
        }
    }
}
