using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Adjust Tactical Position", story: "[Agent] adjusts their tactical position", category: "Action/Military", id: "3b453297f7e4fb69d794097e2ac9011e")]
public partial class AdjustTacticalPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    AITacticalPositioningBehaviour _tacticalPositioning;

    AITacticalPositioningBehaviour TacticalPositioning
    {
        get
        {
            if (_tacticalPositioning == null) _tacticalPositioning = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AITacticalPositioningBehaviour>();
            return _tacticalPositioning;
        }
    }

    protected override Status OnStart()
    {
        TacticalPositioning.InitiateTacticalPositioning();
        return Status.Running;
    }

    protected override Status OnUpdate() => Status.Running;

    protected override void OnEnd()
    {
        TacticalPositioning.StopTacticalPositioning();
    }
}

