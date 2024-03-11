using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoss : MonoBehaviour
{
    public Transform bossTransform;
    private Vector3 offset;

    private void Start()
    {
        if (bossTransform != null)
        {
            offset = transform.position - bossTransform.position;
        }
    }

    private void Update()
    {
        if (bossTransform != null)
            transform.position = bossTransform.position + offset;
    }
}