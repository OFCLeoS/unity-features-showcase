using Unity.Behavior.GraphFramework;
using UnityEngine;

/// <summary>
/// Controls how a Guard Agent will respond to different stimuli
/// </summary>
public class GuardBrain : AIBrain, IVisualReceptor
{
    #region Blackboard References
    SerializableGUID guardTaskGUID;
    #endregion

    GuardTask currentTask;
    AIThreatAssessorService threatAssessor;

    #region Initialization
    protected void Start()
    {
        threatAssessor = modulesDatabase.GetModule<AIThreatAssessorService>();
        InitializeGuard();
    }
    void InitializeGuard()
    {
        currentTask = GuardTask.Patrol;
        if (!agentBehaviourTree.GetVariableID("Guard Task", out guardTaskGUID))
        {
            throw new BlackboardVariableNotFoundException("Seeing Hostile");
        }
        agentBehaviourTree.SetVariableValue(guardTaskGUID, currentTask);
    }
    #endregion

    public void TransmitVisualTarget(Collider visualTarget)
    {
        string targetTag = visualTarget.tag;
        switch (targetTag)
        {
            case "Weapon":
                {

                }
                break;

            case "Prisoner":
                {
                    Debug.Log(name + " SPOTTED THE " + visualTarget.transform.root.name + " PRISIONER");
                    if (threatAssessor.AssessThreat(visualTarget.transform.root, 0.5f))
                    {
                        currentTask = GuardTask.Combat;
                        agentBehaviourTree.SetVariableValue(guardTaskGUID, currentTask);
                    }
                }
                break;

            default:
                {

                }
                break;

        }

    }

    void Update()
    {

    }
}
