using UnityEngine;

/// <summary>
/// Allows an AI Agent to find a tactical position that meets their current needs
/// </summary>
public class AITacticalPositionEvaluator : AIModule
{
    //TODO: USE AI PROFILES
    [SerializeField] float maxDistance;
    [SerializeField] float minimumCoverScore;
    [SerializeField] float minimumConcealmentScore;
    //TODO: To get automatically
    [SerializeField] TacticalPositionFinder tacticalPositionFinder;
    TacticalPosition currentTacticalPosition;

    /// <summary>
    /// Finds the best cover from the hostile position over the max distance.
    /// </summary>
    /// <param name="hostilePosition">Position of the hostile</param>
    /// <param name="maxDistance">How roughly far from the request position will cover be searched</param>
    /// <param name="minimumScore">The minimum score the cover must have to be considered valid</param>
    /// <returns>Cover if found</returns>
    public TacticalPosition FindBestCover(Vector3 hostilePosition)
    {
        #region DEBUGGING
#if UNITY_EDITOR
        if (DEBUG_SHOW_THOUGHT_PROCESS) return tacticalPositionFinder.DEBUG_FindBestCover(transform, hostilePosition, maxDistance, minimumCoverScore);
#endif
        #endregion
        return tacticalPositionFinder.FindBestCover(transform, hostilePosition, maxDistance, minimumCoverScore);
    }

    /// <summary>
    /// Finds the nearest concealment from the hostile position over the max distance.
    /// </summary>
    /// <param name="hostilePosition">Position of the hostile</param>
    /// <param name="maxDistance">How roughly far from the request position will concealment be searched</param>
    /// /// <param name="minimumScore">The minimum score the concealment must have to be considered valid</param>
    /// <returns>Concealment if found</returns>
    public TacticalPosition FindBestConcealment(Vector3 hostilePosition)
    {
        #region DEBUGGING
#if UNITY_EDITOR
        if (DEBUG_SHOW_THOUGHT_PROCESS) return tacticalPositionFinder.DEBUG_FindBestConcealment(transform, hostilePosition, maxDistance, minimumConcealmentScore);
#endif
        #endregion
        return tacticalPositionFinder.FindBestConcealment(transform, hostilePosition, maxDistance, minimumConcealmentScore);
    }

    #region Debugging
#if UNITY_EDITOR
    [Tooltip("Shows this Agent's thought process when finding cover.")]
    [SerializeField] bool DEBUG_SHOW_THOUGHT_PROCESS = false;
#endif
    #endregion
}
