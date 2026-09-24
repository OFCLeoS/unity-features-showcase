using UnityEngine;

/// <summary>
/// Base class for all consumer electrical devices (such as computers or lights)
/// </summary>
public abstract class ElectricalLoadDevice : ElectricalDevice
{
    protected ElectricalControlDevice controlDevice;

    #region Initialization
    protected virtual void Start()
    {
        if (controlDevice is null)
        {
            Debug.LogError(transform.name + " does not have a control device assigned to it. Controlling the device will not be possible.");
        }
    }

    /// <summary>
    /// Sets the control device for this electrical load device. Should be set on Awake().
    /// </summary>
    /// <param name="controlDevice">The new control device for this electrical device</param>
    public void SetControlDevice(ElectricalControlDevice controlDevice) => this.controlDevice = controlDevice;
    #endregion

}
