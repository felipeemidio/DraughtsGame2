using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Piece : MonoBehaviour {

    protected bool hasBeenCaptured = false;
    protected IntVector2 position;
    protected Board board;

    public virtual void Start()
    {
        this.SetCurrentPosition();
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
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
    public bool SetCurrentPosition()
    {
        // Get current position.
        TileHandler parentTile = transform.parent.GetComponent<TileHandler>();
        position = new IntVector2(parentTile.getRow(), parentTile.getColumn());
        return true;

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


}
