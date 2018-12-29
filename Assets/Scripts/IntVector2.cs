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
    
    public override string ToString()
    {
        return "<" + x + ", " + y + ">";
    }
}
