using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Location", story: "[Agent] moves to [Location]", category: "Action", id: "97bea0779080ce5efca53984a3fca75b")]
public partial class MoveToLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Location;

    AIMovementController _movementController;

    AIMovementController MovementController
    {
        get
        {
            if (_movementController == null) _movementController = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AIMovementController>();
            return _movementController;
        }
    }

    protected override Status OnStart()
    {
        MovementController.MoveToPosition(Location.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // TODO: More safeguards?
        if (!MovementController.HasReachedDestination())
        {
            return Status.Running;
        }
        else return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

