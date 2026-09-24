#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class RoomConnection : MonoBehaviour
{
    // TODO: This is very messy, refactor when possible (expect bugs as well)

    /// <summary>
    /// A connection start at the left most row/column, depending on the rotation and size
    /// </summary>
    Vector2Int connectionStartPosition;
    [SerializeField] int connectionSize;

    public int GetConnectionSize() => connectionSize;
    public void SetConnectionSize(int connectionSize) => this.connectionSize = connectionSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cellSize"></param>
    public void Initialize(int cellSize)
    {
        connectionStartPosition = new Vector2Int(GetRow(cellSize), GetColumn(cellSize));
    }

    /// <summary>
    /// Gets the row where the connection is depending on the given cell size
    /// </summary>
    int GetRow(int cellSize)
    {
        switch (transform.localEulerAngles.y)
        {
            case 0: return Mathf.RoundToInt(transform.localPosition.z / cellSize);
            case 90: return Mathf.RoundToInt(transform.localPosition.z / cellSize) - 1;
            case 180: return Mathf.RoundToInt(transform.localPosition.z / cellSize) - 1;
            case 270: return Mathf.RoundToInt(transform.localPosition.z / cellSize);
            default: throw new InvalidOperationException("Rotation of connection not without bounds");
        }
    }
    /// <summary>
    /// Gets the column where the connection is depending on the given cell size
    /// </summary>
    int GetColumn(int cellSize)
    {
        switch (transform.localEulerAngles.y)
        {
            case 0: return Mathf.RoundToInt(transform.localPosition.x / cellSize);
            case 90: return Mathf.RoundToInt(transform.localPosition.x / cellSize);
            case 180: return Mathf.RoundToInt(transform.localPosition.x / cellSize) - 1;
            case 270: return Mathf.RoundToInt(transform.localPosition.x / cellSize) - 1;
            default: throw new InvalidOperationException("Rotation of connection not without bounds");
        }
    }

    /// <summary>
    /// Gets the foward Vector in the grid (e.g. (1,0), (-1,0), etc...).
    /// </summary>
    /// <returns>A Vector2Int where the x axis is that of transform.foward.x and y axis transform.foward.z</returns>
    Vector2Int GetFowardGridVector() => new Vector2Int(Mathf.RoundToInt(transform.forward.x), Mathf.RoundToInt(transform.forward.z));

    /// <summary>
    /// Gets the right Vector in the grid (e.g. (1,0), (-1,0), etc...).
    /// </summary>
    /// <returns>A Vector2Int where the x axis is that of transform.right.x and y axis transform.right.z</returns>
    Vector2Int GetRightGridVector() => new Vector2Int(Mathf.RoundToInt(transform.right.x), Mathf.RoundToInt(transform.right.z));

    public List<Vector2Int> GetConnectionCellsPositions(int cellSize)
    {
        List<Vector2Int> connectionCellsPositions = new List<Vector2Int>();

        Vector2Int rightGridVector = GetRightGridVector();

        int startRow = GetRow(cellSize);
        int startColumn = GetColumn(cellSize);

        // One of these will be the same number as connectionStartPos, which is intended
        int maxRow = startRow + (rightGridVector.y * connectionSize);
        int maxColumn = startColumn + (rightGridVector.x * connectionSize);

        // This is technically O(n)
        for (int row = Mathf.Min(startRow, maxRow); row < Mathf.Max(startRow, maxRow); row++)
        {
            for (int column = Mathf.Min(startColumn, maxColumn); column < Mathf.Max(startColumn, maxColumn); column++)
            {
                connectionCellsPositions.Add(new Vector2Int(row, column));
            }
        }

        return connectionCellsPositions;
    }

#if UNITY_EDITOR
    [Header("Editor Variables")]
    public GeneratedRoom linkedRoom;

    void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject != gameObject) return;
        if (!linkedRoom) return;
        int cellSize = linkedRoom.EDITOR_GetCellSizePreview();
        Vector2Int dimensions = linkedRoom.EDITOR_GetRowsAndColumns();

        int rows = dimensions.y;
        int columns = dimensions.x;

        int connectionRow = GetRow(cellSize);
        int connectionColumn = GetColumn(cellSize);

        // The fowards can be backward depending on the rotation, fowards is in relation to the connection
        Vector2Int fowardGridVector = GetFowardGridVector();
        Vector2Int rightGridVector = GetRightGridVector();

        int fowardRow = connectionRow + fowardGridVector.y;
        int fowardColumn = connectionColumn + fowardGridVector.x;

        // Debug.Log($"Rows: {rows}. Foward Row: {fowardRow}. Columns: {columns}. Foward Column: {fowardColumn}.");

        int rowMovement = connectionSize * rightGridVector.y;
        int columnMovement = connectionSize * rightGridVector.x;

        int connectionRowEnd = -1;
        int connectionColumnEnd = -1;

        // We subtract from the direction the connection is moving towards as to only mark the required cells as connection cells, and not their bounds. 
        // Case: Side Movement
        if (rowMovement == 0)
        {
            connectionRowEnd = connectionRow;

            if (columnMovement > 0) columnMovement--;
            else columnMovement++;

            connectionColumnEnd = connectionColumn + columnMovement;
        }
        // Case: Horizontal Movement
        else
        {
            connectionColumnEnd = connectionColumn;

            if (rowMovement > 0) rowMovement--;
            else rowMovement++;

            connectionRowEnd = connectionRow + rowMovement;
        }

        // Below is done for easier logic later
        if (connectionRow > connectionRowEnd)
        {
            int temp = connectionRow;
            connectionRow = connectionRowEnd;
            connectionRowEnd = temp;
        }
        if (connectionColumn > connectionColumnEnd)
        {
            int temp = connectionColumn;
            connectionColumn = connectionColumnEnd;
            connectionColumnEnd = temp;
        }

        // Debug.Log($"Con Row: {connectionRow}, end: {connectionRowEnd}");
        // Debug.Log($"Con Column: {connectionColumn}, end: {connectionColumnEnd}");
        // Debug.Log($"Fowards Vector: {fowardGridVector}");
        // Debug.Log($"Right Vector: {rightGridVector}");

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Gizmos.color = Color.black;

                // TODO: Put this in method?
                if (column >= connectionColumn && column <= connectionColumnEnd && row >= connectionRow && row <= connectionRowEnd)
                {
                    // Debug.Log($"Rows: {rows}. Foward Row: {fowardRow}. Columns: {columns}. Foward Column: {fowardColumn}.");
                    // Debug.Log($"Got Con Node!: Coords: ({row},{column}). Max/Min Column: {connectionColumn}/{connectionColumnEnd}. Max/Min Row: {connectionRow}/{connectionRowEnd}");

                    // Case: Connection facing up or down
                    if (fowardGridVector.x == 0)
                    {
                        if (fowardRow >= rows || fowardRow < 0) Gizmos.color = Color.green;
                        else Gizmos.color = Color.red;
                    }
                    // Case: Connection facing left or right
                    else
                    {
                        if (fowardColumn >= columns || fowardColumn < 0) Gizmos.color = Color.green;
                        else Gizmos.color = Color.red;
                    }
                }

                Gizmos.DrawCube(
                    new Vector3(
                        linkedRoom.transform.position.x + (column * cellSize) + (cellSize / 2.0f),
                        linkedRoom.transform.position.y + 0.039f,
                        linkedRoom.transform.position.z + (row * cellSize) + (cellSize / 2.0f)),
                    new Vector3(
                        cellSize - 0.039f,
                        0,
                        cellSize - 0.039f));
            }
        }
    }

    public void UpdateRoomConnection()
    {
        // TODO: DEPENDS ON ROTATIONS
        float yRotation = transform.localEulerAngles.y;

        if (yRotation == 90 || yRotation == 270)
        {
            connectionSize = Mathf.Clamp(connectionSize, 1, linkedRoom.EDITOR_GetRowsAndColumns().y);
        }
        else
        {
            connectionSize = Mathf.Clamp(connectionSize, 1, linkedRoom.EDITOR_GetRowsAndColumns().x);
        }
    }

    void Update()
    {
        if (!transform.hasChanged) return;
        if (!linkedRoom) return;

        int cellSize = linkedRoom.EDITOR_GetCellSizePreview();
        Vector2Int dimensions = linkedRoom.EDITOR_GetRowsAndColumns();

        int pivotRow = Mathf.RoundToInt(transform.localPosition.z / cellSize);
        int pivotColumn = Mathf.RoundToInt(transform.localPosition.x / cellSize);

        if (pivotRow > dimensions.y) pivotRow = dimensions.y;
        if (pivotRow < 0) pivotRow = 0;

        if (pivotColumn > dimensions.x) pivotColumn = dimensions.x;
        if (pivotColumn < 0) pivotColumn = 0;

        transform.localPosition = new Vector3(
            pivotColumn * cellSize,
            0,
            pivotRow * cellSize);

        int yRotation = 0;

        if (transform.localEulerAngles.y >= 270) yRotation = 270;
        else if (transform.localEulerAngles.y >= 180) yRotation = 180;
        else if (transform.localEulerAngles.y >= 90) yRotation = 90;

        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
#endif
}