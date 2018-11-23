using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    // Possible states of the board.
    public enum State {
        waitingMovement,
        playerSelectAPiece,
        enemyTime,
        doingMovement,
        haveWinner
    }

    private State state;

    // Public Variables.
    public Color selectionColor;
    public Color possibiliteColor;
    public List<GameObject> allTiles;
    public float speed = 5f;

    public float timeTakenDuringLerp = 0.1f;

    // Private Variables.
    private bool someTileClicked;
    private TileHandler lastTileClicked;
    private Color colorLastTileClicked;

    private float timeStartedLerping;
    private Vector3 originPosition;
    private Vector3 destinyPosition;

    private Piece currentPiece;
    // Use this to put to the piece overlay other sprites.
    private GameObject overlay;
    private TileHandler targetTile;
    private GameController gameController;
    private ArrayList canMove = null;
    private ArrayList allPlayerPieces;
    private ArrayList allEnemyPieces;
    private bool someCanCapture = false;

    private Piece pieceWithinSucessiveCapture = null;


    void Awake()
    {
        // Set variables.
        state = State.waitingMovement;
        someTileClicked = false;
        lastTileClicked = null;
        currentPiece = null;
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
            currentPiece.transform.position = Vector3.Lerp(originPosition,
                destinyPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                // Set the new position of the piece
                currentPiece.transform.SetParent(targetTile.transform, true);
                currentPiece.SetCurrentPosition();

                // Try to promote
                ManPiece currentManPiece = currentPiece.GetComponent<ManPiece>();
                if (currentManPiece != null)
                    currentManPiece.Promote();

                // See if its possible a sucessiveMovement.
                if(currentPiece.GetCaptureMovements().Count > 0)
                {
                    //Debug.Log("sucessiveCapture");
                    pieceWithinSucessiveCapture = currentPiece;
                }
                else
                {
                    pieceWithinSucessiveCapture = null;
                    gameController.NextTurn();
                }
                
                currentPiece = null;
                state = State.waitingMovement;
            }
        }
        else
        {
            bool isPlayerTurn = gameController.isPlayerTurn();
            // Get right mouse's button click.
            if (Input.GetMouseButtonUp(0) && state != State.doingMovement
                && isPlayerTurn)
            {
                // Deselect when clicked out of the table.
                if (someTileClicked)
                {
                    someTileClicked = false;
                }
                else
                {
                    DeselectTile();
                    ResetPossibleMovements();
                }
            }   
        }
    }

    /**
     * If a tiled is clicked this function is called. 
     */
    public void TileClicked(TileHandler tile, int row, int collumn)
    {
        ResetPossibleMovements();
        
        // Do nothing if a piece is already moving.
        if (state == State.doingMovement || !gameController.isPlayerTurn())
        {
            return;
        }

        // Will be true if it's moving a piece.
        bool pieceMoved = false;
        // Set that this event was called.
        someTileClicked = true;

        // Select a piece and change the color of the possible tiles of a chosen piece.
        if (tile.transform.childCount > 0 && tile.transform.GetChild(0).CompareTag("BluePiece"))
        {

            // Set the chosen piece as the current one.
            currentPiece = tile.transform.GetChild(0).gameObject.GetComponent<PlayerManPiece>();
            if(currentPiece == null)
            {
                currentPiece = tile.transform.GetChild(0).gameObject.GetComponent<PlayerKingPiece>();
            }

            if( (pieceWithinSucessiveCapture != null && pieceWithinSucessiveCapture == currentPiece)
                || pieceWithinSucessiveCapture == null)
            {
                //printTree(currentPiece.GetBestSucessiveCapture());

                // Get Possible moves that piece can make.
                canMove = currentPiece.GetBestSucessiveCapture();
                if (canMove.Count == 0 && !someCanCapture)
                {
                    canMove = currentPiece.GetWalkMovements();
                }
                // Change the color of the avaliable tiles.
                foreach (Movement e in canMove)
                {
                    GetTile(e.getDestinyPosition().x, e.getDestinyPosition().y).GetComponent<Image>().color = possibiliteColor;
                }
            }
            
        }
        // See if is to move a piece.
        else if (currentPiece != null && tile.transform.childCount == 0)
        {
            pieceMoved = true;
            
            Movement choseMovement = this.GetMovementIfExists(canMove, tile.getPosition());
            if (canMove != null && choseMovement != null)
            {
                // Get the piece that will be captured if exists.
                Piece pieceToBeCaptured = choseMovement.getCapturedPiece();

                // It is a capture movement?
                if ( pieceToBeCaptured != null)
                {
                    pieceToBeCaptured.Capture();
                }

                /*
                 * Change the piece's parent to the 'overlay' object
                 * because we want the piece above others sprites.
                 */
                currentPiece.transform.SetParent(overlay.transform, true);
                
                // Get this tile to reference it as a parent of the piece later.
                targetTile = tile;
                
                state = State.doingMovement;
                
                //These are the parameters necessary to lerp the piece until the destiny.
                timeStartedLerping = Time.time;
                originPosition = currentPiece.transform.position;
                destinyPosition = tile.transform.position;
            }
        }

        DeselectTile();
        
        if (!pieceMoved)
        {
            // Set the new last tile clicked
            lastTileClicked = tile;
            colorLastTileClicked = tile.GetComponent<Image>().color;
            // Change selected tile's color.
            tile.GetComponent<Image>().color = selectionColor;
        }
    }

    private void DeselectTile()
    {
        // Deselect a tile if already selected;
        if (lastTileClicked != null)
        {
            lastTileClicked.GetComponent<Image>().color = colorLastTileClicked;
            lastTileClicked = null;
        }
    }

    /**
     * Return tiles that the player could move to the original color.
     */
    private void ResetPossibleMovements()
    {
        if (canMove != null)
        {
            foreach (Movement e in canMove)
            {
                GetTile(e.getDestinyPosition().x, e.getDestinyPosition().y).GetComponent<Image>().color = 
                    new Color(0.3113f, 0.3113f, 0.3113f);
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
                someCanCapture = true;
                return true;
            }
        }
        someCanCapture = false;
        return false;
    }

    /**
     * Destroy the pieces that are marked with "hasBeenCaptured" variable.
     */
    public void DestroyCapturedPieces()
    {
        this.RefreshAllPieces();
        ArrayList destroyedPieces = new ArrayList();
        foreach (EnemyManPiece piece in allEnemyPieces)
        {
            if (piece.HasBeenCaptured())
            {
                destroyedPieces.Add( piece);
            }
        }
        foreach (EnemyManPiece pieceToBeDestroyed in destroyedPieces)
        {
            allEnemyPieces.Remove(pieceToBeDestroyed);
            Destroy(pieceToBeDestroyed.gameObject);
        }

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
            allEnemyPieces.Add(pieceObject.GetComponent<EnemyManPiece>());
        }
    }

    private string PrintMovements(ArrayList list)
    {
        string result = "";
        foreach (IntVector2 pos in list)
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
    /// Verify if the row and column contains in the board.
    /// </summary>
    public bool WithinBounds(int r, int c)
    {
        if (r > 0 && r <= 8 && c > 0 && c <= 8)
            return true;
        return false;
    }
}
