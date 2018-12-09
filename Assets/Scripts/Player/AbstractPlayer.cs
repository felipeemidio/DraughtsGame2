using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractPlayer {

    protected Board board;
    protected Piece currentPiece = null;
    protected bool isCapturing = false;
    protected bool isSucessiveCapture = false;

    /**
     * Sign the start of that player's turn.
     */
    public abstract void Play();

    /**
     * It's called when the movement chose by this player is finished.
     */
    public abstract void NotifyEndOfMovement();

    /// <summary>
    /// Return true if the current piece used was a king.
    /// </summary>
    public bool UsedKingPiece()
    {
        if (currentPiece != null && currentPiece.GetComponent<KingPiece>())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Verify if some piece int the piece list parameter can capture.
    /// Return true if some piece can capture and false if doesn't.
    /// </summary>
    public bool SomePieceCanCapture(ArrayList piecesList)
    {
        ArrayList captureMovements;
        foreach (Piece piece in piecesList)
        {
            captureMovements = piece.GetCaptureMovements();
            if (captureMovements.Count != 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool SomePieceCanWalk(ArrayList piecesList)
    {
        ArrayList walkMovements;
        foreach (Piece piece in piecesList)
        {
            walkMovements = piece.GetWalkMovements();
            if (walkMovements.Count != 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool GetIsCapturing()
    {
        return this.isCapturing;
    }

    public bool GetIsSucessiveCapture()
    {
        return this.isSucessiveCapture;
    }
}
