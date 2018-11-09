using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHandler : MonoBehaviour {
    
    // Private Variables.
    private static Dictionary<char, int> collumDic = new Dictionary<char, int>()
    {
        {'A', 1},
        {'B', 2},
        {'C', 3},
        {'D', 4},
        {'E', 5},
        {'F', 6},
        {'G', 7},
        {'H', 8},
    };
    private Board board;
    private int row;
    private int column;

	// Use this for initialization
	void Awake () {

        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board> ();
        
        // Add the 'clickHandler' method to the onClick listener.
        Button btn = GetComponent<Button> ();
        btn.onClick.AddListener (ClickHandler);

        // Get Tile Position (row and collumn) by it's name.
        string tileName = transform.name;
        string tilePosition = tileName.Substring(tileName.Length - 2, 2);
        row = (int)System.Char.GetNumericValue(tilePosition[0]); ;
        column = collumDic[tilePosition[1]];
        //Debug.Log("row: " + tilePosition[0] + " collumn: " + tilePosition[1]);
    }

    public void ClickHandler()
    {
        board.TileClicked ( this, row, column );
    }

    public int getRow()
    {
        return this.row;
    }

    public int getColumn()
    {
        return this.column;
    }

    public IntVector2 getPosition()
    {
        return new IntVector2(this.row, this.column);
    }
}
