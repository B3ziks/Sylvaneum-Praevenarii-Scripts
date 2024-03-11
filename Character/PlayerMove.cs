using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rgbd2d;
    [HideInInspector]
    public Vector3 movementVector;
    [HideInInspector]
    public float lastHorizontalDeCoupledVector;
    [HideInInspector]
    public float lastVerticalDeCoupledVector;

    [HideInInspector]
    public float lastHorizontalCoupledVector;
    [HideInInspector]
    public float lastVerticalCoupledVector;

    private DashMechanic dashMechanic;  // Reference to the DashMechanic script

    private Character character;  // Reference to the Character script
    private float speed;  // This will now use the movementSpeed from the Character script
    private Vector3 knockbackVelocity;

    Animate animate;

    private void Awake()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        movementVector = new Vector3();
        animate = GetComponent<Animate>();
        dashMechanic = GetComponent<DashMechanic>();  // Get the DashMechanic script
        character = GetComponent<Character>();  // Get the Character script
    }

    private void Start()
    {
        lastHorizontalDeCoupledVector = -1f;
        lastVerticalDeCoupledVector = 1f;

        lastHorizontalCoupledVector = -1f;
        lastVerticalCoupledVector = -1f;

        speed = character.movementSpeed;  // Set the speed to movementSpeed from Character script
    }

    // Update is called once per frame
    void Update()
    {
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");

        if (movementVector.x != 0 || movementVector.y != 0)
        {
            lastHorizontalCoupledVector = movementVector.x;
            lastVerticalCoupledVector = movementVector.y;
        }

        if (movementVector.x != 0)
        {
            lastHorizontalDeCoupledVector = movementVector.x;
        }
        if (movementVector.y != 0)
        {
            lastVerticalDeCoupledVector = movementVector.y;
        }

        animate.horizontal = movementVector.x;

        movementVector *= character.movementSpeed;  // Directly use movementSpeed from Character script here

        rgbd2d.velocity = movementVector;

        // Set isMoving parameter based on whether the player is stationary or moving
        animate.SetIsMoving(!IsStationary());
    }
    // Method to check if the player is stationary
    public bool IsStationary()
    {
        return rgbd2d.velocity.sqrMagnitude < 0.01f;
    }
    public void Knockback(Vector3 direction, float force, float timeWeight)
    {
        knockbackVelocity = direction.normalized * force;
        StartCoroutine(KnockbackDuration(timeWeight));
    }

    private IEnumerator KnockbackDuration(float timeWeight)
    {
        float timer = timeWeight;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            rgbd2d.velocity = Vector2.Lerp(rgbd2d.velocity, knockbackVelocity, timer / timeWeight);
            yield return null;
        }
    }
}