using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    float _speedCamera = 100f;

    Vector3 _velocity = Vector3.zero;
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 goodStatement = _target.position + _target.rotation * _offset; 

        transform.position = Vector3.Lerp(transform.position,goodStatement, _speedCamera*Time.deltaTime);

        transform.LookAt( _target );
    }
}
