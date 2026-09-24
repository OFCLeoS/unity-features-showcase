using Unity.Behavior;
using UnityEngine;
/// <summary>
/// The AI Brain is used by AI agents to capture stimuli and react to it acordingly
/// </summary>
[RequireComponent(typeof(AIModulesDatabase))]
public abstract class AIBrain : AIModule
{
    protected BehaviorGraphAgent agentBehaviourTree;

    public BehaviorGraphAgent GetAgentBehaviourTree() => agentBehaviourTree;

    void Awake() => agentBehaviourTree = GetComponent<BehaviorGraphAgent>();
}