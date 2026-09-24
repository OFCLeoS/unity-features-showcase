using System.Collections.Generic;
using System.Linq;
using MIConvexHull;
using UnityEngine;

/// <summary>
/// Generates a Random Map
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<GeneratedRoom> roomsToGenerate;
    List<Room> rooms = new List<Room>();

    [Tooltip("How big is a cell compared to 1 Unity Unit. (E.g. Cell Size = 5 means that 1 cell is equivalent to 5 Unity Units)")]
    [SerializeField] int cellSize;

    [Tooltip("How far from 0,0 can rooms spawn at")]
    [SerializeField] Vector2Int spawnJitter;

    [Tooltip("How fast can rooms move when collisions are being resolved")]
    [SerializeField] Vector2Int displacementSpeed;

    [Tooltip("The percentage of additional edges on top of the MST that will be used for room connections (in order to add more variety to the map).")]
    [SerializeField] float additionalEdgesPercentage = 0.2f;

    FacilityFloorGrid currentFloorMap;

    #region Initialization
    void Awake()
    {
        spawnJitter.x = Mathf.Abs(spawnJitter.x);
        spawnJitter.y = Mathf.Abs(spawnJitter.y);
        displacementSpeed.x = Mathf.Abs(displacementSpeed.x);
        displacementSpeed.y = Mathf.Abs(displacementSpeed.y);
        if (cellSize <= 0)
        {
            StopMapGenDueToError("The Map Generator has an invalid cell size, it will not work!");
        }
    }
    #endregion

    void CreateNewFloor()
    {
        if (!SpawnRooms())
        {
            StopMapGenDueToError("The Map Generator encountered an error when creating the rooms array!");
        }
        // TODO: Process could be changed for different map results? Make them modifable on inspector?

        // We create the map with the newly created Room list
        // TODO!!!

        // We set random layers to all the rooms
        SetLayers();
        // We add spawn jitter to all the rooms
        AddSpawnJitter();
        // We set random displacement vectors to all the rooms
        SetDisplacements();
        // We Resolve all Room Collisions
        ResolveRoomCollisions();

        HashSet<UndirectedRoomEdge> roomsGraph = CreateRoomsGraph();

        UndirectedRoomEdge[] roomsGraphSortedArray = roomsGraph.ToArray();
        System.Array.Sort(roomsGraphSortedArray);

        // foreach (UndirectedRoomEdge ure in roomsGraphSortedArray)
        // {
        //     Vector3 room1Center = new Vector3(ure.Room1.Pos.x + ure.Room1.Width / 2.0f, 0, ure.Room1.Pos.y + ure.Room1.Height / 2.0f);
        //     Vector3 room2Center = new Vector3(ure.Room2.Pos.x + ure.Room2.Width / 2.0f, 0, ure.Room2.Pos.y + ure.Room2.Height / 2.0f);

        //     Debug.DrawLine(room1Center, room2Center, Color.blue, 10f);
        // }

        HashSet<UndirectedRoomEdge> roomsGraphMST = KrustalRoomMSTFinder.GetRoomGraphMST(roomsGraphSortedArray);

        // foreach (UndirectedRoomEdge ure in roomsGraphMST)
        // {
        //     Vector3 room1Center = new Vector3(ure.Room1.Pos.x + ure.Room1.Width / 2.0f, 0, ure.Room1.Pos.y + ure.Room1.Height / 2.0f);
        //     Vector3 room2Center = new Vector3(ure.Room2.Pos.x + ure.Room2.Width / 2.0f, 0, ure.Room2.Pos.y + ure.Room2.Height / 2.0f);

        //     Debug.DrawLine(room1Center, room2Center, Color.red, 3f);
        // }

        RemoveMSTEdgesFromRoomsGraph(roomsGraphMST, roomsGraph);

        currentFloorMap = new FacilityFloorGrid(rooms);

        // We create the connections from the edges of the MST
        CreateRoomConnections(roomsGraphMST);
        // We create the connections from random edges
        CreateRoomConnections(GetRandomEdgesFromGraph(roomsGraph.ToList(), additionalEdgesPercentage));

        // TODO: INSTANTIATE HALLWAYS AS WELL
        // We Instantiate all the Rooms in-game
        InstantiateRooms();
    }

    #region Map Creation Steps
    /// <summary>
    /// Adds a representation of all the rooms to be spawned to the rooms array.
    /// </summary>
    /// <returns>True if successful</returns>
    bool SpawnRooms()
    {
        if (roomsToGenerate.Count <= 0) return false;
        rooms.Clear();
        foreach (GeneratedRoom room in roomsToGenerate)
        {
            rooms.Add(room.GetRoomRepresentation(cellSize));
        }
        return true;
    }

    /// <summary>
    /// Sets a random Layer to all room coordinates
    /// </summary>
    void SetLayers()
    {
        // !!! HAVING 2+ ROOMS WITH THE SAME LAYER IS DANGEROUS AND WILL LEAD TO ANOMALIES !!!
        // The maximum layer number will be set to the number of rooms in the map
        // TODO: CHANGE LAYER SYSTEM, CERTIFIY NO 2 ROOMS HAVE THE SAME LAYER
        int currentLayer = 0;
        foreach (Room room in rooms)
        {
            room.Layer = currentLayer++;
        }
    }

    /// <summary>
    /// Adds a random vector to all room coordinates
    /// </summary>
    void AddSpawnJitter()
    {
        foreach (Room room in rooms)
        {
            Vector2Int roomSpawnJitter = new Vector2Int(Random.Range(-spawnJitter.x, spawnJitter.x + 1), Random.Range(-spawnJitter.y, spawnJitter.y + 1));
            room.Pos += roomSpawnJitter;
        }
    }

    /// <summary>
    /// Sets a random Displacement vector to all room coordinates
    /// </summary>
    void SetDisplacements()
    {
        foreach (Room room in rooms)
        {
            // TODO: GAUSSIAN DISTRIBUTION?
            Vector2Int roomDisplacementSpeed = new Vector2Int(Random.Range(-displacementSpeed.x, displacementSpeed.x + 1), Random.Range(-displacementSpeed.y, displacementSpeed.y + 1));
            room.Displacement = roomDisplacementSpeed;
        }
    }

    /// <summary>
    /// Pushes rooms away from each other using their Displacement Vectors until no collisions are detected
    /// </summary>
    void ResolveRoomCollisions()
    {
        bool allRoomsPlaced = false;
        int placedRoomsCount = 0;
        while (!allRoomsPlaced)
        {
            // TODO: Could remove placed rooms, but may need them in the list later...
            foreach (Room room in rooms)
            {
                if (room.IsPlaced) continue;
                if (RoomIsColliding(room)) room.DisplaceRoom();
                else
                {
                    placedRoomsCount++;
                    room.IsPlaced = true;
                }
                // TODO: Could early stop here with below if statement? Or not worth it?
            }
            if (placedRoomsCount >= rooms.Count) allRoomsPlaced = true;
        }
    }


    /// <summary>
    /// Creates a Graph of the rooms using Delaunay Triangulation
    /// </summary>
    HashSet<UndirectedRoomEdge> CreateRoomsGraph()
    {
        HashSet<UndirectedRoomEdge> graph = new HashSet<UndirectedRoomEdge>();

        // Could also change the PlaneDistanceTolerance if doing some freaky y-axis stuff
        DelaunayTriangulation<Room, DefaultTriangulationCell<Room>> roomsDelaunayTriangulation =
            DelaunayTriangulation<Room, DefaultTriangulationCell<Room>>.Create(rooms, Constants.DefaultPlaneDistanceTolerance);

        // For each triangle, we add it's edges to the HashSet (which will automatically remove duplicates)
        foreach (DefaultTriangulationCell<Room> cell in roomsDelaunayTriangulation.Cells)
        {
            graph.Add(new UndirectedRoomEdge(cell.Vertices[0], cell.Vertices[1]));
            graph.Add(new UndirectedRoomEdge(cell.Vertices[1], cell.Vertices[2]));
            graph.Add(new UndirectedRoomEdge(cell.Vertices[2], cell.Vertices[0]));
        }

        return graph;
    }

    /// <summary>
    /// Removes all MST Edges from the rooms graph. Is used to then select new, random edges to add to the facility.
    /// </summary>
    /// <param name="mst">The Minimum Spanning Tree</param>
    /// <param name="roomsGraph">The rooms graph</param>
    void RemoveMSTEdgesFromRoomsGraph(HashSet<UndirectedRoomEdge> mst, HashSet<UndirectedRoomEdge> roomsGraph)
    {
        foreach (UndirectedRoomEdge edge in mst)
        {
            roomsGraph.Remove(edge);
        }
    }

    /// <summary>
    /// Gets a certain percentage of random edges from a given graph.
    /// </summary>
    /// <param name="roomGraph"></param>
    /// <param name="percentageOfEdges">The percentage of edges from the "roomGraph" to get</param>
    /// <returns>"percentageOfEdges"% of random edges from the given graph</returns>
    HashSet<UndirectedRoomEdge> GetRandomEdgesFromGraph(List<UndirectedRoomEdge> roomGraph, float percentageOfEdges)
    {
        List<UndirectedRoomEdge> roomGraphCopy = new List<UndirectedRoomEdge>(roomGraph);

        float clampedPercentageOfEdges = Mathf.Clamp(percentageOfEdges, 0, 1);
        int numberOfEdgesToReturn = Mathf.RoundToInt(roomGraphCopy.Count * clampedPercentageOfEdges);

        HashSet<UndirectedRoomEdge> randomEdges = new HashSet<UndirectedRoomEdge>();

        for (int i = 0; i < numberOfEdgesToReturn; i++)
        {
            int ran = Random.Range(0, roomGraphCopy.Count);
            randomEdges.Add(roomGraphCopy[ran]);
            roomGraphCopy.RemoveAt(ran);
        }

        return randomEdges;
    }

    /// <summary>
    /// Creates connections between 2 rooms by using the given edges
    /// </summary>
    void CreateRoomConnections(HashSet<UndirectedRoomEdge> edgesToConnect)
    {
        //TODO: Pick closest connections

    }

    /// <summary>
    /// Correctly instantiates all the rooms in the scene depending on their position and the cell size of the map
    /// </summary>
    void InstantiateRooms()
    {
        foreach (Room room in rooms)
        {
            Instantiate(room.RoomObject, new Vector3(room.Pos.x * cellSize, 0, room.Pos.y * cellSize), Quaternion.identity);
        }
    }

    /// <summary>
    /// Checks if a room collides with any other room in the layer below or equals to it
    /// </summary>
    /// <returns></returns>
    bool RoomIsColliding(Room roomToCheck)
    {
        foreach (Room room in rooms)
        {
            if (room == roomToCheck) continue;
            // We check for IsPlaced because we do not want to place rooms over already placed rooms
            if (room.Layer >= roomToCheck.Layer || room.IsPlaced)
            {
                // AABB Bottom Left Pivot
                float aXMin = room.Pos.x;
                float aXMax = room.Pos.x + room.Width;
                float aYMin = room.Pos.y;
                float aYMax = room.Pos.y + room.Height;

                float bXMin = roomToCheck.Pos.x;
                float bXMax = roomToCheck.Pos.x + roomToCheck.Width;
                float bYMin = roomToCheck.Pos.y;
                float bYMax = roomToCheck.Pos.y + roomToCheck.Height;

                bool collides =
                    aXMin < bXMax &&
                    aXMax > bXMin &&
                    aYMin < bYMax &&
                    aYMax > bYMin;

                if (collides) return true;
            }
        }
        return false;
    }

    #endregion

    // TODO: SEED SYSTEM!!!!!!!!!

    #region DEBUGGING
