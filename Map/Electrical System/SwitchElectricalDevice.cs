using UnityEngine;

public class SwitchElectricalDevice : ElectricalControlDevice
{
    bool isOn = false;

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        isOn = false;
    }
    #endregion

    protected override bool CanTurnOn(){
        return hasCurrent && isOn;
    }

    public override void OnInteract()
    {
        isOn = !isOn;
        HandleDeviceStatus();
    }
}
