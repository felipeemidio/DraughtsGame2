using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManPiece : ManPiece {

    public override void Start()
    {
        base.Start();
        base.forward = -1;
        base.enemy_tag = "BluePiece";
    }
}
