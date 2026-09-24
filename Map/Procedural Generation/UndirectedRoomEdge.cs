using System;
using UnityEngine;

public class UndirectedRoomEdge : IComparable<UndirectedRoomEdge>
{
    public Room Room1 { get; private set; }
    public Room Room2 { get; private set; }

    /// <summary>
    /// Squared Magnitude between the midle of the 2 rooms
    /// </summary>
    public float Weight { get; private set; } // TODO: Randomize this a bit?

    public UndirectedRoomEdge(Room room1, Room room2)
    {
        if (room1 == null) throw new ArgumentNullException("room1");
        if (room2 == null) throw new ArgumentNullException("room2");

        this.Room1 = room1;
        this.Room2 = room2;

        Vector2 room1Middle = room1.Pos + new Vector2(room1.Width / 2.0f, -room1.Height / 2.0f);
        Vector2 room2Middle = room2.Pos + new Vector2(room2.Width / 2.0f, -room2.Height / 2.0f);

        Weight = (room2Middle - room1Middle).sqrMagnitude;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        UndirectedRoomEdge roomEdgeToCompare = (UndirectedRoomEdge)obj;
        return
        (roomEdgeToCompare.Room1 == Room1 && roomEdgeToCompare.Room2 == Room2)
        ||
        (roomEdgeToCompare.Room2 == Room1 && roomEdgeToCompare.Room1 == Room2);
    }

    public override int GetHashCode()
    {
        int hash1 = Room1.GetHashCode();
        int hash2 = Room2.GetHashCode();

        // Make order irrelevant
        if (hash1 > hash2)
        {
            int temp = hash2;
            hash2 = hash1;
            hash1 = temp;
        }

        return HashCode.Combine(hash1, hash2);
    }

    public int CompareTo(UndirectedRoomEdge other)
    {
        return Weight.CompareTo(other.Weight);
    }
}