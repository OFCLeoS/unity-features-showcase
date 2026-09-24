using UnityEngine;

/// <summary>
/// AI Behaviour Module for the adjustment of positioning to achieve an advantage over the enemy. Used for Agents that are able to engage in Firefights.
/// </summary>
public class AITacticalPositioningBehaviour : AIModule
{
    const float PEEK_OFFSET = 0.2f;

    AIMovementController movementController;
    AIRigController rigController;
    AIStanceController stanceController;
    AITacticalPositionEvaluator tacticalPositionEvaluator;
    AIThreatAssessorService threatAssessor;
    AIAimBehaviour aimBehaviour;

    TacticalPosition currentPosition;
    // Used to check if the Agent is in the Tactical Position
    bool inPosition = false;

    [Tooltip("The offset to the side that the Agent will have from a right-sided tactical position. (Used for proper position when peeking)")]
    [SerializeField] float rightPeekPositionOffset = 0.39f;
    [Tooltip("The offset to the side that the Agent will have from a left-sided tactical position. (Used for proper position when peeking)")]
    [SerializeField] float leftPeekPositionOffset = 0.39f;

    #region Initialization
    void Start()
    {
        movementController = modulesDatabase.GetModule<AIMovementController>();
        rigController = modulesDatabase.GetModule<AIRigController>();
        stanceController = modulesDatabase.GetModule<AIStanceController>();
        tacticalPositionEvaluator = modulesDatabase.GetModule<AITacticalPositionEvaluator>();
        threatAssessor = modulesDatabase.GetModule<AIThreatAssessorService>();
        aimBehaviour = modulesDatabase.GetModule<AIAimBehaviour>();
        enabled = false;
    }
    #endregion

    /// <summary>
    /// Agent will start performing adjustments to their position according to the Hostile Position.
    /// </summary>
    /// <returns>False if no hostile exists</returns>
    public bool InitiateTacticalPositioning()
    {
        // USE IF VECTOR3 NEEDS TO BE NULLABLE Vector3? lastSeenHostilePositionNullable = threatAssessor.LastSeenHostilePosition;
        //if (!lastSeenHostilePositionNullable.HasValue) return false;

        Vector3 lastSeenHostilePosition = threatAssessor.LastSeenHostilePosition;

        TacticalPosition tacticalPosition = tacticalPositionEvaluator.FindBestCover(lastSeenHostilePosition);
        if (tacticalPosition == null)
        {
            tacticalPosition = tacticalPositionEvaluator.FindBestConcealment(lastSeenHostilePosition);
        }
        if (tacticalPosition != null)
        {
            currentPosition = tacticalPosition;
            // Offset so that the character can peek properly, i.e. can see what is on the other side of the cover/concealment
            Vector3 peekOffset = currentPosition.IsRightPeek ? transform.right * rightPeekPositionOffset : -transform.right * leftPeekPositionOffset;
            movementController.MoveToPosition(currentPosition.transform.position + peekOffset);
            inPosition = false;
        }
        else
        {
            // TODO: MAKE AGENT GO PRONE
            inPosition = true;
            currentPosition = null;
        }
        enabled = true;
        return true;
    }

    /// <summary>
    /// Agent will stop performing adjustments to their position
    /// </summary>
    public void StopTacticalPositioning()
    {
        currentPosition = null;
        stanceController.SetLeanCoefficient(0);
        enabled = false;
        Debug.Log("Bye");
    }

    // When in a Tactical Position, the Agent will attempt to expose themselves as little as possible
    // TODO: ADD GOING BACK TO COVER AFTER POPPING A FEW SHOTS
    void HandleTacticalPositioning()
    {
        if (inPosition)
        {
            // USE IF VECTOR3 NEEDS TO BE NULLABLE Vector3? lastSeenHostilePositionNullable = threatAssessor.LastSeenHostilePosition;
            //if (!lastSeenHostilePositionNullable.HasValue) return false;

            Vector3 lastSeenHostilePosition = threatAssessor.LastSeenHostilePosition;
            float peekDirectionCoefficient = 0;
            // TODO: Gradual Leaning?
            if (currentPosition.IsRightPeek) // Right-Hand peeking
            {
                stanceController.SetLeanCoefficient(1);
                peekDirectionCoefficient = 1;
            }
            else // Left-Hand peeking
            {
                stanceController.SetLeanCoefficient(-1);
                peekDirectionCoefficient = -1;
            }

            // If the Agent is not seeing the Hostile, it will aim at the corner of the cover, simulating peeking
            if (!threatAssessor.SeeingHostile)
            {
                // TODO: SET POSITIONCORNER TO BE THIS WHOLE THING INSTEAD OF ALWAYS CALCULATING?
                // TODO: SET 1.3f TO HEIGHT OR SMT!
                Vector3 aimTarget = currentPosition.CornerPosition + (currentPosition.transform.right * peekDirectionCoefficient * PEEK_OFFSET) + currentPosition.transform.forward + transform.up * 1.3f;
                aimBehaviour.SetAimTarget(aimTarget);
                // .......
                // If the Agent is not at the tactical position whilst not seeing the hostile, it will go there
                // movementController.MoveToPosition(currentPosition.transform.position,0.01f);
            }
            #region TODO POSITION READJUSTMENT
            // If the Agent is seeing the Hostile, it will constantly readjust its position
            // ??? Use Dot Product between pos.right and direction of pos to collider center ??? (Is it here already?)
            // else
            // {
            //     Transform currentPositionTransform = currentPosition.transform;
            //     Vector3 directionToHostile = lastSeenHostilePosition - currentPositionTransform.position;
            //     float dotProduct = Vector3.Dot(currentPositionTransform.right, directionToHostile);
            //     // TODO: Unperformant! Check Min distance!!! USE MOVEMENT CONTROLLER
            //     movementController.MoveToPosition(currentPositionTransform.position - (currentPositionTransform.right * peekDirectionCoefficient) * dotProduct*0.15f,0.01f);
            // }
            #endregion
        }
        else if (movementController.HasReachedDestination())
        {
            transform.forward = currentPosition.transform.forward;
            inPosition = true;
        }
    }

    #region Performance
    const float MIN_CHECK_TIME = 0.3f;
    const float MAX_CHECK_TIME = 0.6f;
    float timeSinceLastCheck = 0;
    float nextCheckTime = 0;
    #endregion

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= nextCheckTime)
        {
            HandleTacticalPositioning();
            timeSinceLastCheck = 0;
            nextCheckTime = Random.Range(MIN_CHECK_TIME, MAX_CHECK_TIME); // TODO: Use faster library?
        }
    }
}
