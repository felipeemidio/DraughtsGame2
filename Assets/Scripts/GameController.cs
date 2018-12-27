using System.Collections;
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
    private int turnsKingMoving;
    private bool inFinal = false;
    private int finalCounter = 0;
    private bool isGameOver = false;
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
        turnsKingMoving = 0;
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
        // Refresh allin case some was destructed.
        //board.RefreshAllPieces();
        bool isSucessiveCapture = false;
        IsInFinals();
        if (turn == Turn.playerTurn) {
            // See if just move a king piece
            RefreshDrawCounters(player);

            player.NotifyEndOfMovement();
            isSucessiveCapture = player.GetIsSucessiveCapture();
            if (!isSucessiveCapture)
                board.RefreshAllPieces();
            // Verify if the player won the game.
            if (WinGame(bot, this.board.GetEnemyPieces()) && resultPanel != null)
            {
                ShowResultPanel("Y O U   W O N !");
            }
        }
        else
        {
            // See if just move a king piece
            RefreshDrawCounters(bot);

            bot.NotifyEndOfMovement();
            isSucessiveCapture = bot.GetIsSucessiveCapture();
            if (!isSucessiveCapture)
                board.RefreshAllPieces();
            // Verify if the bot won the game.
            if (WinGame(player, this.board.GetPlayerPieces()))
            {
                ShowResultPanel("Y O U   L O S E !");
            }
        }

        if (inFinal)
            finalCounter += 1;

        if(turnsKingMoving >= 20 || finalCounter >= 10)
        {
            ShowResultPanel("D R A W !");
        }
        if(!isSucessiveCapture && !isGameOver)
            this.NextTurn();
    }

    /**
     * Verify the winning condition given a player.
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
            isGameOver = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Updates the turnKingMoving variable that is incremented
    /// when some player just moves a king.
    /// </summary>
    private void RefreshDrawCounters(AbstractPlayer absPlayer)
    {
        if (absPlayer.UsedKingPiece() && !absPlayer.GetIsCapturing())
        {
            turnsKingMoving += 1;
        }
        else
        {
            turnsKingMoving = 0;
        }
    }

    /// <summary>
    /// See the condition to start with the final countdown.
    /// If the game do not finish
    /// </summary>
    private void IsInFinals ()
    {
        if ( !inFinal &&
            this.board.GetPlayerPieces().Count <= 2 &&
            this.board.GetEnemyPieces().Count <= 2 &&
            this.board.NumberOfPlayerManPieces() + this.board.NumberOfEnemyManPieces() <= 1 )
        {
            Debug.Log("Estamos em finais");
            inFinal = true;
        }
    }

    /// <summary>
    /// Finish the gamez.
    /// Open the result panel with the text given as parameter.
    /// </summary>
    private void ShowResultPanel(string text)
    {
        if (resultPanel.gameObject.activeSelf)
            return;
        resultPanel.gameObject.SetActive(true);
        Text resultText = resultPanel.transform.GetChild(0).GetComponent<Text>();
        resultText.text = text;
    }

    /// <summary>
    /// Return true if is the player turn.
    /// </summary>
    public bool IsPlayerTurn()
    {
        if (turn == Turn.playerTurn)
            return true;
        return false;
    }
}
