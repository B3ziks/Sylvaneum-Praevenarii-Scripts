using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRecoil : MonoBehaviour
{
    public float recoilStrength = 2000f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Use this to test the recoil with the Space key
        {
            ApplyRecoil();
        }
    }

    private void ApplyRecoil()
    {
        // For simplicity, recoil to the left
        Vector2 recoilDirection = Vector2.left;
        rb.AddForce(recoilDirection * recoilStrength, ForceMode2D.Force);
    }
}