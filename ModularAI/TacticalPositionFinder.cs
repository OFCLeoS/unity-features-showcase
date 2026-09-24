using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Actually in cover?. Better scoring system.
public class TacticalPositionFinder : MonoBehaviour
{
    [Tooltip("Size of the cells that will store each Tactical Position.")]
    [SerializeField] int cellSize = 5;
    [Tooltip("The penalty a Tactical Position score will suffer if it is behind the requestor.")]
    [SerializeField] int directionPenalty = -25;
    Dictionary<Vector2Int, List<TacticalPosition>> coverPositions;
    Dictionary<Vector2Int, List<TacticalPosition>> concealmentPositions;

    /// <summary>
    /// Used in some computations
    /// </summary>
    float halfCellSize;

    #region Initialization
    void Awake()
    {
        Initialize();
    }
    /// <summary>
    /// Gathers all cover and concealment positions. They will then be organized in cells, which can be accessed by the floored division of the position with the cell size
    /// </summary>
    void Initialize()
    {
        if (directionPenalty > 0) directionPenalty = -directionPenalty;
        if (cellSize <= 0) cellSize = 1;
        halfCellSize = cellSize / 2.0f;

        coverPositions = new Dictionary<Vector2Int, List<TacticalPosition>>();
        concealmentPositions = new Dictionary<Vector2Int, List<TacticalPosition>>();

        List<TacticalPosition> inspectedList;
        foreach (TacticalPosition tacticalPosition in FindObjectsByType<TacticalPosition>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            int x = Mathf.FloorToInt(tacticalPosition.transform.position.x / cellSize);
            int z = Mathf.FloorToInt(tacticalPosition.transform.position.z / cellSize);
            Vector2Int key = new Vector2Int(x, z);

            if (tacticalPosition.IsCover)
            {
                if (!coverPositions.TryGetValue(key, out inspectedList))
                {
                    inspectedList = new List<TacticalPosition>();
                    coverPositions.Add(key, inspectedList);
                }
                inspectedList.Add(tacticalPosition);
            }
            else
            {
                if (!concealmentPositions.TryGetValue(key, out inspectedList))
                {
                    inspectedList = new List<TacticalPosition>();
                    concealmentPositions.Add(key, inspectedList);
                }
                inspectedList.Add(tacticalPosition);
            }
        }
    }
    #endregion

    /// <summary>
    /// Finds the best cover from the reference position over the max distance.
    /// </summary>
    /// <param name="requestor">The cover requestor</param>
    /// <param name="hostilePosition">Position of the hostile</param>
    /// <param name="maxDistance">How roughly far from the request position will cover be searched</param>
    /// <param name="minimumScore">The minimum score the cover must have to be considered valid</param>
    /// <returns>Cover if found</returns>
    public TacticalPosition FindBestCover(Transform requestor, Vector3 hostilePosition, float maxDistance, float minimumScore)
    {
        if (maxDistance < 0) maxDistance = 0;
        return RingBestPositionsSearch(true, requestor, hostilePosition, maxDistance, minimumScore);
    }

    /// <summary>
    /// Finds the nearest concealment from the reference position over the max distance.
    /// </summary>
    /// <param name="requestor">The concealment requestor</param>
    /// <param name="hostilePosition">Position of the hostile</param>
    /// <param name="maxDistance">How roughly far from the request position will concealment be searched</param>
    /// /// <param name="minimumScore">The minimum score the concealment must have to be considered valid</param>
    /// <returns>Concealment if found</returns>
    public TacticalPosition FindBestConcealment(Transform requestor, Vector3 hostilePosition, float maxDistance, float minimumScore)
    {
        if (maxDistance < 0) maxDistance = 0;
        return RingBestPositionsSearch(false, requestor, hostilePosition, maxDistance, minimumScore);
    }

    /// <summary>
    /// Finds the nearest tactical position from the reference point over the max distance.
    /// </summary>
    /// <param name="requestPosition">From where tactical position will be searched</param>
    /// <param name="maxDistance">How roughly far from the request position will tactical position be searched</param>
    /// <returns>Tactical Position if found</returns>
    public TacticalPosition FindNearestTacticalPosition(Transform requestor, int maxDistance)
    {
        if (maxDistance < 0) maxDistance = 0;

        return null;
    }

