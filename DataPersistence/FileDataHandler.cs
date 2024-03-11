using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        UnityEngine.Debug.Log("Attempting to load from: " + fullPath);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = File.ReadAllText(fullPath);
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error occurred when trying to load data from file " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        // Construct the path using the instance's dataDirPath and dataFileName
        string path = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // Convert the data object to JSON
            string jsonData = JsonUtility.ToJson(data, true);  // 'true' for pretty print

            // Write the JSON data to the file
            File.WriteAllText(path, jsonData);

            // Log the path to the console to know where it's attempting to save
            UnityEngine.Debug.Log("Data saved to: " + path);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error saving data: " + e.Message);
        }
    }


}