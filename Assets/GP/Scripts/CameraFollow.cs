using UnityEditor.Rendering;
using UnityEngine.Rendering.HighDefinition;

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    public Transform Target => _target;
    public Camera MainCamera => _mainCamera;
    
    
    [SerializeField] Transform _target;
    [SerializeField] float _rotationSpeed = 5f;
    
    [SerializeField] private float _fovSmoothSpeed = 5f;

    [SerializeField] float _transitionDuration;
    [SerializeField] AnimationCurve _velocityTransitionCurve;

    
    float _currentOffsetZ;
    Camera _mainCamera;
    float _speedCamera = 5f;

    Vector3 _velocity = Vector3.zero;
    float _velocityCam;

    private Quaternion _targetRotation;

    private float _baseDistance;

    private float _minFOV;
    private float _maxFOV;

    private float _currentPlayerSpeed;
    private bool _hasPassedDoors;
    private float _transitionTimer;
    private float _startPosition;


    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        _baseDistance = transform.localPosition.z;
        _mainCamera.fieldOfView = 100f;
        _maxFOV = 100;
        _minFOV = 20;
    }


    void LateUpdate()
    {
        /*float newDist = UpdateVertigoDistance(_mainCamera.fieldOfView);

        float targetPosition = newDist;
        transform.localPosition = new Vector3(0, 3, Mathf.Lerp(transform.localPosition.z, targetPosition, Time.deltaTime * 0.5f));*/

        if (_hasPassedDoors)
        {
            _transitionTimer += Time.deltaTime;
            float normalizedTimer = _transitionTimer / _transitionDuration;

            if (normalizedTimer <= 1f)
            {
                float t  = _velocityTransitionCurve.Evaluate(normalizedTimer);
                float Z = Mathf.Lerp(_startPosition, _startPosition - 5, t);
                transform.localPosition = new Vector3(0, 3, Z);
            }
            else
                _hasPassedDoors = false;
        }

        //transform.LookAt(_target.position + Vector3.up);
    }


    float UpdateVertigoDistance(float currentFOV)
    {
        float baseTan = Mathf.Tan(_minFOV * 0.5f * Mathf.Deg2Rad);
        float currentTan = Mathf.Tan(currentFOV * 0.5f * Mathf.Deg2Rad);

        return _baseDistance * (baseTan / currentTan);
    }

    public void SetFieldOfview(float energy)
    {
        float targetFOV = _maxFOV - energy;
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime);
    }

    public void SetHasPassedDoorsGood()
    {

        _hasPassedDoors = true;
        _startPosition = transform.localPosition.z;
        _transitionTimer = 0f;
    }


    #region Obsolète

    public void SetCameraSpeed(float speed)
    {
        _speedCamera = speed;
    }

    public void SetZAxisOfCamera(float speed)
    {
        float targetZ = Mathf.Lerp(-10, -5, speed / 100);
        _currentOffsetZ = Mathf.Lerp(_currentOffsetZ, targetZ, Time.deltaTime * 0.5f);
        //_offset.z = _currentOffsetZ;
    }
    #endregion
}