    // TODO: Ring Search can be generic, lots of repetition
    /// <summary>
    /// Searches for the best tactical position by drawing rectangular rings around a point which represents the starting position, checking each ring for a valid position.
    /// </summary>
    TacticalPosition RingBestPositionsSearch(bool coverSearch, Transform requestor, Vector3 hostilePosition, float maxDistance, float minimumScore)
    {

        Vector3 requestPosition = requestor.transform.position;
        // Actual max distance is the span of the cells will be searched for a tactical position 
        int actualMaxDistance = Mathf.CeilToInt(maxDistance / cellSize);
        // Convert starting position to grid position
        int x = Mathf.FloorToInt(requestPosition.x / cellSize);
        int z = Mathf.FloorToInt(requestPosition.z / cellSize);
        Vector2Int startingPoint = new Vector2Int(x, z);

        TacticalPosition bestPosition = null;

        List<TacticalPosition> inspectedList;

        Dictionary<Vector2Int, List<TacticalPosition>> tacticalPositions = coverSearch ? coverPositions : concealmentPositions;
        float bestScore = -Mathf.Infinity;

        Vector2Int cellToInspect = startingPoint;
        // Each iteration is 1 ring around the starting point
        for (int d = 0; d <= actualMaxDistance; d++)
        {
            if (d == 0 && tacticalPositions.TryGetValue(cellToInspect, out inspectedList))
            {
                for (int i = 0; i < inspectedList.Count; i++)
                {
                    float positionScore = GetTacticalPositionScore(inspectedList[i], requestor, hostilePosition);
                    if (positionScore < minimumScore) continue;
                    if (positionScore > bestScore)
                    {
                        bestScore = positionScore;
                        bestPosition = inspectedList[i];
                    }
                }
            }
            else
            {
                // Top and Bottom sides
                for (int dx = -d; dx <= d; dx++)
                {
                    cellToInspect = startingPoint + new Vector2Int(dx, d);
                    // Top Side
                    if (tacticalPositions.TryGetValue(cellToInspect, out inspectedList))
                    {
                        for (int i = 0; i < inspectedList.Count; i++)
                        {
                            float positionScore = GetTacticalPositionScore(inspectedList[i], requestor, hostilePosition);
                            if (positionScore < minimumScore) continue;
                            if (positionScore > bestScore)
                            {
                                bestScore = positionScore;
                                bestPosition = inspectedList[i];
                            }
                        }
                    }
                    cellToInspect = startingPoint + new Vector2Int(dx, -d);
                    // Bottom Side
                    if (tacticalPositions.TryGetValue(cellToInspect, out inspectedList))
                    {
                        for (int i = 0; i < inspectedList.Count; i++)
                        {
                            float positionScore = GetTacticalPositionScore(inspectedList[i], requestor, hostilePosition);
                            if (positionScore < minimumScore) continue;
                            if (positionScore > bestScore)
                            {
                                bestScore = positionScore;
                                bestPosition = inspectedList[i];
                            }
                        }
                    }
                }

                // Left and Right sides (excluding corners)
                for (int dz = -d + 1; dz <= d - 1; dz++)
                {
                    cellToInspect = startingPoint + new Vector2Int(d, dz);
                    // Right Side
                    if (tacticalPositions.TryGetValue(cellToInspect, out inspectedList))
                    {
                        for (int i = 0; i < inspectedList.Count; i++)
                        {
                            float positionScore = GetTacticalPositionScore(inspectedList[i], requestor, hostilePosition);
                            if (positionScore < minimumScore) continue;
                            if (positionScore > bestScore)
                            {
                                bestScore = positionScore;
                                bestPosition = inspectedList[i];
                            }
                        }
                    }
                    cellToInspect = startingPoint + new Vector2Int(-d, dz);
                    // Left Side
                    if (tacticalPositions.TryGetValue(cellToInspect, out inspectedList))
                    {
                        for (int i = 0; i < inspectedList.Count; i++)
                        {
                            float positionScore = GetTacticalPositionScore(inspectedList[i], requestor, hostilePosition);
                            if (positionScore < minimumScore) continue;
                            if (positionScore > bestScore)
                            {
                                bestScore = positionScore;
                                bestPosition = inspectedList[i];
                            }
                        }
                    }
                }
            }

            if (bestPosition != null && !CanPotentiallyFindBetterPosition(d, bestScore))
            {
                #region DEBUGGING
#if UNITY_EDITOR
                if (DEBUG_NEXT) DEBUG_SetPicked();
#endif
                #endregion
                return bestPosition;
            }
        }
        if (bestPosition != null)
        {
            #region DEBUGGING
#if UNITY_EDITOR
            if (DEBUG_NEXT) DEBUG_SetPicked();
#endif
            #endregion
            return bestPosition;
        }
        return null;
    }

