using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTree {

    Node root;
    ArrayList bestSolution;
    int bestDepth;

    /**
     * Constructor
     */
    public CaptureTree(Node root)
    {
        this.root = root;
    }

    /**
     * Return the way to the deepest node.
     */
    public ArrayList GetBestMoves()
    {
        bestSolution = new ArrayList();
        bestDepth = 0;

        ArrayList result = new ArrayList();
        if(root != null)
        {
            RecursiveSearch(this.root, result, 1);
        }
        return bestSolution;
    }

    /**
     * Recursive method for search the deepest node.
     */
    public void RecursiveSearch(Node currentNode, ArrayList bestWay, int depth)
    {
        bestWay.Add(currentNode);

        if(depth > this.bestDepth)
        {
            this.bestDepth = depth;
            this.bestSolution = bestWay;
        }

        if (currentNode.GetChilds().Count != 0)
        {
            foreach (Node child in currentNode.GetChilds())
            {
                RecursiveSearch(child, bestWay, depth + 1);
            }
        }
 
    }
}
