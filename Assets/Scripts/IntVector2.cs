using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The same as Vector2 but with int values
 */
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
