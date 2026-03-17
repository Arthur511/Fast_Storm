using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum MovementType { Static, Translation, Chain, Rotation }
    public Vector3 StartPosition => _startPosition;
    public Transform RestartPointChainMovement => _restartPointChainMovement;
    public Transform LimitPointChainDistance => _limitPointChainMovement;
    //public BoxCollider TriggerForwardMovement => _triggerForwardMovement;

    [SerializeField] MovementType _movementType;
    [SerializeField] float _obstacleMovementSpeed;

    [SerializeField] Transform _restartPointChainMovement;
    [SerializeField] Transform _limitPointChainMovement;

    //[SerializeField] BoxCollider _triggerForwardMovement;

    private IObstacleMovement _typeMovement;
    private Vector3 _startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;

        switch (_movementType)
        {
            case MovementType.Static:
                break;
            case MovementType.Translation:
                _typeMovement = new TranslationMovement();
                break;
            case MovementType.Chain:
                _typeMovement = new ChainMovement();
                break;
            case MovementType.Rotation:
                _typeMovement = new RotationMovement();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_typeMovement != null)
        {
            _typeMovement.Move(transform, _obstacleMovementSpeed);
        }
    }
}
