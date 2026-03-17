using System.Collections.Generic;
using UnityEngine;

public class TransporterObstacle : MonoBehaviour
{
    [SerializeField] List<GameObject> _transporters;
    [SerializeField] float _speedTransporter;

    bool _isMoving = false;
    Vector3[] _transportersStartPositions;
    int _indexTransporter = 0;

    private void Start()
    {
        _transportersStartPositions = new Vector3[_transporters.Count];
        foreach (var transporter in _transporters)
        {
            _transportersStartPositions[_indexTransporter] = transporter.transform.position;
            _indexTransporter++;
        }
    }

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

    public void ResetTransportersPosition()
    {
        _isMoving = false;
        _indexTransporter = 0;
        foreach (var transporter in _transporters)
        {
            transporter.transform.position = _transportersStartPositions[_indexTransporter];
            _indexTransporter++;
        }
    }


}
