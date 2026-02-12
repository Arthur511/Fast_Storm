using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class EffectSystem : MonoBehaviour
{
    [SerializeField] StepSpeedEffect[] _stepsSpeed;
    [SerializeField] GameObject _playerVisual;

    List<Material> _activeMaterials = new List<Material>();
    List<GameObject> _particleGameObjects = new List<GameObject>();
    StepSpeedEffect _lastStepBefore;

    public void UpdateEffect()
    {
        foreach (StepSpeedEffect step in _stepsSpeed)
        {
            if (PlayerController.Instance.SpeedPlayer >= step._lowSpeedThreshold && PlayerController.Instance.SpeedPlayer < step._highSpeedThreshold)
            {
                if (step != _lastStepBefore)
                {
                    DestroyActiveParticle();
                    _lastStepBefore = step;
                    _activeMaterials = step.materialToPlayer;
                    DisplayMaterials();
                    foreach (var item in step.particleSystems)
                    {
                        GameObject particle = Instantiate(item, _playerVisual.transform);
                        particle.transform.localPosition = Vector3.zero;
                        _particleGameObjects.Add(particle);
                        particle.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
        }
    }


    private void DisplayMaterials()
    {
        if (_activeMaterials.Count > 0)
            PlayerController.Instance.gameObject.GetComponentInChildren<Renderer>().materials = _activeMaterials.ToArray();
    }

    public void DestroyActiveParticle()
    {
        foreach (var gameParticle in _particleGameObjects)
        {
            Destroy(gameParticle);
        }
        _particleGameObjects.Clear();
    }



}

[Serializable]
class StepSpeedEffect
{
    public float _lowSpeedThreshold;
    public float _highSpeedThreshold;
    public List<Material> materialToPlayer;
    public List<GameObject> particleSystems;
}
