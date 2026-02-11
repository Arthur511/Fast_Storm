using UnityEngine;

public class Energy : MonoBehaviour
{
    public float CurrentEnergy
    {
        get { return _currentEnergy; }
        set { _currentEnergy = value; }
    }
    public int MaxEnergy { 
        get => _maxEnergy; 
        set { _maxEnergy = value; } 
    }
    
    [SerializeField]float _currentEnergy;

    int _maxEnergy;
    
    private void Awake()
    {
        _maxEnergy = 50;
    }

}
