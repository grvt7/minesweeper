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
        // Populates the cell array with empty tiles
        GenerateCells();
        // Offsets the camera to position around the board
        Camera.main.transform.position = new Vector3(width/2f, height/2f, -10f);
        GenerateMines();
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
}
