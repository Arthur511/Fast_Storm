using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class EffectSystem : MonoBehaviour
{
    [SerializeField] StepSpeedEffect[] _stepsSpeed;

    List<Material> _activeMaterials = new List<Material>();
    //List<ParticleSystem> _activeParticles = new List<ParticleSystem>();
    private void Start()
    {
        
    }

    private void Update()
    {
        foreach (StepSpeedEffect step in _stepsSpeed)
        {
            if (PlayerController.Instance.SpeedPlayer > step.speedThreshold)
            {
                //_activeParticles
                _activeMaterials = step.materialToPlayer;
                DisplayMaterials();
            }
        }
    }

    private void DisplayMaterials()
    {
        PlayerController.Instance.gameObject.GetComponentInChildren<Renderer>().materials = _activeMaterials.ToArray();
    }

    private void DisplayParticles()
    {
        
    }

    /*private void DestroyActiveParticle()
    {
        foreach (var item in _activeParticles)
        {
            Destroy
        }
    }*/

}



[Serializable]
class StepSpeedEffect
{
    public float speedThreshold;
    public List<Material> materialToPlayer;
    public List<GameObject> particleSystems;
}
