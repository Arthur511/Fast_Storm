using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Image _energyJauge;

    [SerializeField] Image _scorePanel;
    [SerializeField] TextMeshProUGUI _scoreAmountText;

    [SerializeField] TextMeshProUGUI _timerText;

    public void refreshEnergyJauge(float currentEnergy, int maxEnergy)
    {
        _energyJauge.fillAmount = currentEnergy / maxEnergy;
    }

    public void DisplayScorePanel(int score)
    {

        _scorePanel.gameObject.SetActive(true);
        StartCoroutine(IncreaseToResult(score));
    }

    IEnumerator IncreaseToResult(int score)
    {
        float elapsed = 0f;

        while (elapsed < 2)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / 2);

            float eased = 1f - Mathf.Pow(1f - progress, 3f);

            int current = Mathf.RoundToInt(Mathf.Lerp(0, score, eased));
            _scoreAmountText.text = current.ToString();

            yield return null;
        }
        _scoreAmountText.text = score.ToString();
    }

    public void RefreshTimerDisplay(int time)
    {
        _timerText.text = string.Format("{0:00} : {1:00}", (time / 60), (time%60));
    }

}
