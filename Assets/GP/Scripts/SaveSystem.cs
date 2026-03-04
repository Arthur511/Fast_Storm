using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    [SerializeField] PlayerController controller;
    [SerializeField] List<ElectronicDevice> _electronicDevices;

    public void SaveData()
    {
        GameData gameData = new GameData()
        {
            PlayerPosition = controller.transform.position,
            EnergyInStock = controller.gameObject.GetComponent<Energy>().CurrentEnergy,
        };

        string jsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + "/SaveData.json";
        System.IO.File.WriteAllText(filePath, jsonData);

    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/SaveData.json";
        string jsonData = System.IO.File.ReadAllText(filePath);
        GameData gameData = JsonUtility.FromJson<GameData>(jsonData);


        controller.transform.position = gameData.PlayerPosition;
        controller.gameObject.GetComponent<Energy>().CurrentEnergy = gameData.EnergyInStock;
        controller.GetComponentInChildren<MeshRenderer>().enabled = true;
        foreach (ElectronicDevice device in _electronicDevices)
        {
            device.ResetEnergy();
        }
    }
}

[System.Serializable]
public class GameData
{
    public Vector3 PlayerPosition;
    public float EnergyInStock;
}
