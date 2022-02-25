using UnityEngine;

// Struct Cell represents the structure of each individual Cell.
public struct Cell {
    // Enum defined they types of cell that it could be.
    public enum Type {
        Empty,
        Mine,
        Number,
    }

    // Contains the position of the cell inside the 2d Matrix.
    public Vector3Int position;
    // Contains the type of cell it is.
    public Type type;
    // Number contains how many mines are around it
    public int number;
    // Contains if the cell is revealed or not, flagged or not or if its is a mine exploded or not.
    public bool revealed, flagged, exploded;
}
