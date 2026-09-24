#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] string areaName;

    [Tooltip("How many enemies will spawn in this area. Goes from 0 to 1, with 0 no enemies or barely any enemies and 1 meaning a huge amount of enemies. Is dependant on the size of the area and the difficulty of the mission.")]
    [Range(0, 1)]
    [SerializeField] float enemyPresenceLevel;
    [SerializeField] RectanglePiece[] areaRectangles;
    [SerializeField] CirclePiece[] areaCircles;

    List<Vector3> allEntryPoints = new List<Vector3>();

    public float GetEnemyPresenceLevel() => enemyPresenceLevel;

    #region Initialization
    void Awake()
    {
        if (transform.localScale != Vector3.one)
        {
            Debug.LogWarning("Scaled Map Areas may result in unexpected behaviour with rotated pieces. (" + transform + ")");
        }
        CreateColliders();
        ConvertEntryPointsToWorldSpace();
    }

    void CreateColliders()
    {
        int colliderNum = 0;
        for (int i = 0; i < areaRectangles.Length; i++)
        {
            colliderNum++;
            GameObject colliderGO = new GameObject(areaName + " Area Collider " + colliderNum);
            colliderGO.layer = LayerMask.NameToLayer("Map Area");
            colliderGO.transform.parent = transform;
            colliderGO.transform.localPosition = areaRectangles[i].piecePosition;
            //Negative because I apperently fucked up the angle calculations or smt idk
            colliderGO.transform.localRotation = Quaternion.Euler(new Vector3(0, -areaRectangles[i].angle, 0));
            BoxCollider collider = colliderGO.AddComponent<BoxCollider>(); ;
            collider.center = new Vector3(
                areaRectangles[i].length / 2,
                0,
                areaRectangles[i].width / 2);
            collider.size = new Vector3(areaRectangles[i].length, 0, areaRectangles[i].width);
            collider.isTrigger = true;
        }
        for (int i = 0; i < areaCircles.Length; i++)
        {
            colliderNum++;
            GameObject colliderGO = new GameObject(areaName + " Area Collider " + colliderNum);
            colliderGO.layer = LayerMask.NameToLayer("Map Area");
            colliderGO.transform.parent = transform;
            colliderGO.transform.localPosition = areaCircles[i].piecePosition;
            colliderGO.transform.localRotation = Quaternion.identity;
            BoxCollider collider = colliderGO.AddComponent<BoxCollider>(); ;
            collider.size = new Vector3(areaCircles[i].radius * 2, 0, areaCircles[i].radius * 2);
            collider.isTrigger = true;
        }
    }

    void ConvertEntryPointsToWorldSpace()
    {
        for (int i = 0; i < areaRectangles.Length; i++)
        {
            float angle = areaRectangles[i].angle * (Mathf.PI / 180);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            for (int j = 0; j < areaRectangles[i].entryPoints.Length; j++)
            {
                Vector3 entryPointPosition = new Vector3(
                    areaRectangles[i].entryPoints[j].x * cos - areaRectangles[i].entryPoints[j].z * sin,
                    areaRectangles[i].entryPoints[j].y,
                    areaRectangles[i].entryPoints[j].z * cos + areaRectangles[i].entryPoints[j].x * sin);
                entryPointPosition += areaRectangles[i].piecePosition;
                allEntryPoints.Add(entryPointPosition);
            }
        }
        for (int i = 0; i < areaCircles.Length; i++)
        {
            for (int j = 0; j < areaCircles[i].entryPoints.Length; j++)
            {
                Vector3 entryPointPosition = areaCircles[i].entryPoints[j] + areaCircles[i].piecePosition;
                allEntryPoints.Add(entryPointPosition);
            }
        }
    }
    #endregion

#if UNITY_EDITOR
    [SerializeField] private bool debugRandomPosition;
