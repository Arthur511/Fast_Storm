using UnityEngine;

public class GroundObserver : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (1 << other.gameObject.layer == PlayerController.Instance.WallLayer)
            PlayerController.Instance.SetOnGround(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (1 << other.gameObject.layer == PlayerController.Instance.WallLayer)
            PlayerController.Instance.SetOnGround(false);
    }
}
