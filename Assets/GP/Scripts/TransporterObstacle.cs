using UnityEngine;

public class TransporterObstacle : MonoBehaviour
{
    [SerializeField] GameObject _transporter;
    [SerializeField] float _speedTransporter;

    bool _isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
            _transporter.transform.position -= new Vector3(0, 0, _speedTransporter * Time.deltaTime);
        if (MainGame.Instance.PlayerController.transform.position.z - _transporter.transform.position.z > 10f)
        {
            _isMoving = false;
            this.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _isMoving = true;
    }

}
