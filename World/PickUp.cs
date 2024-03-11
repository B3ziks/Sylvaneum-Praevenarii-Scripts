using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    //[SerializeField] int healAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character c = collision.GetComponent<Character>();
        IPickUpObject pickUpObject = GetComponent<IPickUpObject>();

        if (c != null && pickUpObject != null)
        {
            pickUpObject.OnPickUp(c);
            Destroy(gameObject);
        }
    }
}
