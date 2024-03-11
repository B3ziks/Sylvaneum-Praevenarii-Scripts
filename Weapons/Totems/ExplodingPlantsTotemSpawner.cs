using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPlantsTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        ExplodingPlantsTotem explodingPlantsTotem = totemInstance.GetComponent<ExplodingPlantsTotem>();
        if (explodingPlantsTotem != null)
        {
            explodingPlantsTotem.weaponBaseReference = this;
        }
    }
}
