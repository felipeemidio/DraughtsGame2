﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
    private ArrayList historic;

    void Awake()
    {
        turn = Turn.enemyTurn;
        if (resultPanel != null)
            resultPanel.gameObject.SetActive(false);
        else
            Debug.LogError("Couldn't find the panel object.");
        bot = new Bot();
        player = new Player();
        historic = new ArrayList();
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
        if (Input.GetKeyUp(KeyCode.S))
        {
            Save(new ArrayList());
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            Load();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            Clear();
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            Test();
        }
    }

    /// <summary>
    /// Random function to write some persitance tests in it.
    /// </summary>
    /// <remarks>
    /// Only used for Debug Purpose.
    /// </remarks>
    public void Test()
    {
        BoardConfiguration foo =
                new BoardConfiguration("b#b###################w######w####B###b#########W########w###w##");
        Movement movement = new Movement(new IntVector2(7, 1),
            new IntVector2(3, 5), new IntVector2(5, 3));
        foo.AddMovement(movement, 5f);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/gameStorage.dat", FileMode.Open);
        historic = (ArrayList)bf.Deserialize(file);
        file.Close();

        bool result = false;
        bool result2 = false;
        foreach (BoardConfiguration bc in historic)
        {
            if (bc.Equals(foo))
            {
                result = true;
                Debug.Log("find: " + bc.ToString());
                Debug.Log("movement contains: " + bc.GetMovements().Contains(foo.GetMovements()[0]));
            }
        }

        result2 = historic.Contains(foo);

        Debug.Log("result for: " + result + "\nresult contain: " + result2);
    }

    /// <summary>
    /// Increment and save the game historic in the 'gameStorage' file.
    /// </summary>
    public void Save(ArrayList list)
    {
        Debug.Log("Save Called.");
        // Create a File.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameStorage.dat");

        // Add the new board configuration list in the historic.
        bool existsConf;
        foreach (BoardConfiguration config in list)
        {
            existsConf = false;
            foreach(BoardConfiguration historicConfig in historic)
            {
                // See if it's has a new Movement for that existing configuration.
                if (historicConfig.Equals(config))
                {
                    existsConf = true;
                    if (!historicConfig.GetMovements().Contains(config.GetMovements()[0]))
                    {
                        historicConfig.AddMovement((Movement)config.GetMovements()[0],
                        (float)config.GetValuesByMovement()[0]);
                    } 
                }
            }
            if (!existsConf)
            {
                historic.Add(config);
            }
        }

        bf.Serialize(file, historic);
        file.Close();

    }

    /// <summary>
    /// Load the game historic in the 'gameStorage' file.
    /// </summary>
    public void Load()
    {
        Debug.Log("Load Called.");
        if (File.Exists(Application.persistentDataPath + "/gameStorage.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameStorage.dat", FileMode.Open);
            historic = (ArrayList) bf.Deserialize(file);
            file.Close();

            if(historic == null)
            {
                Debug.Log("historico nulo.");
                return;
            }

            foreach(BoardConfiguration conf in historic)
            {
                Debug.Log(conf.ToString());
            }
        }
    }

    /// <summary>
    /// Clear the game historic in the 'gameStorage' file.
    /// </summary>
    /// <remarks>
    /// Only used for Debug Purpose.
    /// </remarks>
    public void Clear()
    {
        Debug.Log("Clear Called.");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameStorage.dat");
        bf.Serialize(file, new ArrayList());
        file.Close();

    }

    /// <summary>
    /// Change the turn between the enemy and the player.
    /// Also update the UI text.
    /// </summary>
    public void NextTurn()
    {
        board.RefreshAllPieces();
        if (turn == Turn.playerTurn)
        {
            turn = Turn.enemyTurn;
            turnText.text = "ENEMY'S TURN";
            //bot.Play();
            StartCoroutine(BotPlay());

        }
        else
        {
            turn = Turn.playerTurn;
            turnText.text = "YOUR TURN";
            board.SomePieceCanCapture();
            player.Play();
        }
    }

    IEnumerator BotPlay()
    {
        yield return new WaitForSeconds(1.0f);
        bot.Play();
    }

    public void SendToPlayer(TileHandler tile)
    {
        player.SelectionHandler(tile);
    }

    public void NotifyPlayerEndOfMovement()
    {
        bool isSucessiveCapture = false;
        float finalValue = -100f;
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
                finalValue = -20f;
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
                finalValue = 20f;
                ShowResultPanel("Y O U   L O S E !");
            }
        }

        if (inFinal)
            finalCounter += 1;

        if(turnsKingMoving >= 20 || finalCounter >= 10)
        {
            finalValue = 5f;
            ShowResultPanel("D R A W !");
        }
        if (isGameOver)
        {
            if(finalValue > -100f)
                bot.SetLastMovement(finalValue);
            Save(bot.GetConfigList());
        }
        if(!isSucessiveCapture && !isGameOver)
            this.NextTurn();
    }

    /// <summary>
    /// Verify the winning condition given a player.
    /// </summary>
    /// <remarks>
    /// #### CONDITIONS ####
    /// 1- the player hasn't pieces.
    /// 2- the player can't move the pieces his has.
    /// </remarks>
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
        isGameOver = true;
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
