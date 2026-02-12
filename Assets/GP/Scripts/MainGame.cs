using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{

    public static MainGame Instance;
    public LayerMask TransitionLayer => _transitionLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;
    //public EffectSystem EffectSystem { get; }

    [SerializeField] LayerMask _transitionLayer;
    [SerializeField] LayerMask _obstacleLayer;

    /*[Header("Scripts")]
    [SerializeField] EffectSystem _effectSystem;*/

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
