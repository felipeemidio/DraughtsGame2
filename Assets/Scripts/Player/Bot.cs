using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : AbstractPlayer {

    /**
     * CONSTRUCTOR
     */
    public Bot ()
    {
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play()
    {
        //Debug.Log("BOT Playing");

        Movement choseMove;
        // Select the same piece if it's playing again.
        if (base.currentPiece != null)
        {
            choseMove = (Movement) base.currentPiece.GetBestSucessiveCapture()[0];
        }
        // Select a new piece if it's the first play of this turn.
        else
        {
            choseMove = ChooseOneMovement();
            base.currentPiece = base.board.GetTile(choseMove.getOriginalPosition())
                .GetChild().GetComponent<Piece>();
        }
        
        //Debug.Log(choseMove.ToString());
        base.board.MovePiece (choseMove);
    }

    /// <summary>
    /// Select one possible movement.
    /// </summary>
    public Movement ChooseOneMovement ()
    {
        ArrayList botPieces = base.board.GetEnemyPieces();
        ArrayList possibleMoves = new ArrayList();

        // Get all possible capture movements. if exists.
        if (base.SomePieceCanCapture (botPieces))
        {
            base.isCapturing = true;
            foreach (Piece piece in botPieces)
            {
                possibleMoves.AddRange(piece.GetBestSucessiveCapture());
            }
        }
        // Get all possible walk movements if anyone can capture.
        else
        {
            foreach (Piece piece in botPieces)
            {
                possibleMoves.AddRange(piece.GetWalkMovements());
            }
        }
        // Get a random movement of the possible ones.
        int randomNumber = Random.Range(0, possibleMoves.Count);
        return (Movement) possibleMoves[randomNumber];
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

        // Finish this turn.
        isSucessiveCapture = false;
        base.isCapturing = false;
        base.currentPiece = null;
        base.board.DestroyCapturedPieces();
    }
}
