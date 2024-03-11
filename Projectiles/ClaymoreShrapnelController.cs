using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClaymoreShrapnelController : MonoBehaviour
{
    private int damage;
    private Vector2 direction;
    private float range;
    private float speed = 10f; // Speed of shrapnel movement

    private void Update()
    {
        if (range > 0)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            range -= speed * Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ActivateEffect(int damage, Vector2 dir, float range)
    {
        this.damage = damage;
        this.direction = dir;
        this.range = range;
        StartCoroutine(DeactivateAfterDelay(1f)); // 1 second delay
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy target = other.GetComponent<Enemy>();
            if (target != null)
            {
                target.TakeDamage(damage);
                PostDamageMessage(damage, other.transform.position);
            }
        }
    }

    private void PostDamageMessage(int damage, Vector3 position)
    {
        // Assuming you have a method to determine the message color based on damage type or other criteria
        Color messageColor = DetermineMessageColor();
        MessageSystem.instance.PostMessage(damage.ToString(), position, messageColor);
    }

    private Color DetermineMessageColor()
    {
        // Define how you determine the color of the message
        // For now, returning a default color
        return new Color(0.8f, 0.8f, 0.8f, 1f);
    }
}