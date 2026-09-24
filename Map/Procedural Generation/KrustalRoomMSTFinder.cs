using System.Collections.Generic;

/// <summary>
/// Helper Class used to find the Minimum Spanning Tree of a Graph consisting of Rooms using Kruskal’s Minimum Spanning Tree Algorithm
/// </summary>
public static class KrustalRoomMSTFinder
{
    /// <summary>
    /// </summary>
    /// <param name="sortedEdges">The graph to find the MST. Graph must be given with the weights in ascending order!</param>
    /// <returns>Edges of the Minimum Spanning Tree of the given graph</returns>
    public static HashSet<UndirectedRoomEdge> GetRoomGraphMST(UndirectedRoomEdge[] sortedEdges)
    {
        HashSet<UndirectedRoomEdge> mst = new HashSet<UndirectedRoomEdge>();

        // We simulate indices for the graph
        Dictionary<Room, int> roomIndexKeyValuePairs = new Dictionary<Room, int>();
        int currentIndex = 0;
        for (int i = 0; i < sortedEdges.Length; i++)
        {
            if (!roomIndexKeyValuePairs.ContainsKey(sortedEdges[i].Room1)) roomIndexKeyValuePairs.Add(sortedEdges[i].Room1, currentIndex++);
            if (!roomIndexKeyValuePairs.ContainsKey(sortedEdges[i].Room2)) roomIndexKeyValuePairs.Add(sortedEdges[i].Room2, currentIndex++);
        }

        // At the end of placing all rooms the dictionary, currentIndex will be equals to the number of rooms
        int roomCount = currentIndex;

        // Traverse edges in sorted order
        DisjointSetUnion dsu = new DisjointSetUnion(roomCount);
        int count = 0;

        for (int i = 0; i < sortedEdges.Length; i++)
        {
            int room1Index = roomIndexKeyValuePairs[sortedEdges[i].Room1];
            int room2Index = roomIndexKeyValuePairs[sortedEdges[i].Room2];

            // Make sure that there is no cycle
            if (dsu.Find(room1Index) != dsu.Find(room2Index))
            {
                dsu.Union(room1Index, room2Index);
                mst.Add(sortedEdges[i]);
                if (++count == roomCount - 1) break;
            }
        }
        return mst;
    }

    private class DisjointSetUnion
    {
        private int[] parent, rank;

        public DisjointSetUnion(int n)
        {
            parent = new int[n];
            rank = new int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                rank[i] = 1;
            }
        }

        public int Find(int index)
        {
            if (parent[index] != index)
            {
                parent[index] = Find(parent[index]);
            }
            return parent[index];
        }

        public void Union(int index1, int index2)
        {
            int s1 = Find(index1);
            int s2 = Find(index2);
            if (s1 != s2)
            {
                if (rank[s1] < rank[s2])
                {
                    parent[s1] = s2;
                }
                else if (rank[s1] > rank[s2])
                {
                    parent[s2] = s1;
                }
                else
                {
                    parent[s2] = s1;
                    rank[s1]++;
                }
            }
        }
    }
}