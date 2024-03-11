using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArachneTrueFormEffect : MonoBehaviour
{
    [SerializeField] private Animator animatorComponent;

    private void Start()
    {
        animatorComponent = GetComponentInChildren<Animator>();
    }

    public void ActivateTrueForm()
    {
        animatorComponent.SetBool("isSpider", true);

        // Implement any additional stat modifications or effect applications here
    }

    public void DeactivateTrueForm()
    {
        animatorComponent.SetBool("isSpider", false);

        // Implement any deactivation logic, resetting stats or effects here
    }
}