using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : Piece {

    protected string enemy_tag;

    protected ArrayList movementsTree;
    protected int biggestDepth;

    public override ArrayList GetBestSucessiveCapture()
    {
        return GetCaptureMovements( base.position, new ArrayList() ) ;
    }

    /// <summary>
    /// Return a list of Movements that capture a piece given the current position.
    /// </summary>
    public override ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path)
    {
        ArrayList possibleCaptureMovements = new ArrayList();

        // Get the possible move in each diagonal direction.
        
        ArrayList upRight = searchMovementInDirection(1, 1);

        ArrayList upLeft = searchMovementInDirection(1, -1);
        

        ArrayList downRight = searchMovementInDirection(-1, 1);

        ArrayList downLeft = searchMovementInDirection(-1, -1);

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
        if (base.board.WithinBounds((int)base.position.x + offsetX, (int)base.position.y + offsetY))
        {

            TileHandler nextTile = base.board.GetTile((int)base.position.x + offsetX, (int)base.position.y + offsetY);
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
    private ArrayList searchMovementInDirection(int offsetX, int offsetY)
    {
        
        ArrayList possibleMoves = new ArrayList();
        // Get the currrent tile of this piece.
        TileHandler tile = base.board.GetTile(base.position.x, base.position.y);
        // Search until find a tile that can capture.
        while (tile != null)
        {
            //Debug.Log("passing by " + tile.getPosition().ToString());
            
            // Once find a tile that can capture, add all tile position that a free in that direction.
            if (CanCapture(offsetX, offsetY, tile.getPosition()))
            {
                Piece enemyPiece = base.board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY).
                    transform.GetChild(0).GetComponent<Piece>();
                tile = board.GetTile(tile.getRow() + 2 * offsetX, tile.getColumn() + 2 * offsetY)
                    .GetComponent<TileHandler>();
                while (tile != null && tile.transform.childCount == 0)
                {
                    possibleMoves.Add(new Movement(base.position, tile.getPosition(), enemyPiece ));
                    tile = board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY);
                }
                break;
            }

            //Debug.Log("next is " + new IntVector2(tile.getRow() + offsetX, tile.getColumn() + offsetY).ToString() );
            tile = base.board.GetTile(tile.getRow() + offsetX, tile.getColumn() + offsetY);

            // Finish if find a piece with the same tag.
            if (tile != null && tile.transform.childCount > 0 && tile.transform.GetChild(0).CompareTag(this.tag))
            {
                //Debug.Log("break");
                break;
            }
        }

        return possibleMoves;
    }

    private string PrintMovements(ArrayList list)
    {
        string result = "Up left\n";
        foreach (Movement move in list)
        {
            result += move.getDestinyPosition().ToString() + " - ";
        }
        result += "final.";
        return result;
    }
}
