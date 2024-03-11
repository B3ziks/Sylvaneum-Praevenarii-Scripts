using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsAndTailsScreen : MonoBehaviour
{
    public GameObject SpinningImage; // Assign the spinning animation game object
    public GameObject HeadsImage;    // Assign the heads image game object
    public GameObject TailsImage;    // Assign the tails image game object

    private void Awake()
    {
        // Deactivate all images initially
        SpinningImage.SetActive(false);
        HeadsImage.SetActive(false);
        TailsImage.SetActive(false);

        // Deactivate the screen itself
        //gameObject.SetActive(false);
    }
}
