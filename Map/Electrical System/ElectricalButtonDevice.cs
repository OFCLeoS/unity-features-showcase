using UnityEngine;

public abstract class ElectricalButtonDevice : ElectricalControlDevice
{
    protected abstract void PressButton();

    public override void OnInteract() => PressButton();
}
