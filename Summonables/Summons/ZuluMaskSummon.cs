using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZuluMaskSummon : MonoBehaviour
{
    [Header("Player Data")]
    public Transform playerTransform;
    public float orbitDistance = 2f;
    public float orbitSpeed = 2f;

    [Header("Healing Data")]
    public WeaponBase weaponBaseReference;
    private float healCooldown;
    private Character playerCharacter;

    private void Start()
    {
        healCooldown = weaponBaseReference.weaponData.stats.timeToAttack;
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
            playerCharacter = playerTransform.GetComponent<Character>();
        }
    }

    private void Update()
    {
        CircleAroundPlayer();
        healCooldown -= Time.deltaTime;
        if (healCooldown <= 0f)
        {
            CastHeal();
            healCooldown = weaponBaseReference.weaponData.stats.timeToAttack *5;
        }
    }

    private void CircleAroundPlayer()
    {
        transform.RotateAround(playerTransform.position, Vector3.forward, orbitSpeed * Time.deltaTime);
        transform.position = playerTransform.position + (transform.position - playerTransform.position).normalized * orbitDistance;

        FlipBasedOnPosition();
    }

    private void FlipBasedOnPosition()
    {
        if (transform.position.x < playerTransform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face left
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face right
        }
    }

    private void CastHeal()
    {
        if (playerCharacter != null)
        {
            int healAmount = weaponBaseReference.weaponData.stats.damage; // Assuming damage value is used as the heal amount
            playerCharacter.Heal(healAmount);
        }
    }
}

