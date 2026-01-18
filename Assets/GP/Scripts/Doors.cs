using UnityEngine;

public class Doors : MonoBehaviour
{

    Animator _doorsAnimator;

    [Header("Values")]
    [SerializeField] float _doorTimer;
    float _currentTime = 0;
    float _progression = 0;
    bool _isClosing = false;

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
    }

    public void SetIsClosing(bool isClosing)
    {
        _isClosing = isClosing;
    }

}
