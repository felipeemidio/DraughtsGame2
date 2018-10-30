using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    private IntVector2 value;
    private ArrayList childs;

    public Node(IntVector2 value)
    {
        this.value = value;
        this.childs = new ArrayList();
    }

    public IntVector2 GetValue()
    {
        return this.value;
    }

    public ArrayList GetChilds()
    {
        return this.childs;
    }

    public int NumberOfChilds()
    {
        return this.childs.Count;
    }

    public void SetChilds(ArrayList list)
    {
        this.childs = list;
    }

}
