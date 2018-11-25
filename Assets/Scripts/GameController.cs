using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public Text turnText;
    public enum Turn
    {
        enemyTurn,
        playerTurn
    }

    private Turn turn;
    private Board board;
    private Bot bot;
    private Player player;

    /*
     * Initialize variables.
     */
    void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board> ();
        bot = new Bot();
        player = new Player();
        turn = Turn.enemyTurn;
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
        if (turn == Turn.playerTurn)
        {
            turn = Turn.enemyTurn;
            turnText.text = "ENEMY'S TURN";
            //bot.Play();
            board.RefreshAllPieces();

        }
        else
        {
            turn = Turn.playerTurn;
            turnText.text = "YOUR TURN";
            board.SomePieceCanCapture();
            player.Play();
        }

        board.DestroyCapturedPieces();
    }

    public void SendToPlayer(TileHandler tile)
    {
        player.SelectionHandler(tile);
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
