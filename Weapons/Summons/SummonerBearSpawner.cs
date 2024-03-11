using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerBearSpawner : SummonerSpawnerBase
{
    public override void Attack()
    {
        base.Attack(); // Call the base class implementation

        // Additional logic specific to SummonerWolfSpawner can go here
    }

    protected override void InitializeSummon(GameObject summonInstance)
    {
        BearSummon bearSummon = summonInstance.GetComponent<BearSummon>();
        if (bearSummon != null)
        {
            bearSummon.weaponBaseReference = this;
        }
        else
        {
            Debug.LogError("[SummonerWolfSpawner] WolfSummon component not found on summonInstance!");
        }

        SummonComboBuff comboBuff = summonInstance.GetComponent<SummonComboBuff>();
        if (comboBuff != null)
        {
            comboBuff.SetWeaponBaseReference(this);
        }
        // Additional bear initializations
    }

}
