using UnityEngine;

public class Energy : MonoBehaviour
{
    public float CurrentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            _currentEnergy = Mathf.Min(value, _maxEnergy);
        }
    }
    public int MaxEnergy
    {
        get => _maxEnergy;
        set { _maxEnergy = value; }
    }

    [SerializeField] int _maxEnergy;

    float _currentEnergy;
}
