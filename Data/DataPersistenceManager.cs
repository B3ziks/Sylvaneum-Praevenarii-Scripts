using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private float autoSaveInterval = 60.0f; // save every minute

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.Log("more than one persistence manager in the scene");
        }
        instance = this;
       // DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
        UnityEngine.Debug.Log("Total IDataPersistence objects found: " + dataPersistenceObjects.Count);

        // Start auto save functionality
        InvokeRepeating("SaveGame", autoSaveInterval, autoSaveInterval);
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void RegisterDataPersistenceObject(IDataPersistence obj)
    {
        if (!dataPersistenceObjects.Contains(obj))
        {
            dataPersistenceObjects.Add(obj);
            UnityEngine.Debug.Log("Registered object: " + obj.GetType().Name);
        }
    }
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            UnityEngine.Debug.Log("No data was found");
            // Handle the case when no saved data is found. For example, initialize with defaults.
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        UnityEngine.Debug.Log("Number of upgrades in gameData before saving to file: " + gameData.upgrades.Count);

        //save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        CancelInvoke("SaveGame"); // Stop autosave before quitting
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

   
}