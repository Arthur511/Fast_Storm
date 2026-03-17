using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] List<ElectronicDevice> _electronicDevices;
    [SerializeField] List<GameObject> _transporters;

    private void Start()
    {
        SaveData();
    }

    public void SaveData()
    {
        GameData gameData = new GameData()
        {
            PlayerPosition = controller.transform.position,
            StartPlayerSectionSpeed = controller.CurrentSpeedPlayer,
            EnergyInStock = controller.gameObject.GetComponent<Energy>().CurrentEnergy,
            Score = controller.Score,
            Timer = MainGame.Instance.TimerScript.CurrentTimeInSeconds,
            ActualNextdoor = controller.ActualNextDoor,
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
        controller.CurrentSpeedPlayer = gameData.StartPlayerSectionSpeed;
        controller.gameObject.GetComponent<Energy>().CurrentEnergy = gameData.EnergyInStock;
        controller.Score = gameData.Score;
        MainGame.Instance.TimerScript.CurrentTimeInSeconds = gameData.Timer;
        controller.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        foreach (ElectronicDevice device in _electronicDevices)
        {
            device.ResetEnergy();
        }
        foreach (GameObject transporter in _transporters)
        {
            transporter.SetActive(true);
            transporter.GetComponentInChildren<TransporterObstacle>().ResetTransportersPosition();
        }
        controller.ActualNextDoor.ResetDoors();
        controller.CameraFollow.SetFieldOfview(controller.gameObject.GetComponent<Energy>().CurrentEnergy);

        MainGame.Instance.UIManager.RefreshTimerDisplay((int)MainGame.Instance.TimerScript.CurrentTimeInSeconds);
        MainGame.Instance.UIManager.refreshEnergyJauge(controller.gameObject.GetComponent<Energy>().CurrentEnergy, controller.gameObject.GetComponent<Energy>().MaxEnergy);


    }
}

[System.Serializable]
public class GameData
{
    public Vector3 PlayerPosition;
    public float StartPlayerSectionSpeed;
    public float EnergyInStock;
    public int Score;
    public float Timer;
    public Doors ActualNextdoor;
}
