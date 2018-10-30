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

    /**
     * set that the piece has been captured and
     * will be destroyed in the end of the turn.
     * Also change the opacity of the piece.
     */
    public void Capture()
    {
        hasBeenCaptured = true;
        this.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.7f);
    }


    /**
     * Set the current position of the piece in the board.
     */
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

    /**
     * Return all the walk movements of the piece in a ArrayList.
     */
    public abstract ArrayList GetWalkMovements();

    /**
     * Return all the capture movements of the piece in a ArrayList.
     */
    public abstract ArrayList GetCaptureMovements();

    /**
     * Verify if the tile 'currentPosition + offset' can be walked to.
     */
    protected abstract bool CanWalk(int offsetX, int offsetY);

    /**
     * Verify if the tile 'currentPosition + offset' has a enemy's piece and can be captured.
     */
    protected abstract bool CanCapture(int offsetX, int offsetY);



}
