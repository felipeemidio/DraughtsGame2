using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : AbstractPlayer {

    private MyGeneticAlgorithm ga;

    /**
     * CONSTRUCTOR
     */
    public Bot()
    {
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        ga = new MyGeneticAlgorithm(base.board);
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play()
    {
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

        Debug.Log(TranslateBoard());
        //Debug.Log(choseMove.ToString());
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
    private string TranslateBoard()
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
}
