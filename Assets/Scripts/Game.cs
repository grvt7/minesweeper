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

    // Storing the game state;
    private bool gameOver;

// Validate Number of Mines that can Exist on the Board
private void OnValidate() {
    // Restricts the number of mines that can exist to width * height of grid
    mineCount = Mathf.Clamp(mineCount, 0, width * height);
}

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
        gameOver = false;
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
        // Ig game is not over do this
        if(!gameOver) {
            // Check for User Input Right Mouse Click
            if(Input.GetMouseButtonDown(1)) {
                Flag();
            }
            // Checks for User Input Left Mouse Click
            else if(Input.GetMouseButtonDown(0)) {
                Reveal();
            }
        }else{
            if(Input.GetMouseButtonDown(0)){
                NewGame();
            }
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
        // Check if the cell is Invalid or Revealed
        if(cell.type == Cell.Type.InValid || cell.revealed) {
            return;
        }
        // Perform not Operation on Flagged cell
        cell.flagged = !cell.flagged;
        // Update the cell state at that position from the cell settings
        state[cellPosition.x, cellPosition.y] = cell;
        // Redraws the board
        board.Draw(state);  
    }

    private void Reveal(){
        // To flag a tile we need to get which tile we clicked on.
        // The mouse in Screen Space. We need to get position from screen space to world space to cell space.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Converting the world position to cell position
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition); 
        // Check if the cell position is valid
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if(cell.type == Cell.Type.InValid || cell.revealed || cell.flagged) {
            return;
        }

        switch(cell.type) {
            case Cell.Type.Mine:
                // Call Explode on the cell that is clickced
                Explode(cell);
                break;
            case Cell.Type.Empty:
                // Flooding the Board if Empty Cell is Clicked
                Flood(cell);
                break;
            default:
                // Turn the Cell to Reveal
                cell.revealed = true;
                // Update the cell state at that position from the cell settings
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }
        // Redraws the board
        board.Draw(state);
    }

// Flooding uses reccursion
    private void Flood(Cell cell) {
        if(cell.revealed) return;
        if(cell.type == Cell.Type.Mine || cell.type == Cell.Type.InValid) {
            return;
        }
        // Turn the Cell to Reveal
        cell.revealed = true;
        // Update the cell state at that position from the cell settings
        state[cell.position.x, cell.position.y] = cell;
        // Reccursively call until mines are hit
        if(cell.type == Cell.Type.Empty) {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

// Logic to Explode when mines are clicked
    private void Explode(Cell cell) {
        // Sets gameOver to true
        gameOver = true;
        // Revel the cell which made the game over
        cell.revealed = true;
        // Set that cell to exploded
        cell.exploded = true;
        // Save it to the matrix
        state[cell.position.x, cell.position.y] = cell;

        // Loops over the grid and reveals all the other mines in case of explosion
        for(int x=0; x < width; x++){
            for(int y=0; y < height; y++){
                cell = state[x,y];
                if(cell.type == Cell.Type.Mine){
                    cell.revealed = true;
                    state[x,y] = cell;
                }
            }
        }
    }

// Check for win conditions
private void CheckWinCondition() {
    for(int x=0; x < width; x++) {
        for(int y=0; y < height; y++) {
            Cell cell = state[x,y];

            if(cell.type != Cell.Type.Mine && !cell.revealed) {
                return;
            }
        }
    }   
    Debug.Log("Winner!");
    gameOver = true;
    // Loops over the grid and flags all the other mines in case of winning
    for(int x=0; x < width; x++){
        for(int y=0; y < height; y++){
            Cell cell = state[x,y];
            
            if(cell.type == Cell.Type.Mine){
                cell.flagged = true;
                state[x,y] = cell;
            }
        }
    }
}

// Gets the cell from position
    private Cell GetCell(int x, int y) {
        // Check if the cell is valid or not i.e. if the cell is on the grid or not
        if(IsValid(x,y)){
            // Return the state/ cell at the index
            return state[x,y];
        }
        else{
            // Returns a new cell that is invalid by default
            return new Cell();
        }
    }

// Check if the cell is valid or not
    private bool IsValid(int x, int y){
        // Return true if all the conditions are met else returns false.
        // True = Valid Cell i.e. inside grid, False = Invalid Cell
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
