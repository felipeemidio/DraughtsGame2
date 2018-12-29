using System;

[Serializable]
public class PlayerKingPiece : KingPiece {

    public override void Start()
    {
        base.Start();
        base.enemy_tag = "WhitePiece";
    }
}
