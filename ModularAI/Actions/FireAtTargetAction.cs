using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Fire at Target", story: "[Agent] fires at [Target]", category: "Action/Military", id: "7af395cecf5d31116772ba1bbe3f22bd")]
public partial class FireAtHostileAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    AIFireAtTargetBehaviour _fireController;

    AIFireAtTargetBehaviour FireController
    {
        get
        {
            if (_fireController == null) _fireController = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AIFireAtTargetBehaviour>();
            return _fireController;
        }
    }

    protected override Status OnStart()
    {
        FireController.StartFiring(Target.Value);
        return Status.Running;
    }

    protected override Status OnUpdate() => Status.Running;

    protected override void OnEnd() => FireController.StopFiring();
}

