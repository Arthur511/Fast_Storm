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
        g.GetComponent<Rigidbody>().linearVelocity = new Vector3(g.GetComponent<Rigidbody>().linearVelocity.x, _jumpSpeed, g.GetComponent<Rigidbody>().linearVelocity.z);
    }

    public void MakeInvertTeleportation()
    {

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
