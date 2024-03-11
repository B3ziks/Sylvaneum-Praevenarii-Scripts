using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject damageMessage;
    [SerializeField] GameObject SpecialAttackMessage;

    List<TMPro.TextMeshPro> damageMessagePool;
    List<TMPro.TextMeshPro> specialAttackMessagePool;

    int objectCount = 10;
    int damageCount = 0;
    int specialAttackCount = 0;

    private void Start()
    {
        damageMessagePool = new List<TMPro.TextMeshPro>();
        specialAttackMessagePool = new List<TMPro.TextMeshPro>();

        for (int i = 0; i < objectCount; i++)
        {
            PopulateDamageMessages();
            PopulateSpecialAttackMessages();
        }
    }

    public void PopulateDamageMessages()
    {
        GameObject go = Instantiate(damageMessage, transform);
        damageMessagePool.Add(go.GetComponent<TMPro.TextMeshPro>());
        go.SetActive(false);
    }

    public void PopulateSpecialAttackMessages()
    {
        GameObject go = Instantiate(SpecialAttackMessage, transform);
        specialAttackMessagePool.Add(go.GetComponent<TMPro.TextMeshPro>());
        go.SetActive(false);
    }

    public void PostMessage(string text, Vector3 worldPosition, Color? color = null)
    {
        TMPro.TextMeshPro message = damageMessagePool[damageCount];
        message.gameObject.SetActive(true);
        message.transform.position = worldPosition;
        message.text = text;

        // If a color is provided, use it; otherwise, use the default color of the text.
        message.color = color ?? message.color;

        damageCount++;
        if (damageCount >= objectCount)
        {
            damageCount = 0;
        }
    }

    public void PostSpecialAttackMessage(string text, Vector3 worldPosition, Color? color = null)
    {
        TMPro.TextMeshPro message = specialAttackMessagePool[specialAttackCount];
        message.gameObject.SetActive(true);
        message.transform.position = worldPosition;
        message.text = text;

        // If a color is provided, use it; otherwise, use the default color of the text.
        message.color = color ?? message.color;

        specialAttackCount++;
        if (specialAttackCount >= objectCount)
        {
            specialAttackCount = 0;
        }
    }
    public void DeactivateAllMessages()
    {
        foreach (var message in damageMessagePool)
        {
            message.gameObject.SetActive(false);
        }

        foreach (var specialMessage in specialAttackMessagePool)
        {
            specialMessage.gameObject.SetActive(false);
        }
    }
}