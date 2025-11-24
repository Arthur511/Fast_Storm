using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{
    [SerializeField] float _naturalDrain;
    [SerializeField] float _energyToSend;
    public float EnergyToSend => _energyToSend;

    float _drainTimer = 3f;
    float _currentDrainTimer;

    private void Start()
    {
        _currentDrainTimer = _drainTimer;
    }


    private void Update()
    {
        if (!IsEmpty())
        {
            _currentDrainTimer -= Time.deltaTime;
            if (_currentDrainTimer <= 0)
            {
                DrainEnergy(_naturalDrain);
                _currentDrainTimer = _drainTimer;
            }
        }
    }

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
