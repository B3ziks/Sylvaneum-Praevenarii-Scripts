using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        EarthTotem earthTotem = totemInstance.GetComponent<EarthTotem>();
        if (earthTotem != null)
        {
            earthTotem.weaponBaseReference = this;
        }
    }
  
}