    /// <summary>
    /// Tactical Position evaluation.
    /// </summary>
    /// <returns>The score of a tactical position. The high number, the better score</returns>
    float GetTacticalPositionScore(TacticalPosition tacticalPosition, Transform requestor, Vector3 hostilePosition)
    {
        float score = 0;

        Vector3 requestPosition = requestor.transform.position;

        // Cover Direction in relation to Hostile: Immidate disqualification if tac pos can be seen by hostile
        float coverHostileDotProduct = Vector3.Dot(tacticalPosition.transform.forward, tacticalPosition.transform.position - hostilePosition);
        if (coverHostileDotProduct >= 0)
        {
            #region DEBUGGING
#if UNITY_EDITOR
            if (DEBUG_NEXT)
            {
                DEBUG_CreateDebugObject(new DEBUG_INFORMATION(true, tacticalPosition, 0, 0, -Mathf.Infinity));
            }
#endif
            #endregion
            return -Mathf.Infinity;
        }

        // Distance score (more distance, more penalties)
        float sqrLen = (tacticalPosition.transform.position - requestPosition).sqrMagnitude;
        float distanceScore = -sqrLen;
        score += distanceScore;


        // TODO?: Requestor Direction in relation to the tac pos score (The more the tac pos is behind the requestor, the more penalties)

        // Tac Pos Behind Penalty
        float tacPosDirectionPenalty = 0;
        Vector3 requestorDirectionToTacPos = tacticalPosition.transform.position - requestor.position;
        float requestorCoverDotProduct = Vector2.Dot(requestor.forward, requestorDirectionToTacPos);
        if (requestorCoverDotProduct < 0) tacPosDirectionPenalty = directionPenalty;
        score += tacPosDirectionPenalty;
        // TODO: Hostile Direction in relation to where requestor will move to?

        #region DEBUGGING
#if UNITY_EDITOR
        if (DEBUG_NEXT)
        {
            DEBUG_CreateDebugObject(new DEBUG_INFORMATION(false, tacticalPosition, distanceScore, tacPosDirectionPenalty, score));
        }
#endif
        #endregion

        return score;
    }

