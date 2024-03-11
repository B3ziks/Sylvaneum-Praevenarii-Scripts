using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        IceTotem iceTotem = totemInstance.GetComponent<IceTotem>();
        if (iceTotem != null)
        {
            iceTotem.weaponBaseReference = this;
        }
    }

}