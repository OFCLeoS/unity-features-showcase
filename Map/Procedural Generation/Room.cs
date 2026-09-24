using MIConvexHull;
using UnityEngine;

/// <summary>
/// A representation of a facility room
/// </summary>
public class Room : IVertex
{
    public GameObject RoomObject { get; }
    public int Width { get; }
    public int Height { get; }
    public Vector2Int Pos { set; get; }
    public int Layer { set; get; }
    // TODO: Exits/Entries (struct?)

    Vector2Int displacement;
    /// <summary>
    /// The maximum the room can move per iteration of the map generator.
    /// </summary>
    public Vector2Int Displacement
    {
        set
        {
            Vector2Int valueReceived = value;

            // If displacement is (0,0) (which may happen with Displacement randomization) we randomize it to a non zero vector
            if (valueReceived.Equals(Vector2Int.zero))
            {
                valueReceived = GetRandomVectorThatIsNotZero();
            }

            bool negativeX = valueReceived.x < 0;
            bool negativeY = valueReceived.y < 0;

            // To calculate the minDisplacement, we first get the absolute values, and negate them if needed later on.
            int absX = Mathf.Abs(valueReceived.x);
            int absY = Mathf.Abs(valueReceived.y);

            int absMinX = Mathf.Max(Mathf.FloorToInt(absX - (absX * 0.1f)), 0);
            int absMinY = Mathf.Max(Mathf.FloorToInt(absY - (absY * 0.1f)), 0);

            if (absMinX == 0 && absMinY == 0)
            {
                Vector2Int randomNonZeroVector = GetRandomVectorThatIsNotZero();
                absMinX = Mathf.Abs(randomNonZeroVector.x);
                absMinY = Mathf.Abs(randomNonZeroVector.y);
            }

            minDisplacement.x = negativeX ? -absMinX : absMinX;
            minDisplacement.y = negativeY ? -absMinY : absMinY;

            displacement = valueReceived;
        }
        get => displacement;
    }
    /// <summary>
    /// Right now should be 10% less than the Displacement
    /// </summary>
    Vector2Int minDisplacement;

    /// <summary>
    /// Is this Room position final?
    /// </summary>
    public bool IsPlaced { set; get; } = false;

    // TODO: Change to average position of exits?
    // This is what decides on the connections, right now it's the middle point, could just later. (Used by MIConvexHull)
    double[] IVertex.Position => new double[] { Pos.x + (Width / 2.0f), Pos.y + (Height / 2.0f) };

    #region Initialization
    public Room(GameObject roomObject, int width, int height) : this(roomObject, width, height, 0, Vector2Int.zero) { }

    public Room(GameObject roomObject, int width, int height, int layer) : this(roomObject, width, height, layer, Vector2Int.zero) { }

    public Room(GameObject roomObject, int width, int height, int layer, Vector2Int startingPos)
    {
        RoomObject = roomObject;
        Width = width;
        Height = height;
        Layer = layer;
        Pos = startingPos;
        IsPlaced = false;
    }

    public Room(GameObject roomObject, int width, int height, int layer, Vector2Int startingPos, Vector2Int startingDisplacement)
    {
        RoomObject = roomObject;
        Width = width;
        Height = height;
        Layer = layer;
        Pos = startingPos;
        Displacement = startingDisplacement;
        IsPlaced = false;
    }
    #endregion

    /// <summary>
    /// Uses the Room's Displacement Vector to change its coordinates
    /// </summary>
    public void DisplaceRoom() => Pos += new Vector2Int(Random.Range(minDisplacement.x, displacement.x + 1), Random.Range(minDisplacement.y, displacement.y + 1));

    /// <summary>
    /// Helper Method for Displacement setting edge cases
    /// </summary>
    Vector2Int GetRandomVectorThatIsNotZero()
    {
        Vector2Int value = Vector2Int.zero;

        int changeBoth = Random.Range(0, 2);

        if (changeBoth == 1)
        {
            int isNegative = Random.Range(0, 2);

            if (isNegative == 1) value.x = -1;
            else value.x = 1;

            isNegative = Random.Range(0, 2);

            if (isNegative == 1) value.y = -1;
            else value.y = 1;
        }
        else
        {
            int changeX = Random.Range(0, 2);
            if (changeX == 1)
            {
                int xNegative = Random.Range(0, 2);

                if (xNegative == 1) value.x = -1;
                else value.x = 1;
            }
            else
            {
                int yNegative = Random.Range(0, 2);

                if (yNegative == 1) value.y = -1;
                else value.y = 1;
            }
        }
        return value;
    }
}