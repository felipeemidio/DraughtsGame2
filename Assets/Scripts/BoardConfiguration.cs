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
        if (move == null)
            Debug.LogError("Error trying to add a Movement");
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

    public void SetLastValue(float value)
    {
        valuesByMovement[valuesByMovement.Count - 1] = value; 
    }

    public override string ToString()
    {
        string result = "ConfigurationBoard: ";
        result += boardConfiguration + "\n";
        for(int i = 0; i < movements.Count; ++i)
        {
            result += "  Movement: " + movements[i].ToString();
            result += " Value: " + valuesByMovement[i] + "\n";
        }
        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        BoardConfiguration other = (BoardConfiguration)obj;
        return other.GetBoardConfiguration().Equals(this.boardConfiguration);
    }

    public override int GetHashCode()
    {
        return -343895701 + EqualityComparer<string>.Default.GetHashCode(boardConfiguration);
    }
}
