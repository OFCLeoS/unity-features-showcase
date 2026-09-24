using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Hit the Ground", story: "[Agent] goes prone", category: "Action", id: "2c2dc9cee9139dc6c55e8125aeed754f")]
public partial class HitTheGroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        //TODO: ADD ACTUAL LOGIC
        Debug.Log(Agent.Value.name+" hits the ground.");
        return Status.Success;
    }

    //This is unused
    protected override Status OnUpdate() => Status.Failure;

    protected override void OnEnd() { }
}

