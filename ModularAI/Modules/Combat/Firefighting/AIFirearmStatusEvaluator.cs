using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

/// <summary>
/// AI Module that allows an AI Agent to check the status of their firearm
/// </summary>
public class AIFirearmStatusEvaluator : AIModule
{
    #region Blackboard References
    SerializableGUID hasAmmoBoolGUID;
    #endregion

    AIFirearmsController firearmsController;

    BehaviorGraphAgent agentBehaviourTree;

    // Used to not be constantly modifying the blackboard variables
    bool hasAmmo;

    void Start()
    {
        agentBehaviourTree = modulesDatabase.Brain.GetAgentBehaviourTree();
        firearmsController = modulesDatabase.GetModule<AIFirearmsController>();
        if (!agentBehaviourTree.GetVariableID("Has Ammo", out hasAmmoBoolGUID))
        {
            throw new BlackboardVariableNotFoundException("Has Ammo");
        }
    }

    public void CheckFirearmStatus()
    {
        //firearmsController.EquippedFirearm;

        agentBehaviourTree.SetVariableValue(hasAmmoBoolGUID, true);
        //equippedFirearm.Feeding;
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
            CheckFirearmStatus();
            timeSinceLastCheck = 0;
            nextCheckTime = Random.Range(MIN_CHECK_TIME, MAX_CHECK_TIME); // TODO: Use faster library?
        }
    }
}
