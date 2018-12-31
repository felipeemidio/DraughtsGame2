using System;
using System.Collections.Generic;

/**
 * Descibe a movement that can be done in the board.
 */
[Serializable]
public class Movement {

    IntVector2 originalPosition;
    IntVector2 destinyPosition;
    IntVector2 capturedPiece;


    public Movement(IntVector2 originalPosition, IntVector2 destinyPosition)
    {
        this.originalPosition = originalPosition;
        this.destinyPosition = destinyPosition;
    }

    public Movement(IntVector2 originalPosition, IntVector2 destinyPosition, IntVector2 capturedPiece)
    {
        this.originalPosition = originalPosition;
        this.destinyPosition = destinyPosition;
        this.capturedPiece = capturedPiece;
    }

    /**
     * See if the movement has captured a enemy.
     */
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

    public IntVector2 getCapturedPiece()
    {
        return this.capturedPiece;
    }

    public void SetCapturedPiece( IntVector2 capturedPiece)
    {
        this.capturedPiece = capturedPiece;
    }

    public override string ToString()
    {
        string result = "from " + originalPosition.ToString() + " to " + destinyPosition.ToString();
        if (hasCapturedAnEnemy())
        {
            result += " Capturing in " + this.capturedPiece.ToString();
        }

        return result;
    }

    public override bool Equals(object obj)
    {
        Movement movement = obj as Movement;
        return movement != null &&
               EqualityComparer<IntVector2>.Default.Equals(originalPosition, movement.originalPosition) &&
               EqualityComparer<IntVector2>.Default.Equals(destinyPosition, movement.destinyPosition) &&
               EqualityComparer<IntVector2>.Default.Equals(capturedPiece, movement.capturedPiece);
    }

    public override int GetHashCode()
    {
        var hashCode = 869111550;
        hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(originalPosition);
        hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(destinyPosition);
        hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(capturedPiece);
        return hashCode;
    }
}
