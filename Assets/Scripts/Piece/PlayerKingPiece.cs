using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKingPiece : KingPiece
{
    public override void Start()
    {
        base.Start();
        base.enemy_tag = "WhitePiece";
    }
}
