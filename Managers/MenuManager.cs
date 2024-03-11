using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private DataContainer dataContainer;

    void Start()
    {
        if (DataPersistenceManager.instance == null)
        {
            UnityEngine.Debug.Log("DataPersistenceManager instance is still null in Start().");
        }
        else
        {
            UnityEngine.Debug.Log("DataPersistenceManager instance found in Start().");
            DataPersistenceManager.instance.RegisterDataPersistenceObject(dataContainer);
            DataPersistenceManager.instance.LoadGame();

            // Update your menu UI...
        }
    }


}