#if UNITY_EDITOR
    public bool DEBUG_START_COLLISION_HANDLING;
    [ContextMenu("DEBUG_START_MAP_GEN")]
    public void DEBUG_START_MAP_GEN() => CreateNewFloor();
    [ContextMenu("DEBUG_MAP_GEN_DEBUGGING")]
    public void DEBUG_MAP_GEN_DEBUGGING() => DEBUG_MAP_GEN();

    Dictionary<Room, Transform> DEBUG_INSTANTIATED_ROOMS = new Dictionary<Room, Transform>();
    public float DEBUG_STEP_DELAY = 0.25f;
    float DEBUG_TIME_SINCE_LAST_STEP = 0;
    float DEBUG_NEXT_STEP_TIME = 0;
    int DEBUG_PLACED_ROOMS = 0;

    [ContextMenu("DEBUG_CRASH_CHECK")]
    public void DEBUG_CRASH_CHECK()
    {
        for (int i = 0; i < 1000; i++)
        {
            CreateNewFloor();
        }
    }

    void DEBUG_MAP_GEN()
    {
        if (!SpawnRooms())
        {
            StopMapGenDueToError("The Map Generator encountered an error when creating the rooms array!");
        }
        // TODO: Process could be changed for different map results? Make them modifable on inspector?

        // We create the map with the newly created Room list
        // TODO!!!

        // We set random layers to all the rooms
        SetLayers();
        // We add spawn jitter to all the rooms
        AddSpawnJitter();
        // We set random displacement vectors to all the rooms
        SetDisplacements();
        // We Spawn all rooms so that we have visible changes
        foreach (Room room in rooms)
        {
            DEBUG_INSTANTIATED_ROOMS.Add(room, Instantiate(room.RoomObject, new Vector3(room.Pos.x * cellSize, 0, room.Pos.y * cellSize), Quaternion.identity).transform);
        }
        // We Resolve all Room Collisions
        DEBUG_START_COLLISION_HANDLING = true;
        DEBUG_TIME_SINCE_LAST_STEP = 0;
        DEBUG_NEXT_STEP_TIME = DEBUG_STEP_DELAY;
    }

    void DEBUG_COLLISION_HANDLING_STEP()
    {
        // TODO: Could remove placed rooms, but may need them in the list later...
        foreach (Room room in rooms)
        {
            if (room.IsPlaced) continue;
            if (RoomIsColliding(room))
            {
                room.DisplaceRoom();
                DEBUG_INSTANTIATED_ROOMS[room].position = new Vector3(room.Pos.x * cellSize, 0, room.Pos.y * cellSize);
            }
            else
            {
                DEBUG_PLACED_ROOMS++;
                room.IsPlaced = true;
            }
            // TODO: Could early stop here with below if statement? Or not worth it?
        }
    }

    void Update()
    {
        if (DEBUG_START_COLLISION_HANDLING)
        {
            DEBUG_TIME_SINCE_LAST_STEP += Time.deltaTime;
            if (DEBUG_TIME_SINCE_LAST_STEP >= DEBUG_NEXT_STEP_TIME)
            {
                DEBUG_COLLISION_HANDLING_STEP();
                DEBUG_TIME_SINCE_LAST_STEP = 0;
                DEBUG_NEXT_STEP_TIME = DEBUG_STEP_DELAY;
            }
            if (DEBUG_PLACED_ROOMS >= rooms.Count)
            {
                DEBUG_PLACED_ROOMS = 0;
                DEBUG_START_COLLISION_HANDLING = false;
            }
        }
    }
#endif
    #endregion

    #region Error Handling
    /// <summary>
    /// Stops the Generation of the Map due to an error.
    /// </summary>
    void StopMapGenDueToError(string errorText)
    {
#if UNITY_EDITOR
        Debug.LogError(errorText);
#endif
        gameObject.SetActive(false);
    }
    #endregion
}