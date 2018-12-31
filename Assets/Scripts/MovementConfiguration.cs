using System;
using System.Collections.Generic;

[Serializable]
public class MovementConfiguration
{
    private readonly Movement move;
    private float adaptation;
    private List<string> results;  // possible board configuration due the selected movement.

    public MovementConfiguration(Movement move, float adaptation)
    {
        this.move = move;
        this.adaptation = adaptation;
        results = new List<string>();
    }

    public Movement GetMove()
    {
        return move;
    }

    public float GetAdaptation()
    {
        return adaptation;
    }

    public void SetAdaptation(float adaptation)
    {
        this.adaptation = adaptation;
    }

    public List<string> GetResults()
    {
        return results;
    }

    public void AddResults(string result)
    {
        if(!results.Contains(result))
            results.Add(result);
    }

    public bool HasResult(string result)
    {
        return results.Contains(result);
    }

    public override bool Equals(object obj)
    {
        var configuration = obj as MovementConfiguration;
        return configuration != null &&
               EqualityComparer<Movement>.Default.Equals(move, configuration.move);
    }

    public override int GetHashCode()
    {
        return -1246910668 + EqualityComparer<Movement>.Default.GetHashCode(move);
    }
}
