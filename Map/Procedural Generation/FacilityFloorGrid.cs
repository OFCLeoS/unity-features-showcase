using System;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

/// <summary>
/// A grid representation of a facility floor
/// </summary>
public class FacilityFloorGrid
{
    private const int DEFAULT_MARGIN_SIZE = 10;
    private const int DEFAULT_CELL_SIZE = 1;

    private readonly int cellSize = DEFAULT_CELL_SIZE;

    /// <summary>
    /// If a Coordinate does not have a list room reference, it means it is empty
    /// </summary>
    public Node[,] FloorGrid { get; private set; }

    /// <summary>
    /// Add this to world/room pos x to get grid position x
    /// </summary>
    public int OffsetX { get; private set; }
    /// <summary>
    /// Add this to world/room pos y to get grid position y
    /// </summary>
    public int OffsetY { get; private set; }

    public FacilityFloorGrid(List<Room> rooms) : this(rooms, DEFAULT_MARGIN_SIZE, DEFAULT_CELL_SIZE) { }

    public FacilityFloorGrid(List<Room> rooms, int cellSize) : this(rooms, DEFAULT_MARGIN_SIZE, cellSize) { }

    public FacilityFloorGrid(List<Room> rooms, int marginSize, int cellSize)
    {
        if (marginSize < 0) throw new ArgumentException("Margin must be above or equals to zero.");
        if (cellSize <= 0) throw new ArgumentException("Cell Size must be above zero.");
        if (rooms == null) throw new ArgumentNullException(nameof(rooms));

        this.cellSize = cellSize;

        int minX = int.MaxValue;
        int maxX = int.MinValue;

        int minY = int.MaxValue;
        int maxY = int.MinValue;

        // We get the extremities of the map
        foreach (Room room in rooms)
        {
            minX = Mathf.Min(room.Pos.x, minX);
            maxX = Mathf.Max(room.Pos.x + room.Width, maxX);

            minY = Mathf.Min(room.Pos.y, minY);
            maxY = Mathf.Max(room.Pos.y + room.Height, maxY);
        }

        // We add a margin
        minX -= marginSize;
        maxX += marginSize;

        minY -= marginSize;
        maxY += marginSize;

        // We save the offset for later translations
        OffsetX = -minX;
        OffsetY = -minY;

        int rows = maxY - minY + 1;
        int columns = maxX - minX + 1;

        FloorGrid = new Node[rows, columns];

        // We map out the rooms first
        foreach (Room room in rooms)
        {
            int roomRowStart = room.Pos.y + OffsetY;
            int roomRowEnd = roomRowStart + room.Height - 1;

            int roomColumnStart = room.Pos.x + OffsetX;
            int roomColumnEnd = roomColumnStart + room.Width - 1;

            for (int row = roomRowStart; row <= roomRowEnd; row++)
            {
                for (int column = roomColumnStart; column <= roomColumnEnd; column++)
                {
                    //Debug.Log($"Row Num: {rows}. Column Num: {column}. Row Start/End: {roomRowStart}/{roomRowEnd}. Column Start/End: {roomColumnStart}/{roomColumnEnd}. Current: ({row},{column})");
                    FloorGrid[row, column] = new Node(new Vector2Int(column - OffsetX, row - OffsetY), true);
                }
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (FloorGrid[row, column] == null) FloorGrid[row, column] = new Node(new Vector2Int(column - OffsetX, row - OffsetY), false);
            }
        }

        //DebugDrawFacilityFloor();
    }

#if UNITY_EDITOR
    void DebugDrawFacilityFloor()
    {
        for (int row = 0; row < FloorGrid.GetLength(0); row++)
        {
            for (int column = 0; column < FloorGrid.GetLength(1); column++)
            {
                Color colour = Color.blue;
                if (FloorGrid[row, column].isRoom) colour = Color.black;

                ShapeDrawer.DebugDrawSquare(new Vector3(FloorGrid[row, column].position.x * cellSize, 0, FloorGrid[row, column].position.y * cellSize), cellSize, cellSize, 10f, colour);
            }
        }
    }
#endif
}