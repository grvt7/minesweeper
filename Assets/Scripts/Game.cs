using UnityEngine;

public class Game : MonoBehaviour {
    // Refrences the height and the width of the board. Public, can be changed from Unity
    public int width = 16, height = 16;
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
        // Draws the cell array
        board.Draw(state);
    }

    private void GenerateCells() {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y< height; y++) {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }
}
