using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum MovementType { Static, Translation, Rotation }
    public Vector3 StartPosition => _startPosition;

    [SerializeField] MovementType _movementType;
    [SerializeField] float _obstacleMovementSpeed;

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
        _typeMovement.Move(transform, _obstacleMovementSpeed);
    }
}
