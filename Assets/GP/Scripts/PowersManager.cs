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
    [SerializeField] float _passThroughDuration = 3.0f;
    [SerializeField] Material _passThroughMaterial;

    public void MakeLateralDash(GameObject g, Vector3 direction)
    {

        Vector3 dashDirection = g.transform.right * direction.x * _dashSpeed;

        g.GetComponent<Rigidbody>().linearVelocity = new Vector3(dashDirection.x, dashDirection.y, g.GetComponent<Rigidbody>().linearVelocity.z);
        StartCoroutine(DashDuration(g));
    }
    IEnumerator DashDuration(GameObject g)
    {
        MainGame.Instance.PlayerController.IsDashing = true;
        yield return new WaitForSeconds(_dashDuration);
        MainGame.Instance.PlayerController.IsDashing = false;
    }

    public void MakeJump(GameObject g)
    {

        Vector3 velocityRun = g.GetComponent<Rigidbody>().linearVelocity - MainGame.Instance.PlayerController.CurrentSurfaceNormal;
        g.GetComponent<Rigidbody>().linearVelocity = velocityRun + MainGame.Instance.PlayerController.CurrentSurfaceNormal * _jumpSpeed;
    }

    public void MakeInvertTeleportation()
    {
        RaycastHit hit;
        var player = MainGame.Instance.PlayerController;

        Vector3 origin = player.transform.position;
        Vector3 direction = -player.CurrentGravityDirection;
        if (Physics.Raycast(origin, direction, out hit, 1000f, player.WallLayer, QueryTriggerInteraction.Ignore))
        {
            //player.transform.rotation = Quaternion.AngleAxis(180, player.transform.forward) * player.transform.rotation;

            player.CurrentSurfaceNormal = hit.normal;
            player.CurrentGravityDirection = -hit.normal;

            player.IsInverting = true;

            player.SmoothedSurfaceNormal = hit.normal;

            //player.GetComponent<Rigidbody>().position = hit.point + hit.normal * 0.5f;
            player.transform.position = hit.point + hit.normal * 0.5f;
            Physics.SyncTransforms();

            float currentSpeed = GetComponent<Rigidbody>().linearVelocity.magnitude;
            GetComponent<Rigidbody>().linearVelocity = player.transform.forward * currentSpeed;

        }
    }

    public void ActivePassThroughMode(GameObject g)
    {
        StartCoroutine(DelayEndPassThrough(g));
    }

    IEnumerator DelayEndPassThrough(GameObject g)
    {
        Material[] currentMats = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _passThroughMaterial;
        g.GetComponent<Collider>().excludeLayers = MainGame.Instance.ObstacleLayer;
        yield return new WaitForSeconds(_passThroughDuration);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials = currentMats;
        g.GetComponent<Collider>().excludeLayers = new LayerMask();
    }

}
