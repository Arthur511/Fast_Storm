using UnityEngine;

public class VRCPower : IDevicePower
{

    [SerializeField] float _bumperSpeed = 30f;

    public void ExecutePower(GameObject g)
    {
        g.GetComponent<Rigidbody>().AddForce(g.transform.up * _bumperSpeed,ForceMode.Impulse);
    }
}
