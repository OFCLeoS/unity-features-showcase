using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Repeat X Times", story: "Repeat [X] Times", category: "Flow/Repeats", id: "d8d50ae52e77e710dfaec2c31ddfccea")]
public partial class RepeatXTimesModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<int> X;

    int count;

    protected override Status OnStart()
    {
        if (Child == null) return Status.Failure;
        count = 0;
        Status childStatus = StartNode(Child);
        if (childStatus == Status.Failure || childStatus == Status.Success)
        {
            return Status.Running;
        }
        return Status.Waiting;
    }

    protected override Status OnUpdate()
    {
        Status childStatus = Child.CurrentStatus;
        if (childStatus == Status.Failure || childStatus == Status.Success)
        {
            count++;
            if (count >= X.Value)
            {
                count = 0;
                return childStatus;
            }
            childStatus = StartNode(Child);
            if (childStatus == Status.Failure || childStatus == Status.Success)
            {
                return Status.Running;
            }
        }
        return Status.Waiting;
    }

    protected override void OnEnd()
    {
    }
}

