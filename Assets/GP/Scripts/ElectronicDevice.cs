using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{

    [SerializeField] float _energyToSend;
    public float EnergyToSend => _energyToSend;

    public void DrainEnergy(float amount)
    {
        if (EnergyToSend < amount) amount = EnergyToSend;
        _energyToSend -= amount;
    }

    public bool IsEmpty()
    {
        return _energyToSend <= 0;
    }
}
