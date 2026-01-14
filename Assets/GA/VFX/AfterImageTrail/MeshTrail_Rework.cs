using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail_Rework : MonoBehaviour
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

    private MeshRenderer[] skinnedMeshRenderers;
    private bool isTrailActive;
    private MeshFilter[] meshFilters;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            Debug.Log("GetKeyDown Detected While Trail Inactive");
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        Debug.Log("IEnumerator ActivateTrail n°01/04");
        while (timeActive > 0)
        {
            Debug.Log("IEnumerator ActivateTrail n°02/04 time active");
            timeActive -= meshRefreshRate;

            if (meshFilters == null)
            {
                meshFilters = GetComponentsInChildren<MeshFilter>();
            }

            for (int i = 0; i < meshFilters.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshFilter mf = gObj.AddComponent<MeshFilter>();
                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();

                gObj.transform.localScale = Vector3.one * 100f;
                mf.mesh = Instantiate(meshFilters[i].sharedMesh);
                mr.material =mat;

                Destroy(gObj, meshDestroyDelay);
            }


            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
        Debug.Log("set TrailActive false");
    }
    /*
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
    */
}
