using System.Collections.Generic;
using UnityEngine;

public class TransporterObstacle : MonoBehaviour
{
    [SerializeField] List<GameObject> _transporters;
    [SerializeField] float _speedTransporter;

    bool _isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
        {
            foreach (var transporter in _transporters)
            {
                transporter.transform.position -= new Vector3(0, 0, _speedTransporter * Time.deltaTime);
            }
        }
        if (MainGame.Instance.PlayerController.transform.position.z - _transporters[_transporters.Count - 1].gameObject.transform.position.z > 10f)
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
