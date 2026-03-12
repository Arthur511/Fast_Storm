using System.Collections;
using UnityEngine;

public class PowersManager : MonoBehaviour
{
    public int EnergyCost;

    [Header("JumpPower")]
    [SerializeField] float _jumpSpeed = 30f;
    [Header("LateralDashPower")]
    [SerializeField] float _dashSpeed = 20f;
    [SerializeField] float _dashDuration = 1.0f;
    [Header("PassThroughPower")]
    [SerializeField] float _passThroughDuration = 1.0f;

    public void MakeLateralDash(GameObject g, Vector3 direction)
    {

        Vector3 dashDirection = g.transform.right * direction.x * _dashSpeed;

        g.GetComponent<Rigidbody>().linearVelocity = new Vector3(dashDirection.x, dashDirection.y, g.GetComponent<Rigidbody>().linearVelocity.z);
        StartCoroutine(DashDuration(g));
    }
    IEnumerator DashDuration(GameObject g)
    {
        yield return new WaitForSeconds(_dashDuration);
        g.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, g.GetComponent<Rigidbody>().linearVelocity.z);
    }

    public void MakeJump(GameObject g)
    {

        Vector3 velocityRun = g.GetComponent<Rigidbody>().linearVelocity - MainGame.Instance.PlayerController.CurrentSurfaceNormal;
        g.GetComponent<Rigidbody>().linearVelocity = velocityRun + MainGame.Instance.PlayerController.CurrentSurfaceNormal * _jumpSpeed;
    }

    public void MakeInvertTeleportation()
    {
        RaycastHit hit;
        Debug.Log("Invert");
        if (Physics.Raycast(MainGame.Instance.PlayerController.transform.position, MainGame.Instance.PlayerController.transform.up, out hit, 100, MainGame.Instance.WallLayer))
        {
            MainGame.Instance.PlayerController.transform.position = hit.point;
            MainGame.Instance.PlayerController.transform.rotation = MainGame.Instance.PlayerController.transform.rotation * new Quaternion(0, 0, 180f, 0);

            MainGame.Instance.PlayerController.CurrentSurfaceNormal = -MainGame.Instance.PlayerController.CurrentSurfaceNormal;
            MainGame.Instance.PlayerController.CurrentGravityDirection = -MainGame.Instance.PlayerController.CurrentGravityDirection;

        }
    }

    public void ActivePassThroughMode(GameObject g)
    {
        StartCoroutine(DelayEndPassThrough(g));
    }

    IEnumerator DelayEndPassThrough(GameObject g)
    {
        yield return new WaitForSeconds(_passThroughDuration);
    }

}
