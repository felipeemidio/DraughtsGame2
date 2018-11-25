using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManPiece : ManPiece {
    
    public override void Start ()
    {
        base.forward = 1;
        base.enemy_tag = "WhitePiece";
        base.kingVersionPath = "Prefabs/KingBluePiece";
        base.Start();
	}
}
