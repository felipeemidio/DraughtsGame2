using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGeneticAlgorithm {

    private Board board;

    public MyGeneticAlgorithm(Board board)
    {
        this.board = board;
    }

    /// <summary>
    /// Return all mutation from the pieces of a player.
    /// </summary>
    public ArrayList GenerateMutations (AbstractPlayer absPlayer, ArrayList pieces)
    {
        ArrayList possibleMoves = new ArrayList();
        // Get all possible capture movements. if exists.
        if (absPlayer.SomePieceCanCapture(pieces))
        {
            absPlayer.SetIsCapturing(true);
            foreach (Piece piece in pieces)
            {
                possibleMoves.AddRange(piece.GetBestSucessiveCapture());
            }
        }
        // Get all possible walk movements if anyone can capture.
        else
        {
            foreach (Piece piece in pieces)
            {
                possibleMoves.AddRange(piece.GetWalkMovements());
            }
        }

        return possibleMoves;
    }

    /// <summary>
    /// Return the adaptation score of a movement.
    /// </summary>
    /// <remarks>
    /// 1 - Walk.
    /// 3 - Capture.
    /// 2 - Protect an ally.
    /// -5 - Movement that cost the piece. 
    /// </remarks>
    public int AdaptationScore(Movement move)
    {
        int score = 1;
        /*
        if (move.hasCapturedAnEnemy())
        {
            score = 3;
        }
        */
        if (CanLostPiece(move))
        {
            score = -5;
        }
        else if (CanProtectAFriend(move))
        {
            Debug.Log("Can protect with move " + move.ToString());
            score = 2;
        }

        return score;
    }

    /// <summary>
    /// Return if a piece in a given position can be captured.
    /// </summary>
    private bool CanLostPiece(Movement move)
    {
        Transform pieceT = null;
        Transform originalParent = null;
        if (move.hasCapturedAnEnemy())
        {
            pieceT = move.getCapturedPiece().transform;
            // Retire the current piece of the board.
            originalParent = pieceT.parent;
            Transform overlay = GameObject.FindGameObjectWithTag("OverLay").transform;
            pieceT.SetParent(overlay);
        }

        IntVector2 pos = move.getDestinyPosition();
        if (this.HasABluePiece(pos, 1, 1) ||
            this.HasABluePiece(pos, -1, 1) ||
            this.HasABluePiece(pos, 1, -1) ||
            this.HasABluePiece(pos, -1, -1))
        {
            if (move.hasCapturedAnEnemy())
                pieceT.SetParent(originalParent);
            return true;
        }

        if (move.hasCapturedAnEnemy())
            pieceT.SetParent(originalParent);
        return false;
    }

    /// <summary>
    /// Return if there is a blue piece in a given tile.
    /// </summary>
    private bool HasABluePiece(IntVector2 position,  int offsetX, int offsetY)
    {
        IntVector2 nextPosition = new IntVector2(position.x + offsetX, position.y + offsetY);
        if (HasPieceWithTag(nextPosition,"BluePiece") ||
            HasKingWithTagByDirection(position, "BluePiece", offsetX, offsetY))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Return if there is a blue piece in a given tile.
    /// </summary>
    private bool HasAWhitePiece(IntVector2 position, int offsetX, int offsetY)
    {
        if (HasPieceWithTag(new IntVector2(position.x + offsetX, position.y + offsetY), "WhitePiece") ||
            HasKingWithTagByDirection(position, "WhitePiece", offsetX, offsetY))
        {
            return true;
        }
        return false;
    }
    
    private bool HasPieceWithTag( IntVector2 position, string tag)
    {
        TileHandler tile = this.board.GetTile(position); 
        if (tile != null && tile.HasChild() && tile.GetChild().CompareTag(tag))
        {
            return true;
        }
        return false;
    }
    
    private bool HasKingWithTagByDirection(IntVector2 position, string tag, int offsetX, int offsetY)
    {

        TileHandler currentTile = this.board.GetTile(position);
        IntVector2 currentPosition = currentTile.getPosition();
        currentPosition.x += offsetX;
        currentPosition.y += offsetY;

        int counter = 0;
        while (this.board.WithinBounds(currentPosition.x, currentPosition.y))
        {
            //Debug.Log("currentPosition: " + currentPosition.ToString() + " offset " + offsetX);
            if(counter > 10)
            {
                Debug.Log("breaking when not should");
                break;
            }
            else
            {
                counter++;
            }

            currentTile = this.board.GetTile(currentPosition);
            if (currentTile != null && currentTile.HasChild())
            {
                if (currentTile.GetChild().CompareTag(tag) &&
                    currentTile.GetChild().GetComponent<KingPiece>() != null)
                    return true;
                else
                    break;
            }
            currentPosition.x += offsetX;
            currentPosition.y += offsetY;
        }

        
        return false;
    }

    private bool CanProtectAFriend(Movement move)
    {
        
        if (this.CanProtectAFriendInDirection(move, 1, 1)  ||
            this.CanProtectAFriendInDirection(move, 1, -1) ||
            this.CanProtectAFriendInDirection(move, -1, 1) ||
            this.CanProtectAFriendInDirection(move, -1, -1) )
        {
            return true;
        }
        return false;
    }

    private bool CanProtectAFriendInDirection(Movement move, int OffsetX, int OffsetY)
    {

        IntVector2 pos = move.getDestinyPosition();
        if (-OffsetX == pos.x - move.getOriginalPosition().x && 
           -OffsetY == pos.y - move.getOriginalPosition().y)
        {
            return false;
        }

        IntVector2 nextPosition = new IntVector2(pos.x + OffsetX, pos.y + OffsetY);
        if (this.HasPieceWithTag(nextPosition, "WhitePiece") &&
            this.HasABluePiece(nextPosition, OffsetX, OffsetY))
        {
            return true;
        }
        return false;
    }
}
