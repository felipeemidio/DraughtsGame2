using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot {

    Board board;

    /**
     * Constructor
     */
	public Bot ()
    {
        this.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
    }

    public void Play()
    {
        Debug.Log ("BOT Playing");
        Movement choseMove = ChooseOneMovement();
        Debug.Log(choseMove.ToString());
        //this.board.MovePiece (choseMove);
    }

    /**
     * Select one possible movement.
     */
    public Movement ChooseOneMovement ()
    {
        this.board.RefreshAllPieces();
        ArrayList botPieces = this.board.GetEnemyPieces();

        ArrayList possibleMoves = new ArrayList();

        if (this.SomePieceCanCapture (botPieces))
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

    /**
     * Verify if some piece can capture a enemy's piece.
     */
    private bool SomePieceCanCapture(ArrayList piecesList)
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
