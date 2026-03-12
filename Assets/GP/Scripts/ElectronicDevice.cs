using UnityEngine;

public class ElectronicDevice : MonoBehaviour
{
    public float EnergyToSend => _energyToSend;
    public IDevicePower DevicePower => _devicePower;
    
    [SerializeField] float _naturalDrain;
    [SerializeField] float _energyToSend;
    
    protected IDevicePower _devicePower;

    float _drainTimer = 3f;
    float _currentDrainTimer;
    float _startEnergy;


    private void Start()
    {
        _startEnergy = _energyToSend;
        _currentDrainTimer = _drainTimer;
    }


    protected void Update()
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

    public void ResetEnergy()
    {
        _energyToSend = _startEnergy;
    }
}