#endif

    public Vector3[] GetAllEntryPoints() => allEntryPoints.ToArray();

    public Vector3 GetRandomPositionInArea()
    {
        float ran = Random.Range(0f, 1f);
        if (areaRectangles.Length <= 0) ran = 1;
        else if (areaCircles.Length <= 0) ran = 0;
        if (ran < 0.5f)
        {
            RectanglePiece rectanglePiece = areaRectangles[Random.Range(0, areaRectangles.Length)];
            float randomLocalCenteredX = Random.Range(0, rectanglePiece.length);
            float randomLocalCenteredZ = Random.Range(0, rectanglePiece.width);
            float angle = rectanglePiece.angle * (Mathf.PI / 180); ;

            //Here we rotate the point around the rectangle start and add the rectangles local position to match the point of the non-rotated rectangle to the rotated rectangle
            float localX = (randomLocalCenteredX * Mathf.Cos(angle)) - (randomLocalCenteredZ * Mathf.Sin(angle)) + rectanglePiece.piecePosition.x;
            float localY = rectanglePiece.piecePosition.y;
            float localZ = (randomLocalCenteredZ * Mathf.Cos(angle)) + (randomLocalCenteredX * Mathf.Sin(angle)) + rectanglePiece.piecePosition.z;
            return transform.TransformPoint(localX, localY, localZ);
        }
        else
        {
            CirclePiece circlePiece = areaCircles[Random.Range(0, areaCircles.Length)];
            float randomRadius = circlePiece.radius * Mathf.Sqrt(Random.Range(0f, 1f));
            float randomAngle = Random.Range(0f, 1f) * 2 * Mathf.PI;

            float localX = circlePiece.piecePosition.x + randomRadius * Mathf.Cos(randomAngle);
            float localY = circlePiece.piecePosition.y;
            float localZ = circlePiece.piecePosition.z + randomRadius * Mathf.Sin(randomAngle);

            return transform.TransformPoint(localX, localY, localZ);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Handles.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Handles.matrix = transform.localToWorldMatrix;
        if (areaRectangles != null)
        {
            for (int i = 0; i < areaRectangles.Length; i++)
            {
                float length = areaRectangles[i].length;
                float width = areaRectangles[i].width;
                float y = areaRectangles[i].piecePosition.y;
                float angle = areaRectangles[i].angle * (Mathf.PI / 180);
                Vector3 rectangle3DPosition = areaRectangles[i].piecePosition;

                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                //Reminder of rotating points around an origin: 
                // x' = xcosθ - ysinθ
                // y' = ycosθ + xsinθ

                Vector3 point1 = Vector3.zero;

                Vector3 point2 = new Vector3(length * cos,
                0,
                length * sin);

                Vector3 point3 = new Vector3(length * cos - (width * sin),
                0,
                width * cos + (length * sin));

                Vector3 point4 = new Vector3(0 * cos - (width * sin),
                0,
                width * cos);

                point1 += rectangle3DPosition;

                point2 += rectangle3DPosition;

                point3 += rectangle3DPosition;

                point4 += rectangle3DPosition;
                Gizmos.DrawLine(point1, point2);
                Gizmos.DrawLine(point2, point3);
                Gizmos.DrawLine(point3, point4);
                Gizmos.DrawLine(point4, point1);

                Gizmos.color = Color.yellow;
                for (int j = 0; j < areaRectangles[i].entryPoints.Length; j++)
                {
                    Vector3 entryPointPosition = new Vector3(
                        areaRectangles[i].entryPoints[j].x * cos - areaRectangles[i].entryPoints[j].z * sin,
                        areaRectangles[i].entryPoints[j].y,
                        areaRectangles[i].entryPoints[j].z * cos + areaRectangles[i].entryPoints[j].x * sin);
                    entryPointPosition += rectangle3DPosition;
                    Gizmos.DrawLine(entryPointPosition, entryPointPosition + Vector3.up * 2);
                }
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

                if (debugRandomPosition)
                {
                    RectanglePiece rectanglePiece = areaRectangles[i];
                    float randomLocalCenteredX = Random.Range(0, rectanglePiece.length);
                    float localY = rectanglePiece.piecePosition.y;
                    float randomLocalCenteredZ = Random.Range(0, rectanglePiece.width);

                    //Here we rotate the point around the rectangle start and add the rectangles local position to match the point of the non-rotated rectangle to the rotated rectangle
                    float localX = (randomLocalCenteredX * Mathf.Cos(angle)) - (randomLocalCenteredZ * Mathf.Sin(angle)) + rectanglePiece.piecePosition.x;
                    float localZ = (randomLocalCenteredZ * Mathf.Cos(angle)) + (randomLocalCenteredX * Mathf.Sin(angle)) + rectanglePiece.piecePosition.z;

                    float r = Random.Range(0f, 1f);
                    float g = Random.Range(0f, 1f);
                    float b = Random.Range(0f, 1f);

                    Debug.DrawRay(transform.TransformPoint(localX, localY, localZ), Vector3.up, new Color(r, g, b, 1), 0.5f);
                }
            }
        }
        if (areaCircles != null)
        {
            for (int i = 0; i < areaCircles.Length; i++)
            {
                Handles.DrawWireArc(areaCircles[i].piecePosition,
                transform.up,
                transform.right,
                360,
                areaCircles[i].radius
                );

                Gizmos.color = Color.yellow;
                for (int j = 0; j < areaCircles[i].entryPoints.Length; j++)
                {
                    Vector3 entryPointPosition = areaCircles[i].entryPoints[j] + areaCircles[i].piecePosition;
                    Gizmos.DrawLine(entryPointPosition, entryPointPosition + Vector3.up * 2);
                }
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

                if (debugRandomPosition)
                {
                    CirclePiece circlePiece = areaCircles[i];
                    float randomRadius = circlePiece.radius * Mathf.Sqrt(Random.Range(0f, 1f));
                    float randomAngle = Random.Range(0f, 1f) * 2 * Mathf.PI;

                    float localX = circlePiece.piecePosition.x + randomRadius * Mathf.Cos(randomAngle);
                    float localY = circlePiece.piecePosition.y;
                    float localZ = circlePiece.piecePosition.z + randomRadius * Mathf.Sin(randomAngle);

                    float r = Random.Range(0f, 1f);
                    float g = Random.Range(0f, 1f);
                    float b = Random.Range(0f, 1f);

                    Debug.DrawRay(transform.TransformPoint(localX, localY, localZ), Vector3.up, new Color(r, g, b, 1), 0.5f);
                }
            }
        }
    }
#endif

    private void OnValidate()
    {
        if (areaRectangles != null)
        {
            for (int i = 0; i < areaRectangles.Length; i++)
            {
                if (areaRectangles[i].length < 0) areaRectangles[i].length = 0;
                if (areaRectangles[i].width < 0) areaRectangles[i].width = 0;
            }
        }
        if (areaCircles != null)
        {
            for (int i = 0; i < areaCircles.Length; i++)
            {
                if (areaCircles[i].radius < 0) areaCircles[i].radius = 0;
            }
        }
    }

    public float GetMapAreaSize()
    {
        float areaSize = 0;
        foreach (RectanglePiece rectanglePiece in areaRectangles)
        {
            areaSize += (rectanglePiece.length * transform.localScale.x) * (rectanglePiece.width * transform.localScale.z);
        }
        foreach (CirclePiece circlePiece in areaCircles)
        {
            areaSize += Mathf.PI * (circlePiece.radius * transform.localScale.x) * (circlePiece.radius * transform.localScale.z);
        }
        return areaSize;
    }

    [System.Serializable]
    internal class AreaPiece
    {
        public Vector3 piecePosition;
        public Vector3[] entryPoints;
    }
    [System.Serializable]
    internal class RectanglePiece : AreaPiece
    {
        public float length;
        public float width;
        public float angle;
    }
    [System.Serializable]
    internal class CirclePiece : AreaPiece
    {
        public float radius;
    }
    // [Serializable]
    // internal class TriangleRepresentation
    // {

    // }

    //Used to check if initial initialization is correct
    /*void Update()
    {
        for (int i = 0; i < allEntryPoints.Count; i++)
        {
            Debug.DrawLine(allEntryPoints[i], allEntryPoints[i] + Vector3.up * 0.5f, Color.green);
        }
    }*/
}