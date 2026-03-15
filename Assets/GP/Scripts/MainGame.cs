using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{

    public static MainGame Instance;
    public LayerMask TransitionLayer => _transitionLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;
    public LayerMask DoorLayer => _doorLayer;
    public LayerMask WallLayer => _wallLayer;
    public PlayerController PlayerController => _playerController;

    [Header("Scripts")]
    public SaveSystem SaveSystem;
    public UIManager UIManager;
    public PlayerController _playerController;
    
    [SerializeField] LayerMask _transitionLayer;
    [SerializeField] LayerMask _obstacleLayer;
    [SerializeField] LayerMask _doorLayer;
    [SerializeField] LayerMask _wallLayer;

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

        if (Input.GetKeyDown(KeyCode.O))
            UIManager.DisplayScorePanel(_playerController.Score);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
