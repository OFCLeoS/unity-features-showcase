using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

/// <summary>
/// AI Module used to assess threats accordingly.
/// Threat Levels go from 0 to 1, where 0 is the minimal threat level and 1 the maximum
/// </summary>
public class AIThreatAssessorService : AIModule
{
    #region Blackboard References
    SerializableGUID seeingHostileBoolGUID;
    SerializableGUID hostileTransformGUID;
    #endregion

    BehaviorGraphAgent agentBehaviourTree;
    AISight sight;

    public bool SeeingHostile { get; private set; }

    Transform currentHostileInFocus;
    public Transform CurrentHostileInFocus => currentHostileInFocus;

    public Vector3 LastSeenHostilePosition { get; private set; }
    float hostileThreatLevel = 0f;

    // TODO: HashSet of Assessed threats?

    #region Initialization
    void Start()
    {
        IntializeThreatAssessor();
    }
    void IntializeThreatAssessor()
    {
        agentBehaviourTree = modulesDatabase.Brain.GetAgentBehaviourTree();
        sight = modulesDatabase.GetModule<AISight>();
        if (!agentBehaviourTree.GetVariableID("Seeing Hostile", out seeingHostileBoolGUID))
        {
            throw new BlackboardVariableNotFoundException("Seeing Hostile");
        }
        if (!agentBehaviourTree.GetVariableID("Hostile", out hostileTransformGUID))
        {
            throw new BlackboardVariableNotFoundException("Hostile");
        }
    }
    #endregion

    /// <summary>
    /// </summary>
    /// <returns>True if new Threat will be the current focus of the Agent</returns>
    public bool AssessThreat(Transform hostile, float hostileThreatLevel)
    {
        if (hostileThreatLevel > this.hostileThreatLevel)
        {
            SeeingHostile = true;
            this.hostileThreatLevel = hostileThreatLevel;
            currentHostileInFocus = hostile;
            LastSeenHostilePosition = currentHostileInFocus.position;
            //Debug.Log("New Threat in Focus");

            agentBehaviourTree.SetVariableValue(seeingHostileBoolGUID, true);
            agentBehaviourTree.SetVariableValue(hostileTransformGUID, currentHostileInFocus);
            return true;
        }
        else return false;
        // TODO: Keep threat in memory?
    }

    void HandleHostileLost()
    {
        SeeingHostile = false;
        currentHostileInFocus = null;
        hostileThreatLevel = 0;

        //Debug.Log("Threat in Focus Lost");
        agentBehaviourTree.SetVariableValue(seeingHostileBoolGUID, false);
        agentBehaviourTree.SetVariableValue<Transform>(hostileTransformGUID, null);
    }

    #region Performance
    const float MIN_CHECK_TIME = 0.3f;
    const float MAX_CHECK_TIME = 0.6f;
    float timeSinceLastCheck = 0;
    float nextCheckTime = 0;
    #endregion

    void Update()
    {
        if (SeeingHostile)
        {
            timeSinceLastCheck += Time.deltaTime;
            if (timeSinceLastCheck >= nextCheckTime)
            {
                if (!sight.IsSeeingTarget(currentHostileInFocus))
                {
                    HandleHostileLost();
                }
                else LastSeenHostilePosition = currentHostileInFocus.position;
                timeSinceLastCheck = 0;
                nextCheckTime = Random.Range(MIN_CHECK_TIME, MAX_CHECK_TIME); // TODO: Use faster library?
            }
        }
    }
}
