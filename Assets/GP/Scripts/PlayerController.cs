using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.LightAnchor;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    [Header("Speed parameters")]
    [SerializeField] float _startSpeedPlayer;
    [SerializeField] float _currentSpeedPlayer;
    float _currentMaxSpeedPlayer;
    bool _isAddingSpeed = false;

    public float SpeedPlayer => _currentSpeedPlayer;
    [SerializeField] float _speedRotation;

    Rigidbody _rb;
    [SerializeField] Animator _playerAnimator;
    int VelocityHash;
    float _groundCheckDistance = 1.5f;

    [Header("Wall Run")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _wallLayer;


    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;
    [SerializeField] EffectSystem _effectSystem;


    bool _isGrounded = false;
    Vector3 _currentGravityDirection = Vector3.down;
    Vector3 _targetGravityDirection = Vector3.down;
    RaycastHit _surfaceHit;
    float _gravityStrenght = 1;
    private float _velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


        _rb = GetComponent<Rigidbody>();
        Instance = this;
        _currentMaxSpeedPlayer = _startSpeedPlayer;
        _playerAnimator.SetTrigger("Run");
        _playerAnimator.Play("Run_Animation_Tree", 0, 0f);
        VelocityHash = Animator.StringToHash("Blend");
    }

    private void Update()
    {
        DetectWall();
        SetCurrentAnimation();
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
            if (Mathf.Abs(_currentSpeedPlayer - _currentMaxSpeedPlayer) >= 0.01f)
            {
                _currentSpeedPlayer += Time.deltaTime * 10;
                _cameraFollow.SetFieldOfview(_currentSpeedPlayer);
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
        if (_currentMaxSpeedPlayer > 100)
            _currentMaxSpeedPlayer = 100;

        return _currentMaxSpeedPlayer;
    }

    private void SetCurrentAnimation()
    {

        #region SetCurrentAnimationV1
        /*AnimatorStateInfo animatorState = _playerAnimator.GetCurrentAnimatorStateInfo(0);
        float progression = Mathf.Clamp01(_currentSpeedPlayer/100);

        _playerAnimator.Play(animatorState.fullPathHash, 0, progression);*/
        #endregion


        #region SetCurrentAnimationV2
        _playerAnimator.speed = _currentSpeedPlayer / 100;
        _playerAnimator.SetFloat(VelocityHash, _playerAnimator.speed*100);
        #endregion
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
            _effectSystem.DestroyActiveParticle();
            _effectSystem.UpdateEffect();
        }
        Debug.Log(MainGame.Instance.TransitionLayer.value);
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == MainGame.Instance.TransitionLayer.value)
        {
            Debug.Log("Transition !!!!");
            _cameraFollow.SetHasPassedDoorsGood();
        }

    }


}
