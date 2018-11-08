using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : Piece {

    /// <summary>
    /// Return a list of Movements that capture a piece given the current position.
    /// </summary>
    public override ArrayList GetCaptureMovements(IntVector2 currentPos, ArrayList path)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Return all the walk movements of the piece in a ArrayList.
    /// </summary>
    public override ArrayList GetWalkMovements()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    ///  Verify if the tile 'currentPosition + offset' has a enemy's piece and can be captured.
    /// </summary>
    protected override bool CanCapture(int offsetX, int offsetY, IntVector2 pos)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Verify if the tile 'currentPosition + offset' can be walked to.
    /// </summary>
    protected override bool CanWalk(int offsetX, int offsetY)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
