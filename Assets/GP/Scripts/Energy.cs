using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField]float _currentEnergy;
    public float CurrentEnergy => _currentEnergy;

    public float SetEnergy(float amount)
    {
        _currentEnergy += amount;
        return _currentEnergy;
    }
}
