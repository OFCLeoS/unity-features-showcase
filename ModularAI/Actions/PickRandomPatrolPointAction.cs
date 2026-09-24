using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pick Random Patrol Point", story: "[Agent] picks a random [PatrolPoint]", category: "Action", id: "0d5f3799e22a5927a537af9281946b72")]
public partial class PickRandomPatrolPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> PatrolPoint;

    AIPatrolPositionEvaluator _patrolPositionEvaluator;

    AIPatrolPositionEvaluator PatrolPositionEvaluator
    {
        get
        {
            if (_patrolPositionEvaluator == null) _patrolPositionEvaluator = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AIPatrolPositionEvaluator>();
            return _patrolPositionEvaluator;
        }
    }
    protected override Status OnStart()
    {
        PatrolPoint.Value = PatrolPositionEvaluator.GetRandomPatrolPosition();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

