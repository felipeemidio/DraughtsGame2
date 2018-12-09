using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    /**
     * Public Variables.
     */
    public Color selectionColor;
    public Color possibiliteColor;
    public Color blackTileColor;
    public List<GameObject> allTiles;
    // These are required to make tha animation.
    public float speed = 5f;
    public float timeTakenDuringLerp = 0.1f;

    /**
     * Private Variables.
     */
    private bool tileClickedCalled = false;
    private GameController gameController;
    private Piece pieceToMove = null;
    private ArrayList allPlayerPieces;
    private ArrayList allEnemyPieces;
    private ArrayList paintedPositions = null;
    // These are required to make tha animation.
    private float timeStartedLerping;
    private Vector3 originPosition;
    private Vector3 destinyPosition;
    // Use this to put to the piece overlay other sprites.
    private GameObject overlay;
    private TileHandler targetTile;

    // Possible states of the board.
    private enum State
    {
        waitingMovement,
        playerSelectAPiece,
        enemyTime,
        doingMovement,
        haveWinner
    }
    private State state;

    void Awake()
    {
        // Set variables.
        state = State.waitingMovement;
        //currentPiece = null;
        gameController = 
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        overlay = GameObject.FindGameObjectWithTag("OverLay");
        if(overlay == null)
        {
            Debug.LogError("Can't found overlay object");
        }
    }

    void Start()
    {
        this.RefreshAllPieces();
    }

    void Update()
    {

        // Animation of the piece moving
        if (state == State.doingMovement)
        {
            float timeSinceStarted = (Time.time - timeStartedLerping) * speed;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            pieceToMove.transform.position = Vector3.Lerp(originPosition,
                destinyPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                // Set the new position of the piece
                pieceToMove.transform.SetParent(targetTile.transform, true);
                pieceToMove.SetCurrentPosition();

                state = State.waitingMovement;

                // Send a message to player saying that the piece finish to move.
                gameController.NotifyPlayerEndOfMovement();
            }
        }
        // Get mouse clicks
        else
        {
            // Get right mouse's button click.
            if (Input.GetMouseButtonUp(0) && state != State.doingMovement
                && gameController.IsPlayerTurn())
            {
                // Deselect when clicked out of the table.
                if (tileClickedCalled)
                {
                    tileClickedCalled = false;
                }
                else
                {
                    DeselectTiles();
                }
            }   
        }
    }

    /**
     * If a tiled is clicked this function is called. 
     */
    public void TileClicked(TileHandler tile)
    {
        // Set that this event was called.
        this.tileClickedCalled = true;
        // Remove selections.
        this.DeselectTiles();

        // Do nothing if a piece is already moving.
        if (state == State.doingMovement || !this.gameController.IsPlayerTurn())
        {
            return;
        }
        
        // Send the tile to the player decide whats he going to do.
        this.gameController.SendToPlayer(tile);
        
    }


    public void SelectPiece(Piece selectedPiece, ArrayList possibleMovements)
    {
        paintedPositions = new ArrayList();
        paintedPositions.Add(selectedPiece.GetPosition());
        // Change selected tile's color.
        TileHandler tile = GetTile(selectedPiece.GetPosition());
        tile.GetComponent<Image>().color = selectionColor;
        // Change the color of the avaliable tiles.
        foreach (Movement movement in possibleMovements)
        {
            paintedPositions.Add(movement.getDestinyPosition());
            GetTile(movement.getDestinyPosition()).GetComponent<Image>().color = possibiliteColor;
        }
    }

    public void DeselectTiles()
    {
        // Deselect a tile if already selected;
        if (paintedPositions != null)
        {
            foreach (IntVector2 tilePosition in paintedPositions)
            {
                this.GetTile(tilePosition).GetComponent<Image>().color = blackTileColor;
            }
            paintedPositions = null;
        }
    }

    public void MovePiece( Movement move )
    {
        pieceToMove = this.GetTile (move.getOriginalPosition())
            .transform.GetChild(0).GetComponent<Piece>();

        // Get the piece that will be captured if exists.
        Piece pieceToBeCaptured = move.getCapturedPiece();

        // It is a capture movement?
        if (pieceToBeCaptured != null)
        {
            pieceToBeCaptured.Capture();
        }

        /*
         * Change the piece's parent to the 'overlay' object
         * because we want the piece above others sprites.
         */
        pieceToMove.transform.SetParent(overlay.transform, true);

        // Get this tile to reference it as a parent of the piece later.
        targetTile = this.GetTile (move.getDestinyPosition());

        //These are the parameters necessary to lerp the piece until the destiny.
        timeStartedLerping = Time.time;
        originPosition = pieceToMove.transform.position;
        destinyPosition = targetTile.transform.position;

        state = State.doingMovement;
    }

    /**
     * Return tiles that the player could move to the original color.
     */
    public void ResetPossibleMovements(ArrayList possibleMoves)
    {
        if (possibleMoves != null)
        {
            foreach (Movement e in possibleMoves)
            {
                GetTile(e.getDestinyPosition())
                    .GetComponent<Image>().color = new Color(0.3113f, 0.3113f, 0.3113f);
            }
        }
    }

    /**
     * Verify if some piece can capture a enemy's piece.
     */
    public bool SomePieceCanCapture()
    {
        ArrayList captureMovements;
        foreach (Piece piece in allPlayerPieces)
        {
            captureMovements = piece.GetCaptureMovements();
            if (captureMovements.Count != 0)
            {
                return true;
            }
        }
        return false;
    }

    /**
     * Destroy the pieces that are marked with "hasBeenCaptured" variable.
     */
    public void DestroyCapturedPieces()
    {
        // Destroy captured enemy's pieces.
        ArrayList destroyedPieces = new ArrayList();
        foreach (Piece piece in allEnemyPieces)
        {
            if (piece.HasBeenCaptured())
            {
                destroyedPieces.Add( piece);
            }
        }
        foreach (Piece pieceToBeDestroyed in destroyedPieces)
        {
            allEnemyPieces.Remove(pieceToBeDestroyed);
            Destroy(pieceToBeDestroyed.gameObject);
        }

        // Destroy captured player's pieces.
        destroyedPieces = new ArrayList();
        foreach (Piece piece in allPlayerPieces)
        {
            if (piece.HasBeenCaptured())
            {
                destroyedPieces.Add(piece);
            }
        }
        foreach (Piece pieceToBeDestroyed in destroyedPieces)
        {
            allEnemyPieces.Remove(pieceToBeDestroyed);
            Destroy(pieceToBeDestroyed.gameObject);
        }
    }

    /// <summary>
    /// Refresh the active pieces in the board.
    /// </summary>
    public void RefreshAllPieces()
    {
        // Get all player pieces.
        allPlayerPieces = new ArrayList();
        GameObject[] allPiecesObjects = GameObject.FindGameObjectsWithTag("BluePiece");
        foreach (GameObject pieceObject in allPiecesObjects)
        {
            if (pieceObject.GetComponent<Piece>().HasBeenCaptured())
                continue;

            if (pieceObject.GetComponent<PlayerManPiece>() != null)
            {
                allPlayerPieces.Add(pieceObject.GetComponent<PlayerManPiece>());
            }
            else
            {
                allPlayerPieces.Add(pieceObject.GetComponent<PlayerKingPiece>());
            }
        }

        // Get all player pieces.
        allEnemyPieces = new ArrayList();
        allPiecesObjects = GameObject.FindGameObjectsWithTag("WhitePiece");
        foreach (GameObject pieceObject in allPiecesObjects)
        {
            if (pieceObject.GetComponent<Piece>().HasBeenCaptured())
                continue;

            if (pieceObject.GetComponent<EnemyManPiece>() != null)
            {
                allEnemyPieces.Add(pieceObject.GetComponent<EnemyManPiece>());
            }
            else
            {
                allEnemyPieces.Add(pieceObject.GetComponent<EnemyKingPiece>());
            }
        }
    }

    private string PrintMovements(ArrayList list)
    {
        string result = "";
        foreach (Movement pos in list)
        {
            result += pos.ToString() + " - ";
        }
        result += "final.";
        return result;
    }

    private void PrintTree(ArrayList matrix)
    {
        string message = "";
        foreach (ArrayList list in matrix)
        {
            foreach (Movement move in list)
            {
                message += "From " + move.getOriginalPosition().ToString();
                message += " To " + move.getDestinyPosition().ToString() + " - ";  
            }
            message += "final\n";
        }

        Debug.Log(message);
    }
    /// <summary>
    /// Return a Movement if it exists within the list.
    /// </summary>
    private Movement GetMovementIfExists(ArrayList list, IntVector2 originalPos)
    {
        if (list == null)
        {
            return null;
        }
        foreach (Movement move in list)
        {
            if(move.getDestinyPosition().x == originalPos.x && move.getDestinyPosition().y == originalPos.y)
            {
                return move;
            }
        }
        return null;
    }

    /// <summary>
    /// Return the number of blue king pieces on the board.
    /// </summary>
    public int NumberOfPlayerKingPieces()
    {
        return this.CountKingPieces(this.allPlayerPieces);
    }

    /// <summary>
    /// Return the number of blue man pieces on the board.
    /// </summary>
    public int NumberOfPlayerManPieces()
    {
        return this.allPlayerPieces.Count - this.CountKingPieces(this.allPlayerPieces);
    }

    /// <summary>
    /// Return the number of white king pieces on the board.
    /// </summary>
    public int NumberOfEnemyKingPieces()
    {
        return this.CountKingPieces(this.allEnemyPieces);
    }

    /// <summary>
    /// Return the number of white king pieces on the board.
    /// </summary>
    public int NumberOfEnemyManPieces()
    {
        return this.allEnemyPieces.Count - this.CountKingPieces(this.allEnemyPieces);
    }

    private int CountKingPieces(ArrayList list)
    {
        int counter = 0;
        foreach (Piece p in list)
        {
            if (p.GetComponent<KingPiece>() != null)
                counter++;
        }
        return counter;
    }

    /// <summary>
    /// Return the white pieces of the board.
    /// </summary>
    public ArrayList GetEnemyPieces()
    {
        return this.allEnemyPieces;
    }

    /// <summary>
    /// Return the blue pieces of the board.
    /// </summary>
    public ArrayList GetPlayerPieces()
    {
        return this.allPlayerPieces;
    }

    /// <summary>
    /// Return a Tile of the board.
    /// </summary>
    public TileHandler GetTile(int row, int collumn)
    {

        if (WithinBounds(row, collumn))
        {
            int pos = (row-1) * 8 + (collumn - 1) ;
            return this.allTiles[pos].GetComponent<TileHandler>();
        }
        return null;       
    }

    /// <summary>
    /// Return a Tile of the board.
    /// </summary>
    public TileHandler GetTile(IntVector2 pos)
    {
        return this.GetTile(pos.x, pos.y);
    }

    /// <summary>
    /// Verify if the row and column contains in the board.
    /// </summary>
    public bool WithinBounds(int r, int c)
    {
        if (r > 0 && r <= 8 && c > 0 && c <= 8)
            return true;
        return false;
    }
}
