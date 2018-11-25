using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    private Board board;
    private Piece currentPiece = null;
    private ArrayList canMoveTo = null;
    private bool somePieceCanCapture = false;
    private bool isCapturing = false;
    private GameController gameController;
    private bool isSucessiveCapture = false;

    public Player ()
    {
        this.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        this.gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }

    public void Play ()
    {
        somePieceCanCapture = this.board.SomePieceCanCapture();
    }

    public void SelectionHandler (TileHandler tile)
    {
        this.board.ResetPossibleMovements(canMoveTo);
        bool hasPiece = tile.HasChild();  
        // In case the player want to move a piece.
        if (currentPiece != null && !isSucessiveCapture && !hasPiece)
        {
            // If that tile is in the 'canMoveTo', then move the current piece.
            Movement movement = GetMovementIfExists (canMoveTo, tile.getPosition());
            if (movement != null)
            {
                this.board.MovePiece (movement, this);
            }    
        }
        // In case the player are in a sucessive capture.
        else if (currentPiece != null && isSucessiveCapture && hasPiece)
        {
            // If the movement is a sucessive movement, then that piece is the only playable.
            if (currentPiece == tile.GetChild().GetComponent<Piece>())
            {
                // Paints select piece's tile and the avaliable tiles to move.
                this.board.SelectPiece(currentPiece, canMoveTo);
            }
            else
            {
                this.board.SelectPiece(tile.GetChild().GetComponent<Piece>(), new ArrayList());
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
            else if (canMoveTo.Count > 0)
            {
                isCapturing = true;
            }
            // Paints select piece's tile and the avaliable tiles to move.
            this.board.SelectPiece (currentPiece, canMoveTo);   
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
    /// This method should be called when the board finish doing a move for the player.
    /// </summary>
    public void NotifyEndOfMovement ()
    {
        // See if the piececan capture again.
        if (isCapturing)
        {
            canMoveTo = currentPiece.GetBestSucessiveCapture();
            if (canMoveTo.Count != 0)
            {
                isSucessiveCapture = true;
                this.board.DeselectTile();
                return;
            }
        }

        // Reset state and call the next turn.
        isSucessiveCapture = false;
        currentPiece = null;
        canMoveTo = null;
        this.board.DeselectTile();
        this.gameController.NextTurn();
    }
}
