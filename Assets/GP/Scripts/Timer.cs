using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool IsPlaying
    {
        get => _isPlaying;
        set => _isPlaying = value;
    }
    public float CurrentTimeInSeconds
    {
        get => _currentTimeInSeconds;
        set => _currentTimeInSeconds = value;
    }

    int _lastSecond;
    float _currentTimeInSeconds;
    bool _isPlaying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isPlaying = true;
        _currentTimeInSeconds = 0;
        _lastSecond = 0;
        MainGame.Instance.UIManager.RefreshTimerDisplay((int)_currentTimeInSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        if (!MainGame.Instance.PlayerController.IsOnPause)
        {
            _currentTimeInSeconds += Time.deltaTime;
            if (_lastSecond != (int)_currentTimeInSeconds)
            {
                MainGame.Instance.UIManager.RefreshTimerDisplay((int)_currentTimeInSeconds);
                _lastSecond = (int)_currentTimeInSeconds;
            }
        }
    }
}
