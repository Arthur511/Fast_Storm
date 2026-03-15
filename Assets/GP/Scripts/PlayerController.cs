using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using static UnityEngine.LightAnchor;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    public float CurrentSpeedPlayer => _currentSpeedPlayer;
    public LayerMask WallLayer => _wallLayer;
    public Doors ActualNextDoor => _actualNextDoor;
    public int Score
    {
        get => _score;
        set => _score = value;
    }
    public Vector3 CurrentSurfaceNormal
    {
        get => _currentSurfaceNormal;
        set => _currentSurfaceNormal = value;
    }
    public Vector3 CurrentGravityDirection
    {
        get => _currentGravityDirection;
        set => _currentGravityDirection = value;
    }
    public bool IsInverting
    {
        get => _isInverting;
        set => _isInverting = value;
    }
    public bool IsOnPause
    {
        get => _isOnPause;
        set => _isOnPause = value;
    }


    [Header("Speed parameters")]
    [SerializeField] float _startSpeedPlayer;
    [SerializeField] float _currentSpeedPlayer;
    [SerializeField] float _lateralSpeed;
    [SerializeField] float _lowestSpeedPlayer;
    [SerializeField] float _highestSpeedPlayer;
    /*[Range(0.5f, 1.5f)]*/
    [SerializeField] float _speedRotation;
    [SerializeField] Animator _playerAnimator;

    [Header("Wall Run")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _wallLayer;

    [Header("Rotation for WallRun")]
    [SerializeField] AnimationCurve _rotationCurve;
    [SerializeField] LayerMask _cornerLayer;

    [Header("Scripts")]
    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] Energy _energy;
    [SerializeField] EffectSystem _effectSystem;
    [SerializeField] PowersManager _powersManager;
    [SerializeField] UIManager _uiManager;
    [SerializeField] List<Doors> _doors;

    [SerializeField] private CustomPassVolume _customPassVolume;

    [SerializeField] float _fallMultiplier;
    [SerializeField] float _jumpCutMultiplier;

    int _score = 0;

    float _currentMaxSpeedPlayer;
    bool _isAddingSpeed = false;
    bool _isLosingSpeed = false;

    Rigidbody _rb;
    int VelocityHash;
    float _groundCheckDistance = 1.5f;

    float _minSurfaceAngle = 45f;
    Vector3 _currentGravityDirection = Vector3.down;
    Vector3 _targetGravityDirection = Vector3.down;
    Vector3 _currentSurfaceNormal = Vector3.up;
    Vector3 _lastSurfaceNormal = Vector3.up;
    Vector3 _smoothedSurfaceNormal = Vector3.up;
    float _hasRotateDelay = 0f;
    Coroutine _delayCoroutine;
    bool _isRotating = false;
    bool _isOnGround = true;

    Quaternion _startRotation;
    Quaternion _targetRotation;
    float _rotationProgress;

    bool _isBackToStartRot = false;
    bool _isInverting = false;
    bool _isOnPause = false;

    List<Vector3> _groundPlan = new List<Vector3>
    {
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down
    };

    bool _lateralRotation = false;

    int _doorsIndex = 0;
    Doors _actualNextDoor;

    Material _speedLineMaterial;

    float _gravityStrenght = 1;

    RaycastHit _surfaceHit;
    RaycastHit[] _hit;
    float _velocity;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isOnPause = false;

        _currentMaxSpeedPlayer = _startSpeedPlayer;
        _playerAnimator.SetTrigger("Run");
        _playerAnimator.Play("Run_Animation_Tree", 0, 0f);
        VelocityHash = Animator.StringToHash("Blend");

        _doors[_doorsIndex].SetIsClosing(true);
        _actualNextDoor = _doors[_doorsIndex];

        var customPass = _customPassVolume.customPasses[0] as FullScreenCustomPass;
        if (customPass != null)
        {
            _speedLineMaterial = customPass.fullscreenPassMaterial;
        }
        _speedLineMaterial.SetFloat("_Alpha", 0f);
        _speedLineMaterial.SetFloat("_Mask_Size", 1f);

        _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
    }

    private void Update()
    {
        if (!_isOnPause)
        {
            float y = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.Q) && _isOnGround)
            {
                UsingJump();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                UsingLateralDash(new Vector3(y, 0, 0));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                UsingInvert();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                UsingPassThrough();
            }

            if (!_isRotating && !_isInverting)
            {
                if (Physics.SphereCastAll(_cameraFollow.Target.position, 0.2f, _currentGravityDirection, 1f, _wallLayer, QueryTriggerInteraction.Ignore).Length > 0)
                {
                    _isOnGround = true;
                    _isBackToStartRot = false;

                    if (_delayCoroutine != null)
                    {
                        StopCoroutine(_delayCoroutine);
                        _delayCoroutine = null;
                    }
                }
                else
                {
                    _isOnGround = false;
                    /*if (_delayCoroutine == null)
                        _delayCoroutine = StartCoroutine(DelayResetGravity());*/
                }
            }
            if (!_isRotating && !_isInverting)
                DetectWall(new Vector3(y, 0, 0));

            SetCurrentAnimation();

            //_lastSurfaceNormal = _currentSurfaceNormal;
        }
    }
    private void FixedUpdate()
    {
        if (!_isOnPause)
        {
            float y = Input.GetAxisRaw("Horizontal");
            Vector3 direction = new Vector3(y, 0, 0).normalized;

            _smoothedSurfaceNormal = Vector3.Slerp(_smoothedSurfaceNormal, _currentSurfaceNormal, Time.deltaTime * 20f);

            ApplyGravityForce();
            RotatePlayer();
            MoveCharacter(direction);

            if (_isOnGround)
                SnapToSurface();

            if (_isAddingSpeed)
            {
                if (_currentSpeedPlayer < _currentMaxSpeedPlayer && Mathf.Abs(_currentSpeedPlayer - _currentMaxSpeedPlayer) >= 0.01f)
                {
                    _currentSpeedPlayer += Time.deltaTime * 10;
                    _cameraFollow.SetFieldOfview(_energy.CurrentEnergy);
                }
                else
                    _isAddingSpeed = false;
            }
            else if (_isLosingSpeed)
            {
                if (_currentSpeedPlayer > _currentMaxSpeedPlayer && Mathf.Abs(_currentSpeedPlayer - _currentMaxSpeedPlayer) >= 0.01f)
                {
                    _currentSpeedPlayer -= Time.deltaTime * 10;
                    _cameraFollow.SetFieldOfview(_energy.CurrentEnergy);
                }
                else
                    _isLosingSpeed = false;
            }

            ChangeAlphaOfSpeedLine();

            _effectSystem.UpdateEffect();
        }
    }

    private void MoveCharacter(Vector3 direction)
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, _smoothedSurfaceNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, _smoothedSurfaceNormal).normalized;

        //_rb.AddForce(forward * _currentSpeedPlayer, ForceMode.Acceleration);

        float gravitySpeed = Vector3.Dot(_rb.linearVelocity, _currentGravityDirection);
        float targetLateralSpeed = direction.x * _lateralSpeed;

        _rb.linearVelocity = forward * (_currentSpeedPlayer * 10)
                           + right * targetLateralSpeed
                           + _currentGravityDirection * gravitySpeed;

    }

    private void UsingJump()
    {
        if (_energy.CurrentEnergy >= _powersManager.EnergyCost)
        {
            _powersManager.MakeJump(gameObject);
            _energy.CurrentEnergy -= _powersManager.EnergyCost;
            SetMaxSpeed(_energy.CurrentEnergy * (_highestSpeedPlayer - _lowestSpeedPlayer) / _energy.MaxEnergy + _lowestSpeedPlayer);
            _isLosingSpeed = true;
            _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
        }
    }
    private void UsingInvert()
    {
        if (_energy.CurrentEnergy >= _powersManager.EnergyCost)
        {
            _powersManager.MakeInvertTeleportation();
            _energy.CurrentEnergy -= _powersManager.EnergyCost;
            SetMaxSpeed(_energy.CurrentEnergy * (_highestSpeedPlayer - _lowestSpeedPlayer) / _energy.MaxEnergy + _lowestSpeedPlayer);
            _isLosingSpeed = true;
            _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
        }
    }

    private void UsingLateralDash(Vector3 lateralDirection)
    {
        if (_energy.CurrentEnergy >= _powersManager.EnergyCost)
        {
            _powersManager.MakeLateralDash(gameObject, lateralDirection);
            _energy.CurrentEnergy -= _powersManager.EnergyCost;
            SetMaxSpeed(_energy.CurrentEnergy * (_highestSpeedPlayer - _lowestSpeedPlayer) / _energy.MaxEnergy + _lowestSpeedPlayer);
            _isLosingSpeed = true;
            _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
        }
    }

    private void UsingPassThrough()
    {
        if (_energy.CurrentEnergy >= _powersManager.EnergyCost)
        {
            _powersManager.ActivePassThroughMode(gameObject);
            _energy.CurrentEnergy -= _powersManager.EnergyCost;
            SetMaxSpeed(_energy.CurrentEnergy * (_highestSpeedPlayer - _lowestSpeedPlayer) / _energy.MaxEnergy + _lowestSpeedPlayer);
            _isLosingSpeed = true;
            _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
        }
    }

    public float SetMaxSpeed(float amountToAdd)
    {
        _currentMaxSpeedPlayer = amountToAdd;
        if (_currentMaxSpeedPlayer > _highestSpeedPlayer)
            _currentMaxSpeedPlayer = _highestSpeedPlayer;
        else if (_currentMaxSpeedPlayer < _lowestSpeedPlayer)
            _currentMaxSpeedPlayer = _lowestSpeedPlayer;
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
                //Debug.DrawRay(_surfaceHit.point, _surfaceHit.normal * 2f, Color.green);
            }
        }
    }

    private void RotatePlayer()
    {
        Quaternion newTargetRotation = Quaternion.LookRotation(transform.forward, _currentSurfaceNormal);

        if (Quaternion.Angle(newTargetRotation, _targetRotation) > 0.5f)
        {
            _startRotation = transform.rotation;
            _targetRotation = newTargetRotation;
            _isRotating = true;
            _rotationProgress = 0f;
        }
        else
        {
            _isRotating = false;
            _isInverting = false;
        }

        _rotationProgress += Time.deltaTime;
        _rotationProgress = Mathf.Clamp01(_rotationProgress);

        float curvedProgress = _rotationCurve.Evaluate(_rotationProgress);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, curvedProgress);

    }
    private void BackToStartRotation()
    {
        _startRotation = transform.rotation;
        _targetRotation = Quaternion.Euler(0, 0, 0);
        _rotationProgress = 0f;
        _rotationProgress += Time.deltaTime * 2;
        _rotationProgress = Mathf.Clamp01(_rotationProgress);

        float curvedProgress = _rotationCurve.Evaluate(_rotationProgress);

        transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, curvedProgress);
    }

    private void SnapToSurface()
    {
        if (Physics.Raycast(transform.position, _currentGravityDirection, out RaycastHit hit, 1.5f, _wallLayer))
        {
            float distanceToSurface = hit.distance;

            if (distanceToSurface > 0.1f)
            {
                float correction = distanceToSurface - 0.1f;
                transform.position += _currentGravityDirection * correction;
            }
            else if (distanceToSurface < 0.1f * 0.5f)
            {
                float correction = 0.1f * 0.5f - distanceToSurface;
                transform.position -= _currentGravityDirection * correction;
            }
        }
    }

    private IEnumerator DelayResetGravity()
    {
        yield return new WaitForSeconds(0.1f);

        /*Vector3 right = Vector3.ProjectOnPlane(transform.right, _surfaceHit.normal).normalized;
        float lateralSpeed = Vector3.Dot(_rb.linearVelocity, right);
        _rb.linearVelocity = right * lateralSpeed;*/

        _currentGravityDirection = Vector3.down;
        _currentSurfaceNormal = Vector3.up;
        _isBackToStartRot = true;
        _delayCoroutine = null;

    }


    private void ApplyGravityForce()
    {

        bool isOnWall = _isOnGround && _currentSurfaceNormal != Vector3.up;

        if (_isOnGround)
            _gravityStrenght = isOnWall ? 0f : 0f;
        else
        {
            if (_rb.linearVelocity.y < 0f)
                _gravityStrenght = 50f * _fallMultiplier;
            else if (_rb.linearVelocity.y > 0f)
                _gravityStrenght = 50f * _jumpCutMultiplier;
            else
                _gravityStrenght = 50f;
        }
        _rb.AddForce(_currentGravityDirection * _gravityStrenght, ForceMode.Force);
    }
    #endregion


    private void ChangeAlphaOfSpeedLine()
    {
        float normalizedSpeed = _currentSpeedPlayer / 70f;
        _speedLineMaterial.SetFloat("_Alpha", normalizedSpeed);
        _speedLineMaterial.SetFloat("_Mask_Size", 1 - normalizedSpeed * 0.5f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ElectronicDevice>(out ElectronicDevice device))
        {
            if (!device.IsEmpty())
            {
                _energy.CurrentEnergy += device.EnergyToSend;
                _score += 100;
                SetMaxSpeed((_energy.CurrentEnergy * (_highestSpeedPlayer - _lowestSpeedPlayer)) / _energy.MaxEnergy + _lowestSpeedPlayer);
                _isAddingSpeed = true;
                device.DrainEnergy(device.EnergyToSend);
                if (device.DevicePower != null)
                    device.DevicePower.ExecutePower(gameObject);
                _uiManager.refreshEnergyJauge(_energy.CurrentEnergy, _energy.MaxEnergy);
            }
            _effectSystem.UpdateEffect();
        }

        if (_cornerLayer.value == 1 << other.gameObject.layer)
        {
            _lateralRotation = true;
        }

        if (MainGame.Instance.TransitionLayer.value == 1 << other.gameObject.layer)
        {
            _cameraFollow.SetHasPassedDoorsGood();
            MainGame.Instance.SaveSystem.SaveData();
            if (_doorsIndex < _doors.Count - 1)
                _doorsIndex++;
            _doors[_doorsIndex].SetIsClosing(true);
            _actualNextDoor = _doors[_doorsIndex];
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (_cornerLayer.value == 1 << other.gameObject.layer)
        {
            _lateralRotation = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (MainGame.Instance.ObstacleLayer.value == 1 << collision.gameObject.layer || MainGame.Instance.DoorLayer.value == 1 << collision.gameObject.layer)
        {
            StartCoroutine(DelayBeforeRestart());
        }
    }

    IEnumerator DelayBeforeRestart()
    {
        _effectSystem.DisplayDeathParticle(transform.position);
        GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        MainGame.Instance.SaveSystem?.LoadData();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_cameraFollow.Target.position, _currentGravityDirection);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_cameraFollow.Target.position - new Vector3(0, 0.5f, 0), 0.2f);
        //Ray hit = Physics.Raycast(MainGame.Instance.PlayerController.transform.position, MainGame.Instance.PlayerController.transform.up, 100, MainGame.Instance.WallLayer)
        //Gizmos.DrawLine(_cameraFollow.Target.position, _cameraFollow.Target.up * 100);
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
