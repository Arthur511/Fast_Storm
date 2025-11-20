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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        Rotation();

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
            MoveCharacter();
            _rb.useGravity = true;
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveCharacter()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, y).normalized;
        _translation = direction;
        _rb.AddForce(direction * _speedPlayer, ForceMode.Acceleration);
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

    private void Rotation()
    {
        _targetRotation = Quaternion.LookRotation(_translation, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _speedRotation * Time.deltaTime);
    }
}
