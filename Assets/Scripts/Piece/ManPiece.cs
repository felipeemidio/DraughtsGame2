using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManPiece : Piece {

    protected string enemy_tag;
    protected int forward;

    protected ArrayList movementsTree;
    private int biggestDepth;

    protected override bool CanCapture(int offsetX, int offsetY)
    {
        if (base.board.WithinBounds((int)base.position.x + offsetX, (int)base.position.y + offsetY))
        {
            GameObject nextTile = 
                base.board.GetTile((int)base.position.x + offsetX, (int)base.position.y + offsetY);
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
                && base.board.WithinBounds((int)base.position.x + 2 * offsetX, (int)base.position.y + 2 * offsetY))
            {
                nextTile = 
                    base.board.GetTile((int)base.position.x + 2 * offsetX, (int)base.position.y + 2 * offsetY);
                if (nextTile.transform.childCount == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override bool CanWalk(int offsetX, int offsetY)
    {
        if (base.board.WithinBounds((int)base.position.x + offsetX, (int)base.position.y + offsetY))
        {
            
            GameObject nextTile = base.board.GetTile((int)base.position.x + offsetX, (int)base.position.y + offsetY);
            // See if the nextTile is occupied.
            if (nextTile.transform.childCount == 0)
            {
                return true;
            }
        }
        return false;
    }

    public override ArrayList GetCaptureMovements()
    {
        ArrayList possibleCaptureMovents = new ArrayList();

        if (CanCapture(1, 1))
        {
            possibleCaptureMovents.Add(new Vector2(base.position.x + 2, base.position.y + 2));
        }
        if (CanCapture(1, -1))
        {
            possibleCaptureMovents.Add(new Vector2(base.position.x + 2, base.position.y - 2));
        }
        if (CanCapture(-1, 1))
        {
            possibleCaptureMovents.Add(new Vector2(base.position.x - 2, base.position.y + 2));
        }
        if (CanCapture(-1, -1))
        {
            possibleCaptureMovents.Add(new Vector2(base.position.x - 2, base.position.y - 2));
        }

        return possibleCaptureMovents;
    }

   

    public override ArrayList GetWalkMovements()
    {
        ArrayList possibleWalkMovents = new ArrayList();
        if (CanWalk(forward, 1))
        {
            possibleWalkMovents.Add(new Vector2(base.position.x + forward, base.position.y + 1));
        }
        if (CanWalk(forward, -1))
        {
            possibleWalkMovents.Add(new Vector2(base.position.x + forward, base.position.y - 1));
        }
        return possibleWalkMovents;
    }

    public ArrayList GetBestSucessiveCapture()
    {
        this.movementsTree = new ArrayList();
        this.biggestDepth = 0;
        ArrayList path = new ArrayList();

        Transform originalParent = transform.parent;
        Transform overlay = GameObject.FindGameObjectWithTag("OverLay").transform;
        transform.SetParent(overlay);

        contructTree(base.position, path , 0);

        transform.SetParent(originalParent);
        return ExtractBiggerSequences(movementsTree);
    }


    public void contructTree(IntVector2 currentPos, ArrayList path, int depth)
    {
        ArrayList possibleMoves = GetCaptureMovements2(currentPos, path);

        if (possibleMoves.Count == 0 )
        {
            movementsTree.Add(path.Clone());
            if(this.biggestDepth < depth)
            {
                this.biggestDepth = depth;
            }
            return;
        }
        foreach (Movement move in possibleMoves)
        {
            path.Add(move);
            contructTree(move.getDestinyPosition(), path, depth+1);
            path.Remove(move);
        }

    }

    public ArrayList GetCaptureMovements2(IntVector2 currentPos, ArrayList path)
    {
        ArrayList possibleCaptureMovents = new ArrayList();

        if (CanCapture2(1, 1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y + 1)) )
        {
            possibleCaptureMovents.Add(new Movement(currentPos, 
                new IntVector2(currentPos.x + 2, currentPos.y + 2), 
                base.board.GetTile(currentPos.x+1, currentPos.y+1).transform.GetChild(0).gameObject.GetComponent<Piece>() ));
        }
        if (CanCapture2(1, -1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y - 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x + 2, currentPos.y - 2),
                base.board.GetTile(currentPos.x + 1, currentPos.y - 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture2(-1, 1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y + 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x - 2, currentPos.y + 2),
                base.board.GetTile(currentPos.x - 1, currentPos.y + 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture2(-1, -1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y - 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x - 2, currentPos.y - 2),
                base.board.GetTile(currentPos.x - 1, currentPos.y - 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }

        return possibleCaptureMovents;
    }


    protected  bool CanCapture2(int offsetX, int offsetY, IntVector2 pos)
    {
        if (base.board.WithinBounds( pos.x + offsetX,  pos.y + offsetY))
        {
            GameObject nextTile =
                base.board.GetTile( pos.x + offsetX, pos.y + offsetY);
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
                && base.board.WithinBounds( pos.x + 2 * offsetX, pos.y + 2 * offsetY))
            {
                nextTile =
                    base.board.GetTile( pos.x + 2 * offsetX,  pos.y + 2 * offsetY);
                if (nextTile.transform.childCount == 0 )
                {
                    return true;
                }

            }
        }

        return false;
    }

    public ArrayList ExtractBiggerSequences( ArrayList matrix)
    {

        ArrayList result = new ArrayList();
        foreach (ArrayList list in matrix)
        {
            if (list.Count == this.biggestDepth)
            {
                result.Add(list);
            }
        }

        return result;
    }

    private bool alreadyCaptured(ArrayList path, IntVector2 enemyPos)
    {
        foreach( Movement move in path)
        {
            IntVector2 movePiecePosition = move.getCapturedPiece().GetPosition();
            if (move.hasCapturedAnEnemy() && movePiecePosition.x == enemyPos.x && movePiecePosition.y == enemyPos.y)
            {
                return true;
            }
        }
        return false;
    }

    private void printMovements(ArrayList list)
    {
        string message = "Movements - ";
        foreach (Movement item in list)
        {
            message += "(" + item.ToString() + ")\n";
        }
        Debug.Log(message);
    }

}
