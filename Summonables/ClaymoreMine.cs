using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaymoreMine : MonoBehaviour
{
    public delegate void ExplodeAction(GameObject mine);
    public event ExplodeAction OnExplode;
    public WeaponBase weaponBase;
    public float shrapnelRange = 10f;
    public PoolObjectData shrapnelPoolData;
    private PoolManager poolManager;

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    public void SetWeapon(WeaponBase weapon)
    {
        this.weaponBase = weapon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            SpawnShrapnelEffect(transform.position, direction);
            Explode();
        }
    }

    void Explode()
    {
        OnExplode?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    private void SpawnShrapnelEffect(Vector2 origin, Vector2 direction)
    {
        GameObject shrapnelEffect = poolManager.GetObject(shrapnelPoolData);
        shrapnelEffect.transform.position = origin;
        shrapnelEffect.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        shrapnelEffect.SetActive(true);

        ClaymoreShrapnelController shrapnelController = shrapnelEffect.GetComponent<ClaymoreShrapnelController>();
        if (shrapnelController != null)
        {
            shrapnelController.ActivateEffect(weaponBase.GetDamage(), direction, shrapnelRange);
        }
    }
}