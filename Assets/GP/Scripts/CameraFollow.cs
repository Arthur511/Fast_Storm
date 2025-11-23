using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    float _speedCamera = 10f;

    Vector3 _velocity = Vector3.zero;
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _target.TransformPoint(_offset), ref _velocity, 1f/_speedCamera);

        transform.LookAt( _target );
    }
}
