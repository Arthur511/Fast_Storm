using UnityEngine;

public class Doors : MonoBehaviour
{
    public float Progression
    {
        get => _progression;
        set => _progression = value;
    }
    public bool IsClosing
    {
        get => _isClosing;
        set => _isClosing = value;
    }

    [Header("Values")]
    [SerializeField] float _doorTimer;
    float _currentTime = 0;
    float _progression = 0;
    bool _isClosing = false;

    Animator _doorsAnimator;

    private void Awake()
    {
        _doorsAnimator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _doorsAnimator.Play("Doors", 0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isClosing)
            CloseDoor();
    }
    private void CloseDoor()
    {
        if (_progression < 1)
        {
            _currentTime += Time.deltaTime;
            _progression = _currentTime / _doorTimer;
            AnimatorStateInfo animatorState = _doorsAnimator.GetCurrentAnimatorStateInfo(0);
            _doorsAnimator.Play(animatorState.fullPathHash, 0, _progression);
        }
        else
        {
            _currentTime = 0;
            _isClosing = false;
        }
    }
    public void SetIsClosing(bool value)
    {
        _isClosing = value;
    }

}
