using System;
using System.Collections;

[Serializable]
public class KingPiece : Piece {

    /// <summary>
    /// Return a list of Movements that capture a piece given the current position.
    /// </summary>
    public override ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path)
    {
        ArrayList possibleCaptureMovements = new ArrayList();

        // Get the possible move in each diagonal direction.
        
        ArrayList upRight = SearchMovementInDirection(1, 1, currentPos, path);

        ArrayList upLeft = SearchMovementInDirection(1, -1, currentPos, path);
        

        ArrayList downRight = SearchMovementInDirection(-1, 1, currentPos, path);

        ArrayList downLeft = SearchMovementInDirection(-1, -1, currentPos, path);

        possibleCaptureMovements.AddRange(upRight);
        possibleCaptureMovements.AddRange(upLeft);
        possibleCaptureMovements.AddRange(downRight);
        possibleCaptureMovements.AddRange(downLeft);

        //Debug.Log(this.PrintMovements(possibleCaptureMovements));

        return possibleCaptureMovements;
    }

    /// <summary>
    /// Return all the walk movements of the piece in a ArrayList.
    /// </summary>
    public override ArrayList GetWalkMovements()
    {
        ArrayList possibleWalkMovents = new ArrayList();

        int offsetX = 1;
        int offsetY = 1;
        while(CanWalk(offsetX, offsetY))
        {
            possibleWalkMovents.Add( new Movement (base.position, 
                new IntVector2(base.position.x + offsetX, base.position.y + offsetY)));
            offsetX++;
            offsetY++;
        }

        offsetX = 1;
        offsetY = -1;
        while(CanWalk(offsetX, offsetY))
        {
            possibleWalkMovents.Add(new Movement(base.position,
                new IntVector2(base.position.x + offsetX, base.position.y + offsetY)));
            offsetX++;
            offsetY--;
        }

        offsetX = -1;
        offsetY = 1;
        while (CanWalk(offsetX, offsetY))
        {
            possibleWalkMovents.Add(new Movement(base.position,
                new IntVector2(base.position.x + offsetX, base.position.y + offsetY)));
            offsetX--;
            offsetY++;
        }

        offsetX = -1;
        offsetY = -1;
        while (CanWalk(offsetX, offsetY))
        {
            possibleWalkMovents.Add(new Movement(base.position,
                new IntVector2(base.position.x + offsetX, base.position.y + offsetY)));
            offsetX--;
            offsetY--;
        }

        return possibleWalkMovents;
    }

    /// <summary>
    /// Return true if this piece can capture the other one located in
    /// a given position + offset.
    /// </summary>
    protected override bool CanCapture(int offsetX, int offsetY, IntVector2 pos)
    {
        if (base.board.WithinBounds(pos.x + offsetX, pos.y + offsetY))
        {
            TileHandler nextTile =
                base.board.GetTile(pos.x + offsetX, pos.y + offsetY);
            /**
             * See if:
             * The nextTile is occupied.
             * Has a enemy Piece in there.
             * the enemy has NOT been captured yet.
             * and the following tile is within bounds.
             */
            if (nextTile.transform.childCount != 0
                && nextTile.transform.GetChild(0).CompareTag(enemy_tag)
                && !nextTile.transform.GetChild(0).GetComponent<Piece>().HasBeenCaptured()
                && base.board.WithinBounds(pos.x + 2 * offsetX, pos.y + 2 * offsetY))
            {
                nextTile =
                    base.board.GetTile(pos.x + 2 * offsetX, pos.y + 2 * offsetY);
                if (nextTile.transform.childCount == 0)
                {
                    return true;
                }

            }
        }

        return false;
    }

    /// <summary>
    /// Verify if the tile 'currentPosition + offset' can be walked to.
    /// </summary>
    protected override bool CanWalk(int offsetX, int offsetY)
    {
        if (base.board.WithinBounds(base.position.x + offsetX, base.position.y + offsetY))
        {

            TileHandler nextTile = base.board.GetTile(base.position.x + offsetX, base.position.y + offsetY);
            // See if the nextTile is occupied.
            if (nextTile.transform.childCount == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get the possible move in a defined diagonal direction.
    /// </summary>
    private ArrayList SearchMovementInDirection(int offsetX, int offsetY, IntVector2 currentPos, ArrayList path)
    {
        
        ArrayList possibleMoves = new ArrayList();
        // Get the currrent tile of this piece.
        TileHandler tile = base.board.GetTile(currentPos.x, currentPos.y);
        // Search until find a tile that can capture.
        while (tile != null)
        {   
            // Once find a tile that can capture, add all tile position that a free in that direction.
            if (CanCapture(offsetX, offsetY, tile.getPosition()) && 
                !AlreadyCaptured(path, new IntVector2(tile.getRow() + offsetX, tile.getColumn() + offsetY)))
            {
                Piece enemyPiece = base.board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY).
                    transform.GetChild(0).GetComponent<Piece>();
                tile = board.GetTile(tile.getRow() + 2 * offsetX, tile.getColumn() + 2 * offsetY)
                    .GetComponent<TileHandler>();
                while (tile != null && tile.transform.childCount == 0)
                {
                    possibleMoves.Add(new Movement( currentPos, tile.getPosition(), enemyPiece.GetPosition()));
                    tile = board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY);
                }
                break;
            }

            // Increment the current Tile.
            tile = base.board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY);

            // Finish if find a piece with the same tag.
            if (tile != null && tile.transform.childCount > 0)
            {
                break;
            }
        }

        return possibleMoves;
    }

}
