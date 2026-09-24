using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reload Firearm", story: "[Agent] reloads their equipped firearm", category: "Action/Military", id: "0f0e9c5a3125a62ad1b964442d164c9f")]
public partial class ReloadFirearmAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    AIFirearmsController _firearmsController;

    AIFirearmsController FirearmsController
    {
        get
        {
            if (_firearmsController == null) _firearmsController = Agent.Value.GetComponent<AIModulesDatabase>().GetModule<AIFirearmsController>();
            return _firearmsController;
        }
    }

    // TODO: RELOAD STAGES IMPLEMENTATION!!!
    protected override Status OnStart()
    {
        FirearmsController.StartReload();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        // Should be aborted by HasAmmo being true, change this?
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

