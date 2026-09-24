using UnityEngine;
using Unity.Behavior.GraphFramework;
using Unity.Behavior;


/// <summary>
/// Reload Handler specifically tailored to AI Agents. Must be initialized by an "AIFirearmsController" module.
/// </summary>
[RequireComponent(typeof(AIFirearmsController))]
public class AIFirearmReloadHandler : FirearmReloadHandler
{
    #region Blackboard References
    SerializableGUID hasAmmoBoolGUID;
    #endregion

    BehaviorGraphAgent agentBehaviourTree;

    // Should be Initialized by the AIFirearmsController Module!
    public void InitializeReloadHandler(SerializableGUID hasAmmoBoolGUID, BehaviorGraphAgent agentBehaviourTree)
    {
        this.hasAmmoBoolGUID = hasAmmoBoolGUID;
        this.agentBehaviourTree = agentBehaviourTree;
    }

    public override void StartReload(bool fastReload, FirearmFeeding firearmFeeding)
    {
        base.StartReload(fastReload, firearmFeeding);
        // TODO: PLACEHOLDER, MAKE COMPATIBLE WITH ANIMATION!
        // This is a placeholder, reloads will not always make it so the agent has ammo. The agent may not be able to reload.
        agentBehaviourTree.SetVariableValue(hasAmmoBoolGUID, true);
    }
}