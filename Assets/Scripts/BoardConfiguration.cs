using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardConfiguration {
    readonly string boardConfiguration;
    private List<MovementConfiguration> movementConfigurations;

    public BoardConfiguration(string boardConfiguration)
    {
        this.boardConfiguration = boardConfiguration;
        movementConfigurations = new List<MovementConfiguration>();
    }

    public bool HasMovementConfiguration(MovementConfiguration moveConfiguration)
    {
        foreach(MovementConfiguration mc in movementConfigurations)
        {
            if (mc.Equals(moveConfiguration))
            {
                return true;
            }
        }
        return false;
    }

    public MovementConfiguration GetMovementConfigurationWithMove(Movement move)
    {
        foreach (MovementConfiguration mc in movementConfigurations)
        {
            if (mc.GetMove().Equals(move))
            {
                return mc;
            }
        }
        return null;
    }

    public void AddMovement (Movement move, float value)
    {
        if (move == null)
            Debug.LogError("Error trying to add a Movement");
        movementConfigurations.Add(new MovementConfiguration(move, value));
    }

    public List<MovementConfiguration> GetMovementsConfigurations()
    {
        return movementConfigurations;
    }

    public string GetBoardConfiguration()
    {
        return boardConfiguration;
    }

    public void SetLastValue(float value)
    {
        movementConfigurations[movementConfigurations.Count - 1].SetAdaptation(value);
    }

    public override string ToString()
    {
        string result = "ConfigurationBoard: ";
        result += boardConfiguration + "\n";

        foreach( MovementConfiguration mc in movementConfigurations)
        {
            result += "  Movement " + mc.GetMove().ToString();
            result += " Value: " + mc.GetAdaptation();
            result += " N of Results: " + mc.GetResults().Count + "\n";
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
