﻿using System;
using UnityEngine;

enum Zone
{
    PLAYER1 = 0, PLAYER2 = 1
}
public class WinZone : MonoBehaviour
{
    [Tooltip("Select the player who have to reach this zone to win")]
    [SerializeField] private Zone playerAttached;
    [SerializeField] private GameObject winScreen;

    private void Start()
    {
        winScreen.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameState gameState = GameManager.Instance.GetCurrentGameState();
        if (other.gameObject.Equals(gameState.players[(int) playerAttached].gameObject))
        {
            Debug.Log("aze");
            GameManager.Instance.Win(gameState.players[(int) playerAttached]);
            winScreen.SetActive(true);
        }
    }
}
