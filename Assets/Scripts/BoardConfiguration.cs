using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardConfiguration {
    readonly string boardConfiguration;
    readonly ArrayList movements;
    readonly ArrayList valuesByMovement;

    public BoardConfiguration(string boardConfiguration)
    {
        this.boardConfiguration = boardConfiguration;
        movements = new ArrayList();
        valuesByMovement = new ArrayList();
    }

    public ArrayList GetValuesByMovement()
    {
        return valuesByMovement;
    }

    public void AddMovement (Movement move, float value)
    {
        movements.Add(move);
        valuesByMovement.Add(value);
    }

    public ArrayList GetMovements()
    {
        return movements;
    }

    public string GetBoardConfiguration()
    {
        return boardConfiguration;
    }

    public override string ToString()
    {
        string result = "ConfigurationBoard: ";
        result += boardConfiguration;
        for(int i = 0; i < movements.Count; ++i)
        {
            result += "  Movement: " + movements[i].ToString();
            result += " Value: " + valuesByMovement[i] + "\n";
        }
        return result;
    }
}