    /// <summary>
    /// Returns in how many rings 
    /// </summary>
    /// <returns></returns>
    bool CanPotentiallyFindBetterPosition(int currentRing, float ringBestScore)
    {
        float potentialScore = 0;

        // Minus Cell Size for best case scenario
        float sqrTheoredicalLength = (cellSize * (currentRing + 1)) - cellSize;
        sqrTheoredicalLength *= sqrTheoredicalLength;

        potentialScore -= sqrTheoredicalLength;
        //Debug.Log("In Ring " + currentRing + ", it is " + (potentialScore > ringBestScore ? "" : "not ") + "possible that, in Ring " + (currentRing + 1) + ", the Potential Score of " + potentialScore + " beats the Current Score of " + ringBestScore);
        return potentialScore > ringBestScore;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region DEBUGGING
#if UNITY_EDITOR
    [Header("DEBUGGING")]
    bool DEBUG_NEXT;

    [SerializeField] GameObject _DEBUG_TAC_POS_OBJECT;
    [SerializeField] float DEBUG_LIFETIME;

    List<DEBUG_TacticalPositionDebugObject> DEBUG_OBJECTS = new List<DEBUG_TacticalPositionDebugObject>();
    int DEBUG_BEST_TAC_POS_INDEX = 0;
    float DEBUG_BEST_TAC_POS_SCORE = -Mathf.Infinity;

    private struct DEBUG_INFORMATION
    {
        public TacticalPosition tacPos;

        public bool facingHostile;

        public float distanceScore;
        public float directionPenalty;
        public float totalScore;

        public DEBUG_INFORMATION(bool facingHostile, TacticalPosition tacPos, float distanceScore, float directionPenalty, float totalScore)
        {
            this.facingHostile = facingHostile;
            this.tacPos = tacPos;

            this.distanceScore = distanceScore;
            this.directionPenalty = directionPenalty;
            this.totalScore = totalScore;
        }
    }
    /// <summary>
    /// DEBUGGING ONLY!
    /// </summary>
    public TacticalPosition DEBUG_FindBestCover(Transform requestor, Vector3 hostilePosition, float maxDistance, float minimumScore)
    {
        if (DEBUG_OBJECTS.Count > 0)
        {
            DEBUG_BEST_TAC_POS_INDEX = 0;
            DEBUG_BEST_TAC_POS_SCORE = -Mathf.Infinity;
            foreach (DEBUG_TacticalPositionDebugObject debugObject in DEBUG_OBJECTS)
            {
                Destroy(debugObject.gameObject);
            }
        }
        if (maxDistance < 0) maxDistance = 0;
        DEBUG_NEXT = true;
        TacticalPosition tacPos = RingBestPositionsSearch(true, requestor, hostilePosition, maxDistance, minimumScore);
        DEBUG_NEXT = false;
        return tacPos;
    }

    /// <summary>
    /// DEBUGGING ONLY!
    /// </summary>
    public TacticalPosition DEBUG_FindBestConcealment(Transform requestor, Vector3 hostilePosition, float maxDistance, float minimumScore)
    {
        if (DEBUG_OBJECTS.Count > 0)
        {
            DEBUG_BEST_TAC_POS_INDEX = 0;
            DEBUG_BEST_TAC_POS_SCORE = -Mathf.Infinity;
            foreach (DEBUG_TacticalPositionDebugObject debugObject in DEBUG_OBJECTS)
            {
                Destroy(debugObject.gameObject);
            }
        }
        if (maxDistance < 0) maxDistance = 0;
        DEBUG_NEXT = true;
        TacticalPosition tacPos = RingBestPositionsSearch(false, requestor, hostilePosition, maxDistance, minimumScore);
        DEBUG_NEXT = false;
        return tacPos;
    }

    void DEBUG_CreateDebugObject(DEBUG_INFORMATION debugInfo)
    {
        TacticalPosition tacPos = debugInfo.tacPos;
        DEBUG_TacticalPositionDebugObject debugObject = Instantiate(_DEBUG_TAC_POS_OBJECT, tacPos.transform.position, tacPos.transform.rotation).GetComponent<DEBUG_TacticalPositionDebugObject>();

        if (tacPos.IsCover) debugObject.InitializeCover(DEBUG_LIFETIME, DEBUG_GetTacPosDescription(debugInfo));
        else debugObject.InitializeConcealment(DEBUG_LIFETIME, DEBUG_GetTacPosDescription(debugInfo));

        if (debugInfo.totalScore > DEBUG_BEST_TAC_POS_SCORE)
        {
            DEBUG_BEST_TAC_POS_INDEX = DEBUG_OBJECTS.Count;
            DEBUG_BEST_TAC_POS_SCORE = debugInfo.totalScore;
        }
        DEBUG_OBJECTS.Add(debugObject);
    }

    void DEBUG_SetPicked() => DEBUG_OBJECTS[DEBUG_BEST_TAC_POS_INDEX].SetAsPicked();

    string[] DEBUG_GetTacPosDescription(DEBUG_INFORMATION debugInfo)
    {
        if (!debugInfo.facingHostile)
        {
            string[] description =
            {
                "Total Score: " +debugInfo.totalScore,
                "Distance Score: " +debugInfo.distanceScore,
                "Direction Penalty: " +debugInfo.directionPenalty
            };
            return description;
        }
        else
        {
            string[] description = { "! COVER IS VISBILE BY HOSTILE !" };
            return description;
        }

    }

    [ContextMenu("DISPLAY_GRID")]
    void DEBUG_DisplayGrid()
    {

    }
#endif
    #endregion
}
