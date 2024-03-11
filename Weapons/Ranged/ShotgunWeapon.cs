using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShotgunWeapon : WeaponBase
{
    [SerializeField] private GameObject shotgunSmokeEffect; // Prefab for the shotgun smoke effect.

    [SerializeField] private float attackRadius = 2f; // Area of effect as a circle
    [SerializeField] private float effectDistanceFromCharacter = 2.0f; // How far the effect appears from the character.
    [SerializeField] private float recoilDashSpeed = 20f; // Speed at which the player dashes back
    [SerializeField] private float recoilDashDuration = 0.1f; // Duration of the recoil dash
    [SerializeField] private float recoilDistance = 5.0f; // Distance the player is pushed back.
    [SerializeField] private float recoilForce = 2000.0f; // Distance the player is pushed back.

    [SerializeField] private Rigidbody2D rb;

    private bool isRecoiling = false;

    protected override void Awake()
    {
        // Assuming this WeaponBase is a child of the character GameObject,
        // or you have some way to find your player character.
        owner = GetComponentInParent<Character>();

        // Safety check to ensure you actually found the owner
        if (owner == null)
        {
            UnityEngine.Debug.LogError("Owner not found on " + gameObject.name);
        }
        else
        {
            rb = owner.GetComponent<Rigidbody2D>();
            // as long as you ensure it's not null first.
        }
    }
    public override void Attack()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        // Adjust the position so the smoke effect is positioned correctly relative to the player
        Vector3 effectPosition = transform.position + directionToMouse * effectDistanceFromCharacter;

        // Calculate the rotation so the smoke effect bottom is aligned with the direction
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
        Quaternion effectRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the smoke effect to be active and position it
        shotgunSmokeEffect.SetActive(false);
        shotgunSmokeEffect.transform.position = effectPosition;
        shotgunSmokeEffect.transform.rotation = effectRotation;
        // Pass the WeaponData to the ShotgunSmoke script
        ShotgunSmoke smokeScript = shotgunSmokeEffect.GetComponent<ShotgunSmoke>();
        if (smokeScript != null)
        {
            smokeScript.SetWeaponReference(this);
        }
        shotgunSmokeEffect.SetActive(true);

        // Apply recoil dash
        Vector3 recoilDirection = -directionToMouse.normalized;
        StartCoroutine(RecoilDash(recoilDirection));

        // Deactivate the smoke effect after the attack animation
        StartCoroutine(DeactivateEffectAfterDelay(1f));
    }
 
    private IEnumerator DeactivateEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shotgunSmokeEffect.SetActive(false);
    }

    // Method to handle recoil as a dash
    private IEnumerator RecoilDash(Vector3 direction)
    {
        float endTime = Time.time + recoilDashDuration;
        while (Time.time < endTime)
        {
            // Apply the force over multiple frames to simulate a sustained push
            rb.AddForce(direction * (recoilForce / recoilDashDuration) * Time.fixedDeltaTime, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }


}