using System;

/**
 * The same as Vector2 but with integer values.
 */
[Serializable]
public class IntVector2 {

    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        IntVector2 vector = obj as IntVector2;
        return vector != null &&
               x == vector.x &&
               y == vector.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return "<" + x + ", " + y + ">";
    }
}
