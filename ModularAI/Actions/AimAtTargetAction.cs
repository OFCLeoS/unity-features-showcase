using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Aim At Target", story: "[Agent] aims at [Target]", category: "Action/Military", id: "5ab84dc13b5368995fce9049c165e419")]
public partial class AimAtTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    // TODO: Could become an issue if fine control is wanted
    AIAimBehaviour _aimController;

    AIAimBehaviour AimController
    {
        get
        {
            if (_aimController == null) _aimController = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AIAimBehaviour>();
            return _aimController;
        }
    }

    protected override Status OnStart() => Status.Running;

    protected override Status OnUpdate()
    {
        AimController.SetAimTarget(Target.Value.position);
        // TODO: Always return Success?
        return Status.Running;
    }

    protected override void OnEnd()
    {
        AimController.ResetAimTarget();
    }
}

