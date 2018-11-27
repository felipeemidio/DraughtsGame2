﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public Text turnText;
    public Image resultPanel;

    private enum Turn
    {
        enemyTurn,
        playerTurn
    }
    private Turn turn;
    private Board board;
    private Bot bot;
    private Player player;

    void Awake()
    {
        turn = Turn.enemyTurn;
        if (resultPanel != null)
            resultPanel.gameObject.SetActive(false);
        else
            Debug.LogError("Couldn't find the panel object.");
        bot = new Bot();
        player = new Player();
    }

    /*
     * Initialize variables.
     */
    void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board> ();
        this.NextTurn();
        if (turnText == null)
        {
            Debug.LogError("Turn Text not Found.");
        }
    }

    void Update()
    {
        // Change to layer turn when press key 'A'.
        if (Input.GetKeyUp(KeyCode.A) && turn == Turn.enemyTurn)
        {
            NextTurn();
        }
    }

    /*
     * Change the turn between the enemy and the player.
     * Also update the UI text.
     */
    public void NextTurn()
    {
        board.RefreshAllPieces();
        if (turn == Turn.playerTurn)
        {
            turn = Turn.enemyTurn;
            turnText.text = "ENEMY'S TURN";
            bot.Play();

        }
        else
        {
            turn = Turn.playerTurn;
            turnText.text = "YOUR TURN";
            board.SomePieceCanCapture();
            player.Play();
        }
    }

    public void SendToPlayer(TileHandler tile)
    {
        player.SelectionHandler(tile);
    }

    public void NotifyPlayerEndOfMovement()
    {
        if(turn == Turn.playerTurn) {
            
            player.NotifyEndOfMovement();
            // Verify if the player won the game.
            if (WinGame(bot, this.board.GetEnemyPieces()) && resultPanel != null)
            {
                resultPanel.gameObject.SetActive(true);
                Text resultText = resultPanel.transform.GetChild(0).GetComponent<Text>();
                resultText.text = "Y O U   W O N !";
            }
        }
        else
        {
            bot.NotifyEndOfMovement();
            // Verify if the bot won the game.
            if (WinGame(player, this.board.GetPlayerPieces()))
            {
                resultPanel.gameObject.SetActive(true);
                Text resultText = resultPanel.transform.GetChild(0).GetComponent<Text>();
                resultText.text = "Y O U   L O S E !";
            }
        } 
    }

    /**
     * Verify the winning condition.
     * 
     * 1- the player hasn't pieces.
     * 2- the player can't move the pieces his has.
     */
    private bool WinGame(AbstractPlayer absEnemy, ArrayList enemiesPieces)
    {
        if (enemiesPieces.Count == 0 ||
            (!absEnemy.SomePieceCanCapture(enemiesPieces) &&
            !absEnemy.SomePieceCanWalk(enemiesPieces)))
        {
            return true;
        }
        return false;
    }

    /*
     * Return true if is the player turn.
     */
    public bool isPlayerTurn()
    {
        if (turn == Turn.playerTurn)
            return true;
        return false;
    }
}
