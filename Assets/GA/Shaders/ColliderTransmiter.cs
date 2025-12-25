using UnityEngine;

public class TargetPositionToShader : MonoBehaviour
{
    public Transform target;
    Renderer rend;
    MaterialPropertyBlock block;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (!target) return;

        block.SetVector("_TargetPosition", target.position);
        rend.SetPropertyBlock(block);
    }
}