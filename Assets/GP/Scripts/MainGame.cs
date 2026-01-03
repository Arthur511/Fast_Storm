using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{

    public static MainGame Instance;

    [SerializeField] LayerMask _transitionLayer;
    public LayerMask TransitionLayer => _transitionLayer;

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
        
    }
}
