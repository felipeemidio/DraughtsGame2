using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManPiece : Piece {
    
    protected int forward;
    protected string kingVersionPath;
    
    /// <summary>
    /// Return a list of Movements given the current position.
    /// </summary>
    public override ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path)
    {
        ArrayList possibleCaptureMovents = new ArrayList();

        if (CanCapture(1, 1, currentPos) && !base.AlreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y + 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x + 2, currentPos.y + 2),
                base.board.GetTile(currentPos.x + 1, currentPos.y + 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(1, -1, currentPos) && !base.AlreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y - 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x + 2, currentPos.y - 2),
                base.board.GetTile(currentPos.x + 1, currentPos.y - 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(-1, 1, currentPos) && !base.AlreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y + 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x - 2, currentPos.y + 2),
                base.board.GetTile(currentPos.x - 1, currentPos.y + 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(-1, -1, currentPos) && !base.AlreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y - 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x - 2, currentPos.y - 2),
                base.board.GetTile(currentPos.x - 1, currentPos.y - 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }

        return possibleCaptureMovents;
    }

    /// <summary>
    /// Return all the walk movements of the piece in a ArrayList.
    /// </summary>
    public override ArrayList GetWalkMovements()
    {
        ArrayList possibleWalkMovents = new ArrayList();
        if (CanWalk(forward, 1))
        {
            possibleWalkMovents.Add(new Movement(base.position, new IntVector2(base.position.x + forward, base.position.y + 1)) );
        }
        if (CanWalk(forward, -1))
        {
            possibleWalkMovents.Add(new Movement(base.position, new IntVector2(base.position.x + forward, base.position.y - 1)));
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

    //// <summary>
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
    /// Create a new king Piece and replace this one.
    /// </summary>
    public void Promote()
    {
        // Find the respective promotion line.
        int promotionLine = 0;
        if (this.forward == 1)
        {
            promotionLine = 8;
        }
         
        if(base.position.x == promotionLine)
        {
            // Create a new King Piece and set it in the same position as this.
            GameObject kingVersion = Resources.Load<GameObject>(this.kingVersionPath);
            if(kingVersion == null)
            {
                Debug.LogError("Path for piece promotion not founded.");
            }
            else
            {
                Piece newPiece = Instantiate(kingVersion, transform.parent.transform, false).GetComponent<Piece>();
                if (newPiece == null)
                {
                    Debug.LogError("Can't create a new piece.");
                    return;
                }
                newPiece.SetCurrentPosition();
                // Destroy this piece
                Destroy(this.gameObject);
                // Refresh the active pieces.
                base.board.RefreshAllPieces();
            }
        }
    }
}