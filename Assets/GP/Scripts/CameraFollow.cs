using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset = new Vector3(0, 2, -10);
    float _currentOffsetZ;
    Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    float _speedCamera = 5f;

    Vector3 _velocity = Vector3.zero;

    [SerializeField] float _rotationSpeed = 5f;
    private Quaternion _targetRotation;

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }
    


    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 goodStatement = _target.position + _target.rotation * _offset; 

        transform.position = Vector3.SmoothDamp(transform.position, goodStatement, ref _velocity, 0.3f);

        //transform.LookAt( _target );
    }

    /*void AlignCameraWithPlayer()
    {
        // Calcule la rotation cible pour que la caméra soit alignée avec le "haut" du joueur
        Vector3 directionToTarget = _target.position - transform.position;

        if (directionToTarget != Vector3.zero)
        {
            // Rotation pour regarder le joueur
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            // Aligne le "haut" de la caméra avec le "haut" du joueur
            Vector3 cameraUp = _target.up;
            _targetRotation = Quaternion.LookRotation(directionToTarget, cameraUp);

            // Applique la rotation progressivement
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }*/

    public void SetFieldOfview(float energy)
    {
        float targetFOV = Mathf.Clamp(_mainCamera.fieldOfView - energy * 0.05f, 20f, 100f);
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime * 2);
    }

    public void SetCameraSpeed(float speed)
    {
        _speedCamera = speed;
    }

    public void SetZAxisOfCamera(float speed)
    {
        float targetZ = Mathf.Lerp(-10, -5, speed / 100);
        _currentOffsetZ = Mathf.Lerp(_currentOffsetZ, targetZ, Time.deltaTime*0.5f);
        _offset.z = _currentOffsetZ;
    }

}
