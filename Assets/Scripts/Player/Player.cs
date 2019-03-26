using System.Collections;
using UnityEngine;

public class Player : AbstractPlayer {

    private ArrayList canMoveTo = null;
    

    public Player ()
    {
        base.board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
    }

    /// <summary>
    /// Sign the start of that player's turn.
    /// </summary>
    public override void Play ()
    {
        base.isCapturing = base.SomePieceCanCapture(base.board.GetPlayerPieces());
    }

    public void SelectionHandler (TileHandler tile)
    {
        base.board.ResetPossibleMovements(canMoveTo);
        bool hasPiece = tile.HasChild();  
        // In case the player want to move a piece.
        if (base.currentPiece != null && !hasPiece)
        {
            // If that tile is in the 'canMoveTo', then move the current piece.
            Movement movement = GetMovementIfExists (canMoveTo, tile.getPosition());
            if (movement != null)
            {
                base.board.MovePiece (movement);
            }    
        }
        // In case the player try to select a piece in a sucessive capture.
        else if (base.currentPiece != null && base.isSucessiveCapture && hasPiece)
        {
            // If the movement is a sucessive movement, then that piece is the only playable.
            if (base.currentPiece == tile.GetChild().GetComponent<Piece>())
            {
                // Paints select piece's tile and the avaliable tiles to move.
                base.board.SelectPiece(base.currentPiece, canMoveTo);
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
            base.currentPiece = tile.GetChild ().GetComponent<Piece>();
            if (base.currentPiece == null || !base.currentPiece.CompareTag("BluePiece"))
                return;
            
            // Store the possible movements in a variable 'canMoveTo'.
            canMoveTo = base.currentPiece.GetBestSucessiveCapture();
            if(canMoveTo.Count == 0 && !base.isCapturing)
            {
                canMoveTo = base.currentPiece.GetWalkMovements();
            }
            // Paints select piece's tile and the avaliable tiles to move.
            base.board.SelectPiece (base.currentPiece, canMoveTo);   
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
            if ( originalPos.Equals(move.getDestinyPosition()) )
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
        if (base.isCapturing)
        {
            canMoveTo = base.currentPiece.GetBestSucessiveCapture();
            if (canMoveTo.Count != 0)
            {
                isSucessiveCapture = true;
                base.board.DeselectTiles();
                base.board.SelectPiece(base.currentPiece, canMoveTo);
                return;
            }
        }

        // Try to promote
        ManPiece currentManPiece = base.currentPiece.GetComponent<ManPiece>();
        if (currentManPiece != null)
            currentManPiece.Promote();

        // Reset state and call the next turn.
        isSucessiveCapture = false;
        canMoveTo = null;
        base.isCapturing = false;
        base.currentPiece = null;
        base.board.DeselectTiles();
        base.board.DestroyCapturedPieces();
    }
}
