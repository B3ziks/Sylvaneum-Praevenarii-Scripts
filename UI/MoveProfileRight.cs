using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProfileRight : MonoBehaviour
{
    private Vector3 originalPosition;

    private void Start()
    {
        // Store the original position at start
        originalPosition = transform.position;
    }

    public void MoveRight(float distance)
    {
        transform.position += Vector3.right * distance;
    }

    public void ResetPosition()
    {
        // Reset to the original position
        transform.position = originalPosition;
    }
}
