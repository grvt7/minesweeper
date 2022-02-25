using UnityEngine;
using UnityEngine.Tilemaps; // Importing tilemap to greate an object of tilemap.

public class Board : MonoBehaviour {
    // Refrences the tile map from the hierarchy.
    // Public getter to get the tile but private setter.
    public Tilemap tilemap {get; private set;}

    // To get refrences to tile states.
    public Tile tileUnknown, tileEmpty, tileMine, tileExploded, tileFlag;
    // Getting refrences to each numbered tile.
    public Tile tile1;
    public Tile tile2;
    public Tile tile3;
    public Tile tile4;
    public Tile tile5;
    public Tile tile6;
    public Tile tile7;
    public Tile tile8;

    private void Awake() {
        // Refrences the tilemap by getting it from the component on which the script is running
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell[,] state) {
        // Getting width (x) and height (y) of the state  or 2D Matrix
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        // Loops over the board using the height and width
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                // Getting the individual Cell Data
                Cell cell = state[x,y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell) {
        if(cell.revealed) {
            return GetRevealedTile(cell);
        }
        else if(cell.flagged) {
            return tileFlag;
        }
        else {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell) {
        switch (cell.type) {
            case Cell.Type.Empty: return tileEmpty;   
            case Cell.Type.Mine: return tileMine;         
            case Cell.Type.Number: return GetNumberedTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberedTile(Cell cell) {
        switch (cell.number) {
            case 1: return tile1;            
            case 2: return tile2;            
            case 3: return tile3;            
            case 4: return tile4;            
            case 5: return tile5;            
            case 6: return tile6;            
            case 7: return tile7;            
            case 8: return tile8;            
            default: return null;
        }
    }
}
