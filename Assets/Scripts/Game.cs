using UnityEngine;

public class Game : MonoBehaviour {
    // Refrences the height and the width of the board. Public, can be changed from Unity
    public int width = 16, height = 16;
    // Refrences number of mines to generate
    public int mineCount = 32;
    // Getting a refrence to the Board Script
    private Board board;

    // Creating a cell matrix to store the cell matrix
    private Cell[,] state;

// Called first when the game runs
    private void Awake() {
        // Getting the board component from the child component inside the grid.
        board = GetComponentInChildren<Board>();
    }

// Called only once after Awake on the first frame
    private void Start() {
        NewGame();
    }

// Initializes the board when the new Game Begins
    private void NewGame() {
        // Creates the Cell Array
        state = new Cell[width, height];
        // Populates the cell array with Empty Tiles
        GenerateCells();
        // Populates the grid with Mines
        GenerateMines();
        // Generates the numbers for showing how many mines are around the cell
        GenerateNumbers();
        // Offsets the camera to position around the board
        Camera.main.transform.position = new Vector3(width/2f, height/2f, -10f);
        // Draws the cell array
        board.Draw(state);
    }

// Generates the Gird with the Height and Width and populates it with Empty Cells
    private void GenerateCells() {
        // Loops the Height and Width for each individual Cell
        for(int x = 0; x < width; x++) {
            for(int y = 0; y< height; y++) {
                Cell cell = new Cell();// Creates a new Cell from structure
                cell.position = new Vector3Int(x, y, 0);// Gives it a position in the grid
                cell.type = Cell.Type.Empty;// Gives it a type
                state[x, y] = cell;// Adding it to Cells array at index x, y
            }
        }
    }

// Generated mines for the grid
    private void GenerateMines() {
        // Loops over the number of mines to be created
        for(int i=0; i<mineCount; i++) {
            // Generate a random x and y coordinate.
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            
            if(state[x,y].type == Cell.Type.Mine){
                i -= 1;
            }
            // Set the type of cell at cell[x,y] as mine
            state[x,y].type = Cell.Type.Mine;
        }
    }

// Generates the numbers for mines around it
    private void GenerateNumbers() {
        // Iterate over the whole grid one by one
        for(int x = 0; x < width; x++) {
            for(int y = 0; y< height; y++) {
                // Get the cell state i.e. cell data individually
                Cell cell = state[x,y];
                // Check if the current cell is mine then skip it
                if(cell.type == Cell.Type.Mine){
                    continue;
                }
                // Get the number of mines surrounding the cell
                cell.number = CountMines(x,y);

                // Checks if there are numbers surrounding the cell
                if(cell.number > 0) {
                    cell.type = Cell.Type.Number;
                }
                // Set the state of the cell since we made changes to the cell
                state[x,y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY){
        // Count to keep track of mines surrounding the tile/cell
        // X,Y to track the coordinate we are looking at
        int count = 0, x = 0, y = 0;
        // Iterate over the 3 adjacent Rows and Columns
        for(int adjacentX = -1; adjacentX <= 1; adjacentX++){
            for(int adjacentY = -1; adjacentY <= 1; adjacentY++){
                // Current position for which this function is called
                if(adjacentX == 0 && adjacentY == 0){
                    continue;
                }
                // Gets the new positon
                x = cellX + adjacentX;
                y = cellY + adjacentY;

                // Check if Coordinate is out of Bounds
                if(x < 0 || x >= width || y < 0 || y >= height){
                    continue;
                }
                
                // Checks for Mines in this new position
                if(GetCell(x, y).type == Cell.Type.Mine) {
                    count++;
                }
            }
        }
        // Return the Number of Mines Around the cell
        return count;
    }

    // Update Function to handle user inputs
    private void Update() {
        if(Input.GetMouseButtonDown(1)) {
            Flag();
        }
    }

// Logic to handle flagging a cell
    private void Flag() {
        // To flag a tile we need to get which tile we clicked on.
        // The mouse in Screen Space. We need to get position from screen space to world space to cell space.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Converting the world position to cell position
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition); 
        // Check if the cell position is valid
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if(cell.type == Cell.Type.InValid || cell.revealed) {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }

// Gets the cell from position
    private Cell GetCell(int x, int y) {
        if(IsValid(x,y)){
            return state[x,y];
        }
        else{
            return new Cell();
        }
    }

    private bool IsValid(int x, int y){
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
