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
        //Debug.Log("All Movements:\n" + PrintMovements(possibleMoves));

        int biggestAdaptation = -100;
        Movement bestMovement = null;
        foreach (Movement move in possibleMoves)
        {
            //Debug.Log("possible move: " + move.ToString());
            int adaptation = this.ga.AdaptationScore(move);
            if (adaptation > biggestAdaptation)
            {
                biggestAdaptation = adaptation;
                bestMovement = move;
            }
        }
        //Debug.Log("Best Movement- Score: " + biggestAdaptation + 
        //    "\nMovement " + bestMovement.ToString());
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

    private string PrintMovements(ArrayList list)
    {
        string result = "";
        foreach (Movement move in list)
        {
            result += move.ToString() + " - ";
        }
        result += "final.";
        return result;
    }
}
