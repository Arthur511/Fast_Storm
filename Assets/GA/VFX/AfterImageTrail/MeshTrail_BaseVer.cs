using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail_BaseVer : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private bool isTrailActive;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            Debug.Log("GetKeyDown Detected While Trail Inactive");
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail (float timeActive)
    {
        Debug.Log("IEnumerator ActivateTrail n°01/04");
        while (timeActive > 0)
        {
            Debug.Log("IEnumerator ActivateTrail n°02/04 time active");
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
            {
                Debug.Log("IEnumerator ActivateTrail n°03/04 if mesh renderer");
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i=0; i<skinnedMeshRenderers.Length; i++)
            {
                Debug.Log("IEnumerator ActivateTrail n°04/04 for i in .length");
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);
            }
            

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
        Debug.Log("set TrailActive false");
    }

    IEnumerator AnimateMaterialFloat (Material mat, float goal, float rate, float refreshRate)
    {
        Debug.Log("IEnumerator AnimateMaterialFloat n°01/02");
        float ValueToAnimate = mat.GetFloat(shaderVarRef);

        while (ValueToAnimate > goal)
        {
            Debug.Log("IEnumerator AnimateMaterialFloat n°02/02");
            ValueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, ValueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }

    }

}
