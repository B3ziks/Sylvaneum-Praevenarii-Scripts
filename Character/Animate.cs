using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    Animator animator;
    public float horizontal;

    private void Update()
    {
        animator.SetFloat("Horizontal", horizontal);
    }

    internal void SetAnimate(GameObject animObject)
    {
        animator = animObject.GetComponent<Animator>();
    }

    public void SetIsMoving(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }
}