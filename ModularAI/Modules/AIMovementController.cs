using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI Module responsible for the moving of an Agent. It is responsible for lower body rotation when moving, movement speed, etc...
/// </summary>
public class AIMovementController : AIModule
{
    const float DEFAULT_STOPPING_DISTANCE = 0.39f;

    NavMeshAgent navMeshAgent;

    float stoppingDistance = DEFAULT_STOPPING_DISTANCE;
    float squaredStoppingDistance = DEFAULT_STOPPING_DISTANCE * DEFAULT_STOPPING_DISTANCE; // For Performance Reasons

    Vector3 targetPosition = Vector3.positiveInfinity;

    #region Initialization
    void Awake() => InitializeMovementController();

    void InitializeMovementController()
    {
        navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }
    #endregion

    /// <summary>
    /// Sets the Agent's destination to the position. Uses the default stopping distance.
    /// </summary>
    public void MoveToPosition(Vector3 position) => MoveToPosition(position, DEFAULT_STOPPING_DISTANCE);

    /// <summary>
    /// Sets the Agent's destination to the position.
    /// </summary>
    public void MoveToPosition(Vector3 position, float stoppingDistance)
    {
        if(Equals(position,targetPosition)) return;
        
        this.stoppingDistance = stoppingDistance;
        squaredStoppingDistance = stoppingDistance * stoppingDistance;
        navMeshAgent.stoppingDistance = stoppingDistance;

        targetPosition = position;

        float sqrMagnitude = Vector3.SqrMagnitude(new Vector3(transform.position.x, targetPosition.y, transform.position.z) - targetPosition);

        // We only set a new destination if its distance is less than or equals to the stopping distance to avoid pointless/heavy calculations
        if (sqrMagnitude > squaredStoppingDistance) navMeshAgent.SetDestination(position);
    }

    public bool HasReachedDestination()
    {
        float sqrMagnitude = Vector3.SqrMagnitude(new Vector3(transform.position.x, targetPosition.y, transform.position.z) - targetPosition);
        bool destinationReached = sqrMagnitude <= squaredStoppingDistance;

        // Debug.Log("Stopping: " + stoppingDistance);
        // Debug.Log("Distance: " + sqrMagnitude);
        if (destinationReached && !navMeshAgent.pathPending) return true;
        else return false;
    }

    void Update()
    {

    }
}
