using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntingTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        TauntingTotem tauntingTotem = totemInstance.GetComponent<TauntingTotem>();
        if (tauntingTotem != null)
        {
            tauntingTotem.weaponBaseReference = this;
        }
    }
}
