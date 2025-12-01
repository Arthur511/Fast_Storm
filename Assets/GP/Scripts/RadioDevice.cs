using UnityEngine;

public class RadioDevice : ElectronicDevice
{
    private void Awake()
    {
        _devicePower = new RadioPower();
    }

}
