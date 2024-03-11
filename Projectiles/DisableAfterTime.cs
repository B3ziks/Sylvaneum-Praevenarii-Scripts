using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisableAfterTime : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float timeToDisable = 0.2f;
    float timer;

    private void OnEnable()
    {
        timer = timeToDisable;
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }

}
