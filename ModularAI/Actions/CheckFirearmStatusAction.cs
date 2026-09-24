using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Firearm Status", story: "[Agent] checks the status of their equipped firearm", category: "Action/Military", id: "f61aaa2f2ec2471d8719776986fee61f")]
public partial class CheckFirearmStatusAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

