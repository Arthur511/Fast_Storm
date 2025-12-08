using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.LightAnchor;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    [SerializeField] float _startSpeedPlayer;
    [SerializeField] float _currentSpeedPlayer;
    float _currentMaxSpeedPlayer;
    bool _isAddingSpeed = false;
    public float SpeedPlayer => _currentSpeedPlayer;
    [SerializeField] float _speedRotation;

    Rigidbody _rb;
    float _groundCheckDistance = 1.5f;

    [Header("Wall Run")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _wallLayer;

    Vector3 _wallRunDirection;
    bool _isRunningOnWall = false;


    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;


    bool _isGrounded = false;
    Vector3 _currentGravityDirection = Vector3.down;
    Vector3 _targetGravityDirection = Vector3.down;
    RaycastHit _surfaceHit;
    float _gravityStrenght = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Instance = this;
        _currentMaxSpeedPlayer = _startSpeedPlayer;
    }

    private void Update()
    {
        DetectWall();
    }

    private void FixedUpdate()
    {
        float y = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(y, 0, 0).normalized;

        ApplyGravityForce();
        RotatePlayer();
        MoveCharacter(direction);

        if (_isAddingSpeed)
        {
            if (Mathf.Abs(_currentSpeedPlayer - _currentMaxSpeedPlayer) >= 0.1f)
            {
                _currentSpeedPlayer = Mathf.Lerp(_currentSpeedPlayer, _currentMaxSpeedPlayer, Time.deltaTime/2f);
                _cameraFollow.SetFieldOfview(_currentSpeedPlayer);
                _cameraFollow.SetCameraSpeed(_currentSpeedPlayer);
                _cameraFollow.SetZAxisOfCamera(_currentSpeedPlayer);
            }
            else
                _isAddingSpeed = false;
        }
    }

    private void MoveCharacter(Vector3 direction)
    {

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, -_currentGravityDirection).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, -_currentGravityDirection).normalized;

        Vector3 relativeMovement = forward + (right * direction.x);

        _rb.AddForce(relativeMovement * _currentSpeedPlayer, ForceMode.Force);

    }
    private void Rotation(Vector3 dir)
    {
        transform.Rotate(Vector3.up, dir.x * _speedRotation * Time.deltaTime);
    }

    public float SetMaxSpeed(float amountToAdd)
    {
        _currentMaxSpeedPlayer += amountToAdd;
        return _currentMaxSpeedPlayer;
    }

    private void DetectWall()
    {
        _isGrounded = false;

        if (Physics.Raycast(transform.position, _currentGravityDirection, out _surfaceHit, _wallCheckDistance, _wallLayer))
        {
            _isGrounded = true;
            _targetGravityDirection = -_surfaceHit.normal;
        }
        else if (Physics.Raycast(transform.position, transform.right, out _surfaceHit, 2f, _wallLayer))
        {
            Debug.Log("Droite");
            _isGrounded = true;
            _targetGravityDirection = -_surfaceHit.normal;
        }
        else if (Physics.Raycast(transform.position, -transform.right, out _surfaceHit, 2f, _wallLayer))
        {
            Debug.Log("Gauche");
            _isGrounded = true;
            _targetGravityDirection = -_surfaceHit.normal;
        }
        else
        {
            _targetGravityDirection = Vector3.down;
        }

        _currentGravityDirection = Vector3.Lerp(_currentGravityDirection, _targetGravityDirection, Time.deltaTime * 10).normalized;

    }

    private void RotatePlayer()
    {
        if (_isGrounded)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _currentGravityDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 500);
        }
    }


    private void ApplyGravityForce()
    {
        _rb.AddForce(_currentGravityDirection * _gravityStrenght, ForceMode.Acceleration);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ElectronicDevice>(out ElectronicDevice device))
        {
            if (!device.IsEmpty())
            {
                _energy.SetEnergy(device.EnergyToSend);
                SetMaxSpeed(_energy.CurrentEnergy);
                _isAddingSpeed = true;
                device.DrainEnergy(device.EnergyToSend);
                if (device.DevicePower != null)
                    device.DevicePower.ExecutePower(gameObject);
            }
        }
    }





    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Raycast de gravité
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, _currentGravityDirection * _groundCheckDistance);

        // Raycast avant (wall)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _wallCheckDistance);

        // Normale de surface
        if (_isGrounded)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_surfaceHit.point, _surfaceHit.normal * 2f);
        }
    }


}
