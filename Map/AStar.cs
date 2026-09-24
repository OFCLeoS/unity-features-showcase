using UnityEngine;
using System.Collections.Generic;

namespace PathFinding
{
    public class Node
    {
        public Vector2Int position;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isRoom;

        public Node parent;

        public Node(Vector2Int position)
        {
            this.position = position;
        }
        public Node(Vector2Int position, bool isRoom)
        {
            this.position = position;
            this.isRoom = isRoom;
        }
    }

    public static class AStar
    {
        public static List<Vector2Int> GetPath(Node[,] map, Vector2Int startPos, Vector2Int targetPos)
        {
            List<Node> nodesToBeEvaluated = new List<Node>(); // TODO: switch this for a sorted collection?
            HashSet<Node> evaluatedNodes = new HashSet<Node>();

            Node startNode = map[startPos.y, startPos.x];
            Node targetNode = map[targetPos.y, targetPos.x];

            nodesToBeEvaluated.Add(startNode);

            // We continue while there are nodes to be evaluated
            while (nodesToBeEvaluated.Count > 0)
            {
                Node nodeBeingEvaluated = nodesToBeEvaluated[0];

                // (Remove if sorted collection is implemented)
                // We find the node with the cheapest fCost
                for (int i = 1; i < nodesToBeEvaluated.Count; i++)
                {
                    if (
                        (nodesToBeEvaluated[i].fCost < nodeBeingEvaluated.fCost)
                        ||
                        (nodesToBeEvaluated[i].fCost == nodeBeingEvaluated.fCost && nodesToBeEvaluated[i].hCost < nodeBeingEvaluated.hCost))
                    {
                        nodeBeingEvaluated = nodesToBeEvaluated[i];
                    }
                }

                nodesToBeEvaluated.Remove(nodeBeingEvaluated);
                evaluatedNodes.Add(nodeBeingEvaluated);

                if (nodeBeingEvaluated == targetNode) return GetPathPositions(startNode, targetNode);

                // We add neighbouring nodes to the nodesToBeEvaluated list
                foreach (Node neighbouringNode in GetNodeNeighbours(nodeBeingEvaluated, map))
                {
                    if (neighbouringNode.isRoom || evaluatedNodes.Contains(neighbouringNode)) continue;

                    int newNeighbourCost = nodeBeingEvaluated.gCost + GetManhattanDistance(nodeBeingEvaluated, neighbouringNode);
                    if (newNeighbourCost < neighbouringNode.gCost || !nodesToBeEvaluated.Contains(neighbouringNode))
                    {
                        neighbouringNode.gCost = newNeighbourCost;
                        neighbouringNode.hCost = GetManhattanDistance(neighbouringNode, targetNode);
                        neighbouringNode.parent = nodeBeingEvaluated;

                        if (!nodesToBeEvaluated.Contains(neighbouringNode)) nodesToBeEvaluated.Add(neighbouringNode);
                    }
                }
            }
            // If the code reaches here, a path was not found
            return null;
        }

        static List<Vector2Int> GetPathPositions(Node startNode, Node endNode)
        {
            List<Vector2Int> pathPositions = new List<Vector2Int>();

            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                pathPositions.Add(currentNode.position);
                currentNode = currentNode.parent;
            }
            pathPositions.Reverse();

            return pathPositions;
        }

        static List<Node> GetNodeNeighbours(Node node, Node[,] map)
        {
            // TODO: If there is a bug with pathing, double check this method
            List<Node> nodeNeighbours = new List<Node>();

            int mapRowCount = map.GetLength(0);
            int mapColumnCount = map.GetLength(1);

            if (node.position.x < mapColumnCount - 1)
            {
                Node neighbouringNode = map[node.position.x + 1, node.position.y];
                if (!neighbouringNode.isRoom) nodeNeighbours.Add(neighbouringNode);
            }
            if (node.position.x > 0)
            {
                Node neighbouringNode = map[node.position.x - 1, node.position.y];
                if (!neighbouringNode.isRoom) nodeNeighbours.Add(neighbouringNode);
            }
            if (node.position.y < mapRowCount - 1)
            {
                Node neighbouringNode = map[node.position.x, node.position.y + 1];
                if (!neighbouringNode.isRoom) nodeNeighbours.Add(neighbouringNode);
            }
            if (node.position.y > 0)
            {
                Node neighbouringNode = map[node.position.x, node.position.y - 1];
                if (!neighbouringNode.isRoom) nodeNeighbours.Add(neighbouringNode);
            }

            return nodeNeighbours;
        }

        static int GetManhattanDistance(Node nodeA, Node nodeB)
        {
            return Mathf.Abs(nodeA.position.x - nodeB.position.x) + Mathf.Abs(nodeA.position.y - nodeB.position.y);
        }
    }
}