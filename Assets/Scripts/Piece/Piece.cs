using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Piece : MonoBehaviour {

    protected bool hasBeenCaptured = false;
    protected int biggestDepth;
    protected string enemy_tag;
    protected IntVector2 position;
    protected ArrayList movementsTree;
    protected Board board;

    public virtual void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        this.SetCurrentPosition();
    }

    /// <summary>
    ///  set that the piece has been captured and
    ///  will be destroyed in the end of the turn.
    ///  Also change the opacity of the piece.
    /// </summary>
    public void Capture()
    {
        hasBeenCaptured = true;
        this.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.7f);
    }


    /// <summary>
    ///  Set the current position of the piece in the board by the parent tile.
    /// </summary>
    public virtual bool SetCurrentPosition()
    {
        // Get current position.
        TileHandler parentTile = transform.parent.GetComponent<TileHandler>();
        position = new IntVector2(parentTile.getRow(), parentTile.getColumn());
        return true;

    }

    /**
     * Given a matrix of Movements, return the longest ones.
     */
    protected ArrayList ApplyMajorityLaw(ArrayList matrix)
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

    public void ChangePosition(IntVector2 pos)
    {
        this.position = pos;
    }

    public IntVector2 GetPosition()
    {
        return this.position;
    }

    public bool HasBeenCaptured()
    {
        return hasBeenCaptured;
    }

    /**
     * Compare to see if a enemy already was captured based in it's position.
     */
    protected bool AlreadyCaptured(ArrayList path, IntVector2 enemyPos)
    {
        foreach (Movement move in path)
        {
            IntVector2 movePiecePosition = move.getCapturedPiece().GetPosition();
            if (move.hasCapturedAnEnemy() && 
                movePiecePosition.x == enemyPos.x && 
                movePiecePosition.y == enemyPos.y)
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
        ContructTree(this.position, path, 0);

        // Put the current piece back to the board.
        transform.SetParent(originalParent);

        // Get just the moves with the majority law.
        ArrayList bestWays = this.ApplyMajorityLaw(movementsTree);

        // Get the first moves of each of the best sequences.
        ArrayList possibleMoves = new ArrayList();
        foreach (ArrayList list in bestWays)
        {
            if (list.Count > 0)
            {
                Movement move = (Movement)list[0];
                possibleMoves.Add(move);
            }
        }

        return possibleMoves;
    }

    /// <summary>
    /// Greed Recursive method that return all sequence of capture movements.
    /// Also sets the biggestDepth variable.
    /// </summary>
    protected void ContructTree(IntVector2 currentPos, ArrayList path, int depth)
    {
        ArrayList possibleMoves = GetCaptureMovements(currentPos, path);
        /*
        Debug.Log("CurrentPos: " + currentPos.ToString() + 
            "\nPath " + PrintMovements(path) +
            "\nPossible Moves " + PrintMovements(possibleMoves));
        */
        // Stop when reach the leafs.
        if (possibleMoves.Count == 0)
        {
            movementsTree.Add(path.Clone());
            if (this.biggestDepth < depth)
            {
                this.biggestDepth = depth;
            }
            return;
        }
        // Recursive call of this method
        foreach (Movement move in possibleMoves)
        {

            path.Add(move);
            ContructTree(move.getDestinyPosition(), path, depth + 1);
            path.Remove(move);
        }
    }

    /// <summary>
    /// Return all the walk movements of the piece in a ArrayList.
    /// </summary>
    public abstract ArrayList GetWalkMovements();

    /// <summary>
    /// Return a list of Movements that capture a piece given the current position.
    /// </summary>
    public abstract ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path);

    /// <summary>
    /// Verify if the tile 'currentPosition + offset' can be walked to.
    /// </summary>
    protected abstract bool CanWalk(int offsetX, int offsetY);

    /// <summary>
    ///  Verify if the tile 'currentPosition + offset' has a enemy's piece and can be captured.
    /// </summary>
    protected abstract bool CanCapture(int offsetX, int offsetY, IntVector2 pos);


    public ArrayList GetCaptureMovements()
    {
        return GetCaptureMovements(this.position, new ArrayList());
    }

    /**
     * TODO: remove when doesn't need anymore.
     * UTILS METHODS
     */
    protected string PrintMovements(ArrayList list)
    {
        string result = "";
        foreach (Movement move in list)
        {
            result += move.ToString() + " - ";
        }
        result += "final.";
        return result;
    }

    protected string PrintMatrix(ArrayList matrix)
    {
        string result = "";
        foreach (ArrayList list in matrix)
        {
            foreach (Movement move in list)
            {
                result += move.ToString() + " - ";
            }
            result += "final.\n";

        }
        return result;
    }
}
