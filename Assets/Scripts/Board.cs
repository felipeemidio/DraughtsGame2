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
    private GameObject lastTileClicked;
    private Color colorLastTileClicked;

    private float timeStartedLerping;
    private Vector3 originPosition;
    private Vector3 destinyPosition;

    private PlayerManPiece currentPiece;
    // Use this to put to the piece overlay other sprites.
    private GameObject overlay;
    private GameObject targetTile;
    private GameController gameController;
    private ArrayList canMove = null;
    private ArrayList allPlayerPieces;
    private ArrayList allEnemyPieces;
    private bool someCanCapture = false;

    private PlayerManPiece pieceWithinSucessiveCapture = null;


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
        // Get all player pieces.
        allPlayerPieces = new ArrayList();
        GameObject[] allPiecesObjects = GameObject.FindGameObjectsWithTag("BluePiece");
        foreach (GameObject pieceObject in allPiecesObjects)
        {
            allPlayerPieces.Add(pieceObject.GetComponent<PlayerManPiece>());
        }

        // Get all player pieces.
        allEnemyPieces = new ArrayList();
        allPiecesObjects = GameObject.FindGameObjectsWithTag("WhitePiece");
        foreach (GameObject pieceObject in allPiecesObjects)
        {
            allEnemyPieces.Add(pieceObject.GetComponent<EnemyManPiece>());
        }
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

                // See if its possible a sucessiveMovement.
                if(currentPiece.GetCaptureMovements().Count > 0)
                {
                    Debug.Log("sucessiveCapture");
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
    public void TileClicked(GameObject tile, int row, int collumn)
    {
        ResetPossibleMovements();
        // Do nothing if a piece is already moving.
        if (state == State.doingMovement || !gameController.isPlayerTurn())
        {
            return;
        }

        //Internal Variables.
        bool pieceMoved = false;
        // Set that this event was called.
        someTileClicked = true;

        // Select a piece and change the color of the possible tiles of a chosen piece.
        if (tile.transform.childCount > 0 && tile.transform.GetChild(0).CompareTag("BluePiece"))
        {

            // Set the chosen piece as the current one.
            currentPiece = tile.transform.GetChild(0).gameObject.GetComponent<PlayerManPiece>();
            if( (pieceWithinSucessiveCapture != null && pieceWithinSucessiveCapture == currentPiece)
                || pieceWithinSucessiveCapture == null)
            {
                /** TODO: Set the best options as mandatory*/
                //printTree(currentPiece.GetBestSucessiveCapture());
                // Get Possible moves that piece can make.
                //canMove = currentPiece.GetCaptureMovements();
                canMove = currentPiece.GetBestSucessiveCapture();
                if (canMove.Count == 0 && !someCanCapture)
                {
                    canMove = currentPiece.GetWalkMovements();
                }
                // Change the color of the avaliable tiles.
                foreach (IntVector2 e in canMove)
                {
                    GetTile((int)e.x, (int)e.y).GetComponent<Image>().color = possibiliteColor;
                }
            }
            
        }
        // See if is to move a piece.
        else if (currentPiece != null && tile.transform.childCount == 0)
        {
            pieceMoved = true;
            

            TileHandler tileScript = tile.GetComponent<TileHandler>();
            IntVector2 tilePosition = new IntVector2(tileScript.getRow(), tileScript.getColumn() );
            Debug.Log("tile position: " + tilePosition.ToString() + "\n canMove: " + PrintMovements(canMove));
            Debug.Log(this.Contain(canMove, tilePosition));
            if (canMove != null && this.Contain(canMove, tilePosition))
            {

                Debug.Log("Entrou");
                /*
                 * Change the piece's parent to the 'overlay' object
                 * because we want the piece above others sprites.
                 */
                TileHandler currentTile = currentPiece.transform.parent.GetComponent<TileHandler>();
                IntVector2 currentPosition = new IntVector2(currentTile.getRow(), currentTile.getColumn());

                // It is a capture movement?
                if ( Mathf.Abs(tilePosition.x - currentPosition.x) >= 2)
                {
                    int rowOffset = (int) tilePosition.x - (int)currentPosition.x ;
                    int columnOffset = (int)tilePosition.y - (int)currentPosition.y;


                    Debug.Log("row : " + rowOffset + " column : " + columnOffset);
                    if (Mathf.Abs(rowOffset) >= 2 && Mathf.Abs(columnOffset) >= 2 )
                    {
                        // Get the tile of the enemy Piece
                        rowOffset = rowOffset / Mathf.Abs(rowOffset);
                        columnOffset = columnOffset / Mathf.Abs(columnOffset);

                        TileHandler enemyTile =
                            GetTile((int)currentPosition.x + rowOffset, (int)currentPosition.y + columnOffset)
                            .GetComponent<TileHandler>();
                        // Get the piece and mark as captured.
                        if(enemyTile.transform.childCount > 0)
                        {
                            Piece enemyPiece = enemyTile.transform.GetChild(0).GetComponent<Piece>();
                            enemyPiece.Capture();
                        }
                    }
                }

                currentPiece.transform.SetParent(overlay.transform, true);
                // Get this tile to reference it as a parent of the piece later.
                targetTile = tile;
                
                state = State.doingMovement;
                //These are the parameters necessary to lerp the piece until the destiny.
                timeStartedLerping = Time.time;
                originPosition = currentPiece.transform.position;
                destinyPosition = tile.transform.position;
            }

            //ResetPossibleMovements();
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
            foreach (IntVector2 e in canMove)
            {
                GetTile((int)e.x, (int)e.y).GetComponent<Image>().color = new Color(0.3113f, 0.3113f, 0.3113f);
            }
        }
    }

    /**
     * Verify if some piece can capture a enemy's piece.
     */
    public bool SomePieceCanCapture()
    {
        ArrayList captureMovements;
        foreach (PlayerManPiece piece in allPlayerPieces)
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
        foreach (PlayerManPiece piece in allPlayerPieces)
        {
            if (piece.HasBeenCaptured())
            {
                destroyedPieces.Add(piece);
            }
        }
        foreach (PlayerManPiece pieceToBeDestroyed in destroyedPieces)
        {
            allEnemyPieces.Remove(pieceToBeDestroyed);
            Destroy(pieceToBeDestroyed.gameObject);
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

    private void printTree(ArrayList matrix)
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

    private bool Contain(ArrayList list, IntVector2 originalPos)
    {
        foreach (IntVector2 pos in list)
        {
            if(pos.x == originalPos.x && pos.y == originalPos.y)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Return a Tile of the board.
    /// </summary>
    public GameObject GetTile(int row, int collumn)
    {

        if (WithinBounds(row, collumn))
        {
            int pos = (row-1) * 8 + (collumn - 1) ;
            return this.allTiles[pos];
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
