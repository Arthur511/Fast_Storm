using UnityEditor.Rendering;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform _target;
    float _currentOffsetZ;
    Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    float _speedCamera = 5f;

    Vector3 _velocity = Vector3.zero;
    float _velocityCam;

    [SerializeField] float _rotationSpeed = 5f;
    private Quaternion _targetRotation;

    [Header("Paramétres de distances de camera")]
    private float _baseDistance;

    [Header("Paramétres de FieldOfView de camera")]
    private float _baseFOV;
    [SerializeField] private float _fovSmoothSpeed = 5f;

    private float _currentPlayerSpeed;

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        _baseDistance = transform.localPosition.z;
        _mainCamera.fieldOfView = 100f;
        _baseFOV = _mainCamera.fieldOfView;
    }
    


    // Update is called once per frame
    void LateUpdate()
    {
        float newDist = UpdateVertigoDistance(_mainCamera.fieldOfView);

        float targetPosition = newDist;
        transform.localPosition = new Vector3(0, 3, Mathf.Lerp(transform.localPosition.z, targetPosition, Time.deltaTime * 0.5f));

        transform.LookAt( _target.position + Vector3.up );
    }

    
    float UpdateVertigoDistance(float currentFOV)
    {
        float baseTan = Mathf.Tan(_baseFOV *0.5f * Mathf.Deg2Rad);
        float currentTan = Mathf.Tan(currentFOV * 0.5f* Mathf.Deg2Rad);

        return _baseDistance * (baseTan/currentTan);
    }

    public void SetFieldOfview(float energy)
    {
        float targetFOV = Mathf.Clamp(_mainCamera.fieldOfView - energy * 0.05f, 0f, 100f);
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime*2f);
    }


    #region Obsolète

    public void SetCameraSpeed(float speed)
    {
        _speedCamera = speed;
    }

    public void SetZAxisOfCamera(float speed)
    {
        float targetZ = Mathf.Lerp(-10, -5, speed / 100);
        _currentOffsetZ = Mathf.Lerp(_currentOffsetZ, targetZ, Time.deltaTime*0.5f);
        //_offset.z = _currentOffsetZ;
    }
    #endregion
}
