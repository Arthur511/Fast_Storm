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
    float _hasRotateDelay = 0f;
    Coroutine _delayCoroutine;



    [Header("Rotation for WallRun")]
    Quaternion _startRotation;
    Quaternion _targetRotation;
    float _rotationProgress;
    [SerializeField] AnimationCurve _rotationCurve;
    bool _isBackToStartRot = false;


    bool _lateralRotation = false;
    [SerializeField] LayerMask _cornerLayer;

    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;
    [SerializeField] EffectSystem _effectSystem;
    [SerializeField] List<Doors> _doors;
    int _doorsIndex = 0;


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

        _doors[_doorsIndex].SetIsClosing(true);
    }

    private void Update()
    {
        float y = Input.GetAxisRaw("Horizontal");
        Debug.Log(_currentGravityDirection);
        if (Physics.SphereCastAll(_cameraFollow.Target.position, 0.25f, -transform.up, 1f, _wallLayer, QueryTriggerInteraction.Ignore).Length > 0)
        {
            _isOnGround = true;
            _isBackToStartRot = false;
        }
        else
        {
            _isOnGround = false;
            if (_delayCoroutine == null)
                _delayCoroutine = StartCoroutine(DelayResetGravity());
            //_isBackToStartRot = true; 
        }

        DetectWall(new Vector3(y, 0, 0));

        SetCurrentAnimation();

        if (_lateralRotation)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) //Gauche
            {
                transform.Rotate(Vector3.up, -90);
                _lateralRotation = false;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1)) // Droite 
            {
                transform.Rotate(Vector3.up, 90);
                _lateralRotation = false;
            }
        }

    }

    private void FixedUpdate()
    {
        float y = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(y, 0, 0).normalized;

        ApplyGravityForce();
        if (!_isBackToStartRot)
            RotatePlayer();
        else
            BackToStartRotation();
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

        _rb.AddForce(forward * _currentSpeedPlayer, ForceMode.Acceleration);

        transform.position += (right * direction.x) * 0.5f;

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
        _playerAnimator.SetFloat(VelocityHash, _playerAnimator.speed * 100);
        #endregion
    }

    #region WallMovement
    private void DetectWall(Vector3 direction)
    {

        if (_hasRotateDelay > 0)
            return;

        Vector3 wallDetection = direction.x > 0 ? _cameraFollow.Target.right : -_cameraFollow.Target.right;

        if (Physics.Raycast(_cameraFollow.Target.position, wallDetection, out _surfaceHit, _wallCheckDistance, _wallLayer, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(_cameraFollow.Target.up, _surfaceHit.normal);
            if (angle > _minSurfaceAngle || wallDetection == _currentGravityDirection)
            {
                _currentSurfaceNormal = _surfaceHit.normal;
                _currentGravityDirection = -_surfaceHit.normal;

                Debug.DrawRay(_surfaceHit.point, _surfaceHit.normal * 2f, Color.green);
            }
        }
    }

    private void RotatePlayer()
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
    private void BackToStartRotation()
    {
        _startRotation = transform.rotation;
        _targetRotation = _targetRotation = Quaternion.Euler(0, 0, 0);
        _rotationProgress = 0f;
        _rotationProgress += Time.deltaTime * 2;
        _rotationProgress = Mathf.Clamp01(_rotationProgress);

        float curvedProgress = _rotationCurve.Evaluate(_rotationProgress);

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, curvedProgress);
    }


    private IEnumerator DelayResetGravity()
    {
        yield return new WaitForSeconds(0.75f);
        _currentGravityDirection = Vector3.down;
        _currentSurfaceNormal = Vector3.up;
        _isBackToStartRot = true;

        _delayCoroutine = null;
    }

    private void ApplyGravityForce()
    {
        if (_isOnGround)
            _gravityStrenght = 0f;
        else
            _gravityStrenght = 150f;
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


        if (_cornerLayer.value == 1 << other.gameObject.layer)
        {
            _lateralRotation = true;
        }

        if (MainGame.Instance.TransitionLayer.value == 1 << other.gameObject.layer)
        {
            _cameraFollow.SetHasPassedDoorsGood();
            if (_doorsIndex != _doors.Count)
                _doorsIndex++;
            _doors[_doorsIndex].SetIsClosing(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (_cornerLayer.value == 1 << other.gameObject.layer)
        {
            _lateralRotation = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_cameraFollow.Target.position - new Vector3(0, 0.5f, 0), 0.2f);
    }

    #region Obsolete
    private void Rotation(Vector3 dir)
    {
        transform.Rotate(Vector3.up, dir.x * _speedRotation * Time.deltaTime);
    }

    public bool SetOnGround(bool value)
    {
        _isOnGround = value;
        return _isOnGround;
    }
    #endregion

}
