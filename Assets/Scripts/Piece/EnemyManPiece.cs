
public class EnemyManPiece : ManPiece {

    public override void Start ()
    {
        base.forward = -1;
        base.enemy_tag = "BluePiece";
        base.kingVersionPath = "Prefabs/KingGrayPiece";
        base.Start();
    }
}
