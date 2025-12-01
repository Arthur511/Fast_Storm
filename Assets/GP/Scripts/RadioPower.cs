using UnityEngine;

public class RadioPower : IDevicePower
{

    [SerializeField] float _dashSpeed = 20f;

    public void ExecutePower(GameObject g)
    {
        Debug.Log("Exécution !!!!");
        g.GetComponent<Rigidbody>().AddForce(g.transform.forward * _dashSpeed,ForceMode.Impulse);
    }
}
