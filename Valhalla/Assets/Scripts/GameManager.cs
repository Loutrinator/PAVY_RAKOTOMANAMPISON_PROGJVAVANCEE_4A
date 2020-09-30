﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum PlayerType{none,player,random,mcts}

public class GameManager : MonoBehaviour
{
    #region fields

    [Header("Events")] 
    [SerializeField] private UnityEvent UpdateLoop;
    [SerializeField] private UnityEvent FixedUpdateLoop;

    public static GameManager Instance => _instance;
    private static GameManager _instance;

    [SerializeField] private Character characterPrefab;
    [SerializeField] private int _nbPlayers = 2; //In case one day the game can handle more than 2 players
    public int NbPlayers {get { return _nbPlayers; }}
    
    private int advantage = 0;
    private Character[] players;
    public PlayerType[] playerTypes;
    public Character MainPlayer {get{return advantage == 1 ? players[0] : advantage == 2 ? players[1] : null;}} // the one who get the direction

    public Vector3[] spawnPoints;
    
    public int Direction{get{return advantage == 1 ? -1 : advantage == 2 ? 1 : 0;}}
    public static bool IsPaused = true; // TRUE if the game is paused, FALSE otherwise 

    public AudioSource lowBeep;
    public AudioSource highBeep;
    
    #endregion

    #region Overriden functions

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {Destroy(this);}
        
        players = new Character[_nbPlayers];// On set le nombre de joueurs
        playerTypes = new PlayerType[_nbPlayers];
        spawnPoints = new Vector3[_nbPlayers];
        
        /*for (int i = 0; i < players.Length; i++)
        {
            players[i] = GameObject.FindGameObjectWithTag("player" + (i+1)).GetComponent<Character>();
        }*/
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        if(IsPaused) return;
        UpdateLoop?.Invoke();
    }

    private void FixedUpdate()
    {
        if(IsPaused) return;
        FixedUpdateLoop?.Invoke();
    }

    #endregion

    #region other function

    public bool InitGame() //returns true if the game properly initialised
    {
        
        if (players.Length != playerTypes.Length) return false;
        
        for(int i = 0; i < players.Length; ++i)
        {
            var position = (spawnPoints != null && (spawnPoints.Length == _nbPlayers)) ? spawnPoints[i] :Vector3.zero;
            
            players[i] = Instantiate(characterPrefab, position, Quaternion.identity);
            players[i].tag = "player" + (i + 1);
            players[i].name = "player" + (i + 1);
            players[i].id = (i + 1);
            AController controller;
            switch (playerTypes[i])
            {
                case PlayerType.player:
                    PlayerController playerController = players[i].gameObject.AddComponent<PlayerController>();
                    controller = playerController;
                    UpdateLoop.AddListener(controller.ExecuteActions);
                    FixedUpdateLoop.AddListener(controller.CustomFixedUpdate);
                    playerController.isPlayerOne = i == 0;
                    break;
                case PlayerType.random:
                    controller = players[i].gameObject.AddComponent<RandomController>();
                    UpdateLoop.AddListener(controller.ExecuteActions);
                    FixedUpdateLoop.AddListener(controller.CustomFixedUpdate);
                    break;
                case PlayerType.mcts:
                    Debug.LogWarning("Warning : Attempting to initialise player " + (i+1) + " as a MCTS AI but MCTS is not currently implemented.");
                    break;
                case PlayerType.none:
                    Debug.LogError("Error : Attempting to initialise player " + (i+1) + " as with no PlayerType.");
                    return false;
            }
        }
        
        IsPaused = true;
        
        StartCoroutine(GameStart());
        return true;
        //le jeu commence sans avoir d'avantage
    }

    IEnumerator GameStart()
    {
        float delay = 0.7f;
        Debug.Log("GameStart");
        lowBeep.Play();
        yield return new WaitForSeconds(delay);
        lowBeep.Play();
        yield return new WaitForSeconds(delay);
        lowBeep.Play();
        yield return new WaitForSeconds(delay);
        highBeep.Play();
        IsPaused = false;
    }
    
    public void PauseGame()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0 : 1;
    }
    public void PlayerDied(int id)
    {
        if (id == 1)
        {
            advantage = 2;
        }else if (id == 2)
        {
            advantage = 1;
        }
    }
    #endregion

    
    
}
