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
        // Get all possible capture movements. if exists.
        ArrayList possibleMoves = new ArrayList();
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
    /// 2 - Lose the piece if not move.
    /// -2 - Is protecting an ally.
    /// -5 - Movement that cost the piece. 
    /// </remarks>
    public int AdaptationScore(Movement move)
    {
        int score = 1;
        if (move.hasCapturedAnEnemy())
            score = 3;

        if (CanLostPiece(move))
        {
            score += -5;
        }
        if (IsProtectingAFriend(move))
        {
            score -= 2;
        }
        if (CanProtectAFriend(move))
        {
            score += 2;
        }
        if (StayLoseAPiece(move))
        {
            score += 2;
        }

        return score;
    }

    /// <summary>
    /// Return if a piece will be captured if do not move.
    /// </summary>
    private bool StayLoseAPiece(Movement move)
    {
        IntVector2 currentPosition = move.getOriginalPosition();

        return (CanCaptureMe(currentPosition, 1, 1) ||
            CanCaptureMe(currentPosition, 1, -1) ||
            CanCaptureMe(currentPosition, -1, 1) ||
            CanCaptureMe(currentPosition, -1, -1));
    }

    /// <summary>
    /// Return if a piece can be captured by a piece in the Pos+offSet tile.
    /// </summary>
    private bool CanCaptureMe(IntVector2 pos, int offSetX, int offSetY)
    {
        return this.HasABluePiece(pos, offSetX, offSetY) && this.IsFree(pos, -offSetX, -offSetY);
    }

    /// <summary>
    /// Return if a piece in a given position can be captured.
    /// </summary>
    private bool CanLostPiece(Movement move)
    {
        Transform pieceT = null;
        Transform originalParent = null;
        bool result = false;
        
        // Retire the piece to be captured of the board.
        if (move.hasCapturedAnEnemy())
        {
            pieceT = board.GetTile(move.getCapturedPiece()).GetChild().transform;
            originalParent = pieceT.parent;
            Transform overlay = GameObject.FindGameObjectWithTag("OverLay").transform;
            pieceT.SetParent(overlay);
        }

        // See if has a blue piece in the corners or 
        // the movement go to the extreme sides of the board.
        IntVector2 pos = move.getDestinyPosition();
        if ((pos.y != 1 && pos.y != 8) &&
            (this.HasABluePiece(pos, 1, 1) || 
            this.HasABluePiece(pos, -1, 1) ||
            this.HasABluePiece(pos, 1, -1) || 
            this.HasABluePiece(pos, -1, -1)))
        {
            result = true;
        }

        // Put the piece to be captured back to the board.
        if (move.hasCapturedAnEnemy())
            pieceT.SetParent(originalParent);

        return result;
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

    private bool IsFree (IntVector2 pos, int offsetX, int offsetY)
    {
        return this.IsFree(new IntVector2(pos.x + offsetX, pos.y + offsetY)); 
    }

    private bool IsFree(IntVector2 pos)
    {
        TileHandler tile = this.board.GetTile(pos);
        return (tile != null && !tile.HasChild());
    }

    /// <summary>
    /// Return if there is a piece with a given tag in the given position.
    /// </summary>
    private bool HasPieceWithTag( IntVector2 position, string tag)
    {
        TileHandler tile = this.board.GetTile(position); 
        if (tile != null && tile.HasChild() && tile.GetChild().CompareTag(tag))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Return if there is a king piece with given in a given direction by the given position.
    /// </summary>
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

    /// <summary>
    /// Return if a movement can protect a friend.
    /// </summary>
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

    /// <summary>
    /// Return if the current position is protecting a friend.
    /// </summary>
    private bool IsProtectingAFriend(Movement move)
    {
        if (this.IsProtectingAFriendInDirection(move, 1, 1) ||
            this.IsProtectingAFriendInDirection(move, 1, -1) ||
            this.IsProtectingAFriendInDirection(move, -1, 1) ||
            this.IsProtectingAFriendInDirection(move, -1, -1))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Return if a movement can protect a friend in a given direction.
    /// </summary>
    private bool CanProtectAFriendInDirection(Movement move, int OffsetX, int OffsetY)
    {
        // Do not analyse the direction the player is coming.
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

    /// <summary>
    /// Return if the current position is protecting a friend in a given direction.
    /// </summary>
    private bool IsProtectingAFriendInDirection(Movement move, int offsetX, int offsetY)
    {
        // Do not analyse the direction the player is coming.
        IntVector2 pos = move.getOriginalPosition();

        IntVector2 nextPosition = new IntVector2(pos.x + offsetX, pos.y + offsetY);
        if (this.HasPieceWithTag(nextPosition, "WhitePiece") &&
            this.HasABluePiece(nextPosition, offsetX, offsetY))
        {
            return true;
        }

        return false;
    }
}
