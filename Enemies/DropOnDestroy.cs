using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDestroy : MonoBehaviour
{
    [SerializeField] List<GameObject> dropItemPrefab;
    [SerializeField] [Range(0f, 1f)] float chance = 1f;

    bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void CheckDrop()
    {

        if (isQuitting) { return; }

        if(dropItemPrefab.Count <= 0)
        {
            Debug.LogWarning("List of drop items is empty");
            return;
        }

        if (Random.value < chance)
        {
            GameObject toDrop = dropItemPrefab[Random.Range(0, dropItemPrefab.Count)];

            if (toDrop == null) 
            {
                Debug.LogWarning("DropOnDestroy reference to dropped item is null");
                return;
            }

            SpawnManager.instance.SpawnObject(transform.position, toDrop);
           // Transform t = Instantiate(toDrop).transform;
           // t.position = transform.position;
        }
    }
}
