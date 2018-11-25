using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AbstractPlayer {

    private Piece currentPiece = null;
    private ArrayList canMoveTo = null;
    private bool somePieceCanCapture = false;
    private bool isCapturing = false;
    private bool isSucessiveCapture = false;

    public Player ()
    {
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        base.gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play ()
    {
        somePieceCanCapture = base.SomePieceCanCapture(base.board.GetPlayerPieces());
        if (somePieceCanCapture)
            isCapturing = true;
    }

    public void SelectionHandler (TileHandler tile)
    {
        base.board.ResetPossibleMovements(canMoveTo);
        bool hasPiece = tile.HasChild();  
        // In case the player want to move a piece.
        if (currentPiece != null && !hasPiece)
        {
            // If that tile is in the 'canMoveTo', then move the current piece.
            Movement movement = GetMovementIfExists (canMoveTo, tile.getPosition());
            if (movement != null)
            {
                base.board.MovePiece (movement);
            }    
        }
        // In case the player try to select a piece in a sucessive capture.
        else if (currentPiece != null && isSucessiveCapture && hasPiece)
        {
            // If the movement is a sucessive movement, then that piece is the only playable.
            if (currentPiece == tile.GetChild().GetComponent<Piece>())
            {
                // Paints select piece's tile and the avaliable tiles to move.
                base.board.SelectPiece(currentPiece, canMoveTo);
            }
            else
            {
                base.board.SelectPiece(tile.GetChild().GetComponent<Piece>(), new ArrayList());
            }
        }
        // In case the player select a new piece.
        else if (hasPiece)
        {
            // Get the piece of the tile selected.tile.GetChild ().GetComponent<Piece>();
            currentPiece = tile.GetChild ().GetComponent<Piece>();
            if (currentPiece == null || !currentPiece.CompareTag("BluePiece"))
                return;
            
            // Store the possible movements in a variable 'canMoveTo'.
            canMoveTo = currentPiece.GetBestSucessiveCapture();
            if(canMoveTo.Count == 0 && !somePieceCanCapture)
            {
                canMoveTo = currentPiece.GetWalkMovements();
            }
            // Paints select piece's tile and the avaliable tiles to move.
            base.board.SelectPiece (currentPiece, canMoveTo);   
        }
    }

    /// <summary>
    /// Return a Movement if it exists within the list.
    /// </summary>
    private Movement GetMovementIfExists (ArrayList list, IntVector2 originalPos)
    {
        if (list == null)
        {
            return null;
        }
        foreach (Movement move in list)
        {
            if (move.getDestinyPosition().x == originalPos.x && 
                move.getDestinyPosition().y == originalPos.y)
            {
                return move;
            }
        }
        return null;
    }

    /// <summary>
    /// It's called when the movement chose by this player is finished.
    /// </summary>
    public override void NotifyEndOfMovement ()
    {
        // See if the piece can capture again.
        if (isCapturing)
        {
            canMoveTo = currentPiece.GetBestSucessiveCapture();
            if (canMoveTo.Count != 0)
            {
                isSucessiveCapture = true;
                base.board.DeselectTiles();
                return;
            }
        }

        // Try to promote
        ManPiece currentManPiece = currentPiece.GetComponent<ManPiece>();
        if (currentManPiece != null)
            currentManPiece.Promote();

        // Reset state and call the next turn.
        isSucessiveCapture = false;
        isCapturing = false;
        currentPiece = null;
        canMoveTo = null;
        base.board.DeselectTiles();
        base.board.DestroyCapturedPieces();
        this.gameController.NextTurn();
    }
}
