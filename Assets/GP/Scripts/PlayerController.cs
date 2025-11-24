using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    [SerializeField] float _startSpeedPlayer;
    [SerializeField] float _currentSpeedPlayer;
    public float SpeedPlayer => _currentSpeedPlayer;
    [SerializeField] float _speedRotation;

    Rigidbody _rb;

    [Header("Wall Run")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _wallLayer;

    Vector3 _wallRunDirection;
    bool _isRunningOnWall = false;


    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Instance = this;
    }

    private void FixedUpdate()
    {
        //float x = Input.GetAxisRaw("Horizontal");
        float x = Input.GetAxisRaw("Horizontal");
        Vector3 dirRotation = new Vector3(x, 0, 0);
        Rotation(dirRotation);

        float y = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0, 0, y).normalized;
        if (Physics.Raycast(transform.position, transform.forward, _wallCheckDistance, _wallLayer))
        {
            _isRunningOnWall = true;
        }
        else
        {
            _isRunningOnWall = false;
        }
        if (direction.magnitude >= 0.001f)
        {

            if (_isRunningOnWall)
            {
                MoveCharacterOnWall();
            }
            else
            {
                MoveCharacter(direction);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveCharacter(Vector3 direction)
    {
        Vector3 forward = _cameraFollow.MainCamera.transform.forward;
        Vector3 right = _cameraFollow.MainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;
        Vector3 forwardRelative = direction.z * forward;
        Vector3 rightRelative = direction.x * right;

        Vector3 relativeMovement = forwardRelative + rightRelative;

        _rb.AddForce(relativeMovement * _currentSpeedPlayer, ForceMode.Force);
    }
    private void MoveCharacterOnWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), transform.forward, out hit, _wallCheckDistance, _wallLayer))
        {
            Vector3 wallTangent = Vector3.Cross(hit.normal, Vector3.up).normalized;
            float inputX = Input.GetAxisRaw("Horizontal");

            Vector3 direction = (wallTangent * inputX).normalized;
            _rb.AddForce(direction * (_currentSpeedPlayer*2) + Vector3.up * (_currentSpeedPlayer*2), ForceMode.Force);

        }

    }

    private void Rotation(Vector3 dir)
    {
        transform.Rotate(Vector3.up, dir.x * _speedRotation * Time.deltaTime);
    }

    public float SetSpeed(float amountToAdd)
    {
        _currentSpeedPlayer += amountToAdd;
        return _currentSpeedPlayer;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ElectronicDevice>(out ElectronicDevice device))
        {
            if (!device.IsEmpty())
            {
                _energy.SetEnergy(device.EnergyToSend);
                _currentSpeedPlayer = _startSpeedPlayer + _energy.CurrentEnergy * 0.5f;
                device.DrainEnergy(device.EnergyToSend);
            }
        }
    }

}
