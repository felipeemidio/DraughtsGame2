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
        base.gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play()
    {
        Debug.Log ("BOT Playing");

        Movement choseMove = ChooseOneMovement();
        Debug.Log(choseMove.ToString());
        base.board.MovePiece (choseMove);
    }

    /**
     * Select one possible movement.
     */
    public Movement ChooseOneMovement ()
    {
        ArrayList botPieces = base.board.GetEnemyPieces();
        ArrayList possibleMoves = new ArrayList();

        if (base.SomePieceCanCapture (botPieces))
        {
            foreach (Piece piece in botPieces)
            {
                possibleMoves.AddRange(piece.GetBestSucessiveCapture());
            }
        }
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
        base.board.DestroyCapturedPieces();
        this.gameController.NextTurn();
    }
}
