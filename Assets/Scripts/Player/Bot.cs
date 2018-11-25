using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : AbstractPlayer {

    private Piece currentPiece = null; 

    /**
     * CONSTRUCTOR
     */
    public Bot ()
    {
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        base.gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play()
    {
        Debug.Log("BOT Playing");

        Movement choseMove;
        // Select the same piece if it's playing again.
        if (currentPiece != null)
        {
            choseMove = (Movement) currentPiece.GetBestSucessiveCapture()[0];
        }
        // Select a new piece if it's the first play of this turn.
        else
        {
            choseMove = ChooseOneMovement();
            this.currentPiece = base.board.GetTile(choseMove.getOriginalPosition())
                .GetChild().GetComponent<Piece>();
        }
        
        Debug.Log(choseMove.ToString());
        base.board.MovePiece (choseMove);
    }

    /// <summary>
    /// Select one possible movement.
    /// </summary>
    public Movement ChooseOneMovement ()
    {
        ArrayList botPieces = base.board.GetEnemyPieces();
        ArrayList possibleMoves = new ArrayList();
        // Choose between the one that can capture, if exists.
        if (base.SomePieceCanCapture (botPieces))
        {
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

        int randomNumber = Random.Range(0, possibleMoves.Count);
        return (Movement) possibleMoves[randomNumber];
    }

    /// <summary>
    /// It's called when the movement chose by this player is finished.
    /// </summary>
    public override void NotifyEndOfMovement()
    {
        ArrayList canMoveTo = currentPiece.GetBestSucessiveCapture();
        if (canMoveTo.Count != 0)
        {
            this.Play();
            return;
        }

        currentPiece = null;
        base.board.DestroyCapturedPieces();
        this.gameController.NextTurn();
    }
}
