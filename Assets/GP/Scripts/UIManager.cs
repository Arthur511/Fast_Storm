using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Image _energyJauge;

    public void refreshEnergyJauge(float currentEnergy, int maxEnergy)
    {
        _energyJauge.fillAmount = currentEnergy/maxEnergy;
    }

}
