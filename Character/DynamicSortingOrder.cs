using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Obstacles"));

        if (hit.collider != null)
        {
            // Find the midpoint of the obstacle's height
            float obstacleMidpoint = hit.collider.bounds.min.y + (hit.collider.bounds.size.y / 2);

            // Compare player's y position to the midpoint to determine sorting order
            spriteRenderer.sortingOrder = transform.position.y > obstacleMidpoint ? -1 : 1;
        }
        else
        {
            // Default sorting order when not near an obstacle
            spriteRenderer.sortingOrder = 1;
        }
    }

}