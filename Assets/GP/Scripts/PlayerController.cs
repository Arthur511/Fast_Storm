using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float _speedPlayer;
    [SerializeField] float _speedRotation;

    Rigidbody _rb;

    [Header("Wall Run")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _wallLayer;

    Vector3 _wallRunDirection;
    bool _isRunningOnWall = false;

    Quaternion _targetRotation;
    Vector3 _translation;

    [SerializeField] CameraFollow _cameraFollow;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //float x = Input.GetAxisRaw("Horizontal");
        float x = Input.GetAxisRaw("Horizontal");
        Vector3 dirRotation = new Vector3(x, 0, 0);
        Rotation(dirRotation);

        float y = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0, 0, y).normalized;
        if (direction.magnitude >= 0.001f)
        {
            if (Physics.Raycast(transform.position, transform.forward, _wallCheckDistance, _wallLayer))
            {
                _isRunningOnWall = true;
            }
            else
            {
                _isRunningOnWall = false;
            }

            if (_isRunningOnWall)
            {
                MoveCharacterOnWall();
                _rb.useGravity = false;
            }
            else
            {
                MoveCharacter(direction);
                _rb.useGravity = true;
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

        _rb.AddForce(relativeMovement * _speedPlayer, ForceMode.Acceleration);
    }
    private void MoveCharacterOnWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _wallCheckDistance, _wallLayer))
        {
            Vector3 wallTangent = Vector3.Cross(hit.normal, Vector3.up).normalized;
            float inputX = Input.GetAxisRaw("Horizontal");

            Vector3 direction = (wallTangent * inputX).normalized;
            _translation = direction;
            _rb.AddForce(direction * _speedPlayer + Vector3.up * _speedPlayer, ForceMode.Acceleration);

        }

    }

    private void Rotation(Vector3 dir)
    {
        transform.Rotate(Vector3.up, dir.x * _speedRotation * Time.deltaTime);
    }
}
