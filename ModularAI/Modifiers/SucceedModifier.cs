using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Succeed", category: "Flow", id: "b6409a026aadb6346037da85477ecebf")]
public partial class SucceedModifier : Modifier
{

    protected override Status OnStart() => Status.Success;

    protected override Status OnUpdate() => Status.Success;

    protected override void OnEnd() { }
}

