using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    public LayerMask WallLayer => _wallLayer;
    float _minSurfaceAngle = 45f;
    Vector3 _currentGravityDirection = Vector3.down;
    Vector3 _targetGravityDirection = Vector3.down;
    Vector3 _currentSurfaceNormal = Vector3.up;
    Vector3 _lockedSurfaceNormal = Vector3.up;
    bool _isGrounded = false;
    float _hasRotateDelay = 0f;

    [Header("Rotation for WallRun")]
    Quaternion _startRotation;
    Quaternion _targetRotation;
    float _rotationProgress;
    [SerializeField] AnimationCurve _rotationCurve;

    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;
    [SerializeField] EffectSystem _effectSystem;


    bool _isOnGround = true;

    RaycastHit _surfaceHit;
    RaycastHit[] _hit;
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

        //CheckGround();

        if (Physics.SphereCastAll(_cameraFollow.Target.position, 0.2f, -transform.up, 10, _wallLayer, QueryTriggerInteraction.Ignore).Length > 0)
        {
            _isOnGround = true;
        }
        else
            _isOnGround = false;

        if (_hasRotateDelay > 0f)
            _hasRotateDelay -= Time.deltaTime;
        else
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
            if (_currentSpeedPlayer < _currentMaxSpeedPlayer && Mathf.Abs(_currentSpeedPlayer - _currentMaxSpeedPlayer) >= 0.1f)
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
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, _currentSurfaceNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, _currentSurfaceNormal).normalized;

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

    public bool SetOnGround(bool value)
    {
        _isOnGround = value;
        return _isOnGround;
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
        _playerAnimator.SetFloat(VelocityHash, _playerAnimator.speed * 100);
        #endregion
    }

    #region WallMovement
    private void DetectWall()
    {

        if (_hasRotateDelay > 0)
            return;

        _isGrounded = false;

        Vector3[] WallPositions = new Vector3[]
        {
            _currentGravityDirection,
            transform.right,
            -transform.right
        };

        foreach (Vector3 wallPosition in WallPositions)
        {
            if (Physics.Raycast(transform.position, wallPosition, out _surfaceHit, _wallCheckDistance, _wallLayer))
            {
                float angle = Vector3.Angle(Vector3.up, _surfaceHit.normal);
                if (angle > _minSurfaceAngle || wallPosition == _currentGravityDirection)
                {
                    _isGrounded = true;
                    if (_hasRotateDelay <= 0)
                        _hasRotateDelay = 0.7f;
                    _currentSurfaceNormal = _surfaceHit.normal;
                    _targetGravityDirection = -_surfaceHit.normal;

                    Debug.DrawRay(_surfaceHit.point, _surfaceHit.normal * 2f, Color.green);
                    break;
                }
            }
        }
    }

    private void RotatePlayer()
    {
        if (_isGrounded)
        {
            Quaternion newTargetRotation = Quaternion.FromToRotation(transform.up, _currentSurfaceNormal) * transform.rotation;

            if (Quaternion.Angle(newTargetRotation, _targetRotation) > 5f)
            {
                _startRotation = transform.rotation;
                _targetRotation = newTargetRotation;
                _rotationProgress = 0f;
            }

            _rotationProgress += Time.deltaTime;
            _rotationProgress = Mathf.Clamp01(_rotationProgress);

            float curvedProgress = _rotationCurve.Evaluate(_rotationProgress);

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, curvedProgress);
        }
    }


    private void ApplyGravityForce()
    {
        if (_isOnGround)
            _gravityStrenght = 1f;
        else
            _gravityStrenght = 50f;
        _rb.AddForce(_currentGravityDirection * _gravityStrenght, ForceMode.Force);
    }
    #endregion



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

        if (MainGame.Instance.TransitionLayer.value == 1 << other.gameObject.layer)
        {
            _cameraFollow.SetHasPassedDoorsGood();
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_cameraFollow.Target.position - new Vector3(0, 0.5f, 0), 0.2f);
    }



}
