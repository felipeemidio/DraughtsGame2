using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractPlayer {

    protected Board board;
    protected GameController gameController;

    /**
     * Sign the start of that player's turn.
     */
    public abstract void Play();

    /**
     * It's called when the movement chose by this player is finished.
     */
    public abstract void NotifyEndOfMovement();

    /// <summary>
    /// Verify if some piece int the piece list parameter can capture.
    /// Return true if some piece can capture and false if doesn't.
    /// </summary>
    protected bool SomePieceCanCapture(ArrayList piecesList)
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
}
