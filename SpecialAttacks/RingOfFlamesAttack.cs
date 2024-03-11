using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Ring Of Flames")]
public class RingOfFlamesAttack : SpecialAttack
{
    public PoolObjectData flameObjectData;
    public float radius;
    public int numberOfFlames;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        float angleStep = 360f / numberOfFlames;
        for (int i = 0; i < numberOfFlames; i++)
        {
            float angleInRad = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad), 0) * radius + executor.transform.position;

            GameObject flame = poolManager.GetObject(flameObjectData);
            flame.transform.position = position;
            flame.transform.rotation = Quaternion.identity;
            //flame.transform.SetParent(bossController.transform);
            FlameOnCharacter flameScript = flame.GetComponent<FlameOnCharacter>();

            // Set the bossTransform reference
            FollowBoss followBossScript = flame.GetComponent<FollowBoss>();
            if (followBossScript != null)
            {
                followBossScript.bossTransform = executor.transform;
            }
            else
            {
                UnityEngine.Debug.LogError("FollowBoss script not found on flame object!");
            }
        }
    }

}