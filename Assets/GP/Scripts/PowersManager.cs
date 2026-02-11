using UnityEngine;

public class PowersManager : MonoBehaviour
{
    public int EnergyCost;

    [Header("BumperPower")]
    [SerializeField] float _bumperSpeed = 30f;
    [Header("LateralDashPower")]
    [SerializeField] float _dashSpeed = 20f;

    public void MakeLateralDash(GameObject g, Vector3 direction)
    {
        g.GetComponent<Rigidbody>().AddForce(direction * _dashSpeed, ForceMode.Impulse);
    }

    public void MakeJump(GameObject g)
    {
        g.GetComponent<Rigidbody>().AddForce(g.transform.up * _bumperSpeed, ForceMode.Impulse);
    }
}
