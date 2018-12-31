using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : AbstractPlayer {

    private MyGeneticAlgorithm ga;
    private List<BoardConfiguration> configList;
    private List<BoardConfiguration> historic;
    private Movement lastMovement;

    private bool useHistoric = false;
    private float biggestAdaptation;
    private List<Movement> bestPath;

    private readonly int MAX_DEPTH = 6;

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
        biggestAdaptation = -1000f;
        string configuration = TranslateBoard();
        if(configList.Count >= 1 && !useHistoric)
        {
            Debug.Log("Last Movement: " + lastMovement);
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
        BoardConfiguration bConfig = new BoardConfiguration(configuration);
        bConfig.AddMovement(choseMove, ga.AdaptationScore(choseMove));
        configList.Add(bConfig);

        //Make a function call to find the best adaptation in the historic.
        Movement moveByHistoric = ChoseMovementFromHistoric(configuration);
        
        if( ga.AdaptationScore(choseMove)/2 < biggestAdaptation)
        {
            //Debug.Log("select historic movement");
            choseMove = moveByHistoric;
            useHistoric = true;
            
        }
        else
        {
            //Debug.Log("select current best movement");
            useHistoric = false;
        }

        //Debug.Log(choseMove.ToString());
        lastMovement = choseMove;
        base.board.MovePiece(choseMove);
    }

    private Movement ChoseMovementFromHistoric(string configuration)
    {
        BoardConfiguration bc = FindBoardConfiguration(configuration);
        float adaptation = -1000f;
        Movement bestMovement = null;
        if (bc == null)
        {
            return bestMovement;
        }

        foreach (MovementConfiguration moveConf in bc.GetMovementsConfigurations())
        {
            if (moveConf.GetResults().Count == 0)
            {
                adaptation = moveConf.GetAdaptation();
                if (adaptation > biggestAdaptation)
                {
                    biggestAdaptation = adaptation;
                    bestMovement = moveConf.GetMove();
                }
            }
            foreach (string result in moveConf.GetResults())
            {

                adaptation = MeanAdaptation(result, 0f, 1);
                if(adaptation > biggestAdaptation)
                {
                    biggestAdaptation = adaptation;
                    bestMovement = moveConf.GetMove();
                }
            }
        }
        //Debug.Log("Best Adaptation: " + biggestAdaptation);

        return bestMovement;
    }

    private float MeanAdaptation(string configuration, float sum, int depth)
    {

        BoardConfiguration bc = FindBoardConfiguration(configuration);
        if (bc == null || depth > MAX_DEPTH )
            return sum/depth;


        float betterAdaptation = -1000f;

        foreach(MovementConfiguration mc in bc.GetMovementsConfigurations())
        {
            if(mc.GetResults().Count == 0 &&
                betterAdaptation < (sum + mc.GetAdaptation()) / (depth + 1))
            {
                betterAdaptation = (sum + mc.GetAdaptation()) / (depth + 1);
            }

            foreach (string result in mc.GetResults())
            {

                float adaptation = MeanAdaptation(result, sum + mc.GetAdaptation(), depth + 1);
                if (adaptation > betterAdaptation)
                {
                    betterAdaptation = adaptation;
                }
            }
        }

        return betterAdaptation;
    }

    private BoardConfiguration FindBoardConfiguration(string configuration)
    {
        BoardConfiguration bc = null;
        foreach (BoardConfiguration boardConf in historic)
        {
            if (boardConf.GetBoardConfiguration().Equals(configuration))
            {
                bc = boardConf;
                break;
            }
        }

        return bc;
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
