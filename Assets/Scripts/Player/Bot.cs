using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : AbstractPlayer {

    private MyGeneticAlgorithm ga;
    private List<BoardConfiguration> configList;
    private List<BoardConfiguration> historic;
    private Movement lastMovement;

    //private readonly int MAX_DEPTH = 6;

    /**
     * CONSTRUCTOR
     */
    public Bot(List<BoardConfiguration> historic)
    {
        this.historic = historic;
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        ga = new MyGeneticAlgorithm(base.board);
        configList = new List<BoardConfiguration>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play()
    {
        string configuration = TranslateBoard();
        if(configList.Count >= 1)
        {
            configList[configList.Count - 1].GetMovementConfigurationWithMove(lastMovement)
                .AddResults(configuration);
        }

        Movement choseMove;
        // Select the same piece if it's playing again.
        if (base.currentPiece != null)
        {
            choseMove = (Movement)base.currentPiece.GetBestSucessiveCapture()[0];
        }
        // Select a new piece if it's the first play of this turn.
        else
        {
            choseMove = ChooseOneMovement();
            base.currentPiece = base.board.GetTile(choseMove.getOriginalPosition())
                .GetChild().GetComponent<Piece>();
        }

        // Add the current board configuration with the chose movement to the config list.
        BoardConfiguration bConfig = new BoardConfiguration(TranslateBoard());
        bConfig.AddMovement(choseMove, ga.AdaptationScore(choseMove));
        configList.Add(bConfig);

        //TODO: Make a function call to find the best adaptation in the historic.

        Debug.Log(bConfig.GetBoardConfiguration());
        //Debug.Log(choseMove.ToString());

        lastMovement = choseMove;
        base.board.MovePiece(choseMove);
    }


    /// <summary>
    /// Select one possible movement.
    /// </summary>
    public Movement ChooseOneMovement()
    {
        base.board.RefreshAllPieces();
        ArrayList botPieces = base.board.GetEnemyPieces();
        ArrayList possibleMoves = this.ga.GenerateMutations(this, botPieces);

        // Select the movement with the biggest adaptation.
        int biggestAdaptation = -100;
        Movement bestMovement = null;
        foreach (Movement move in possibleMoves)
        {
            int adaptation = this.ga.AdaptationScore(move);
            if (adaptation > biggestAdaptation)
            {
                biggestAdaptation = adaptation;
                bestMovement = move;
            }
        }
        return bestMovement;
    }

    /// <summary>
    /// It's called when the movement chose by this player is finished.
    /// </summary>
    public override void NotifyEndOfMovement()
    {
        // Find out if the current piece can capture again.
        if (base.isCapturing)
        {
            ArrayList canMoveTo = base.currentPiece.GetBestSucessiveCapture();
            isSucessiveCapture = true;
            if (canMoveTo.Count != 0)
            {
                this.Play();
                return;
            }
        }

        // Try to promote
        ManPiece currentManPiece = base.currentPiece.GetComponent<ManPiece>();
        if (currentManPiece != null)
            currentManPiece.Promote();

        // Finish this turn.
        isSucessiveCapture = false;
        base.isCapturing = false;
        base.currentPiece = null;
        base.board.DestroyCapturedPieces();

    }

    /// <summary>
    /// Tranform the current state of the board in a string.
    /// </summary>
    public string TranslateBoard()
    {
        string result = "";
        TileHandler tile;
        for (int i = 1; i <= 8; ++i)
        {
            for(int j = 1; j <= 8; ++j)
            {
                tile = base.board.GetTile(i, j);
                if (!tile.HasChild())
                {
                    result += '#';
                }
                else
                {
                    Piece child = tile.GetChild().GetComponent<Piece>();
                    if (child.HasBeenCaptured())
                        result += '#';
                    else if (child.GetComponent<ManPiece>() != null)
                    {
                        if (child.CompareTag("BluePiece"))
                        {
                            result += 'b';
                        }
                        else
                        {
                            result += 'w';
                        }
                    }
                    else
                    {
                        if (child.CompareTag("BluePiece"))
                        {
                            result += 'B';
                        }
                        else
                        {
                            result += 'W';
                        }
                    }
                }
            }
        }

        return result;
    }

    public List<BoardConfiguration> GetConfigList()
    {
        return configList;
    }

    public void SetLastMovement(float value)
    {
        BoardConfiguration bc = configList[configList.Count - 1];
        bc.SetLastValue(value);
        configList[configList.Count - 1] = bc;
    }
}
