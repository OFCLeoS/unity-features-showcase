using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Fail", category: "Flow", id: "3aca2a89237344e7d1468f0527771ac9")]
public partial class FailModifier : Modifier
{

    protected override Status OnStart() => Status.Failure;

    protected override Status OnUpdate() => Status.Failure;

    protected override void OnEnd() { }
}

