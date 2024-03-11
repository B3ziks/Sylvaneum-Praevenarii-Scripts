using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        FireTotem fireTotem = totemInstance.GetComponent<FireTotem>();
        if (fireTotem != null)
        {
            fireTotem.weaponBaseReference = this;
        }
    }

}