using UnityEngine;

public class VRCDevice : ElectronicDevice
{
    private void Awake()
    {
        _devicePower = new VRCPower();
    }

}
