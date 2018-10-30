using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement {

    IntVector2 originalPosition;
    IntVector2 destinyPosition;
    Piece capturedPiece;


    public Movement(IntVector2 originalPosition, IntVector2 destinyPosition)
    {
        this.originalPosition = originalPosition;
        this.destinyPosition = destinyPosition;
    }

    public Movement(IntVector2 originalPosition, IntVector2 destinyPosition, Piece capturedPiece)
    {
        this.originalPosition = originalPosition;
        this.destinyPosition = destinyPosition;
        this.capturedPiece = capturedPiece;
    }

    public bool hasCapturedAnEnemy()
    {
        if (capturedPiece != null)
            return true;
        return false;
    }

    public IntVector2 getOriginalPosition()
    {
        return this.originalPosition;
    }

    public IntVector2 getDestinyPosition()
    {
        return this.destinyPosition;
    }

    public Piece getCapturedPiece()
    {
        return this.capturedPiece;
    }

    public void SetCapturedPiece( Piece capturedPiece)
    {
        this.capturedPiece = capturedPiece;
    }

    public override string ToString()
    {
        string result = "from " + originalPosition.ToString() + " to " + destinyPosition.ToString();
        if (hasCapturedAnEnemy())
        {
            result += " Capturing";
        }

        return result;
    }
}
