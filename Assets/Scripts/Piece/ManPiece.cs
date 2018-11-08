using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManPiece : Piece {
    
    protected string enemy_tag;
    protected int forward;

    protected ArrayList movementsTree;
    protected int biggestDepth;

    /// <summary>
    /// Return a list of Movements given the current position.
    /// </summary>
    public override ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path)
    {
        ArrayList possibleCaptureMovents = new ArrayList();

        if (CanCapture(1, 1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y + 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x + 2, currentPos.y + 2),
                base.board.GetTile(currentPos.x + 1, currentPos.y + 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(1, -1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x + 1, currentPos.y - 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x + 2, currentPos.y - 2),
                base.board.GetTile(currentPos.x + 1, currentPos.y - 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(-1, 1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y + 1)))
        {
            possibleCaptureMovents.Add(new Movement(currentPos,
                new IntVector2(currentPos.x - 2, currentPos.y + 2),
                base.board.GetTile(currentPos.x - 1, currentPos.y + 1).transform.GetChild(0).gameObject.GetComponent<Piece>()));
        }
        if (CanCapture(-1, -1, currentPos) && !alreadyCaptured(path, new IntVector2(currentPos.x - 1, currentPos.y - 1)))
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
            possibleWalkMovents.Add(new IntVector2(base.position.x + forward, base.position.y + 1));
        }
        if (CanWalk(forward, -1))
        {
            possibleWalkMovents.Add(new IntVector2(base.position.x + forward, base.position.y - 1));
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
            GameObject nextTile =
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

    /// <summary>
    /// Return a matrix with the best moves considering the Majority Law.
    /// </summary>
    /// <remarks>
    /// ---MAJORITY LAW---
    ///  If more than one capture mode is shown in the
    ///  same move, it is mandatory to execute the movement
    ///  that captures the largest number of pieces.
    /// </remarks>
    public ArrayList GetBestSucessiveCapture()
    {
        // Prepare the variables to contruct a tree.
        this.movementsTree = new ArrayList();
        this.biggestDepth = 0;
        ArrayList path = new ArrayList();
        // Retire the current piece of the board.
        Transform originalParent = transform.parent;
        Transform overlay = GameObject.FindGameObjectWithTag("OverLay").transform;
        transform.SetParent(overlay);

        // Contruct a tree recursively.
        contructTree(base.position, path , 0);

        // Put the current piece back to the board.
        transform.SetParent(originalParent);

        // Get just the moves with the majority law.
        ArrayList bestWays = ApplyMajorityLaw(movementsTree);

        // Get the first moves of each of the best sequences.
        ArrayList possibleMoves = new ArrayList();
        foreach (ArrayList list in bestWays)
        {
            if(list.Count > 0)
            {
                Movement move = (Movement)list[0];
                possibleMoves.Add(move.getDestinyPosition());
            }
            
        }

        return possibleMoves;
    }

    /// <summary>
    /// Greed Recursive method that return all sequence of capture movements.
    /// Also sets the biggestDepth variable.
    /// </summary>
    public void contructTree(IntVector2 currentPos, ArrayList path, int depth)
    {
        ArrayList possibleMoves = GetCaptureMovements(currentPos, path);

        // Stop when reach the leafs.
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

    /**
     * Given a matrix of Movements, return the longest ones.
     */
    public ArrayList ApplyMajorityLaw( ArrayList matrix)
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

    /**
     * Compare to see if a enemy already was captured based in it's position.
     */
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

    /**
     * Print a list o Movements.
     */
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