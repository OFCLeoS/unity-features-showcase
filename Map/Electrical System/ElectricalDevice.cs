using UnityEngine;

/// <summary>
/// The base class for all electrical devices
/// </summary>
public abstract class ElectricalDevice : MonoBehaviour
{
    protected bool hasCurrent;
    protected bool deviceOn;

    [SerializeField] protected float powerConsumptionRate = 0;

    #region Initialization
    protected virtual void Awake()
    {
        hasCurrent = false;
        deviceOn = false;
        DeviceOff();
    }
    #endregion

    /// <summary>
    /// Adds current to the device and checks its status
    /// </summary>
    public virtual void AddCurrent()
    {
        hasCurrent = true;
        HandleDeviceStatus();
    }

    /// <summary>
    /// Removes current to the device and checks its status
    /// </summary>
    public void RemoveCurrent()
    {
        hasCurrent = false;
        HandleDeviceStatus();
    }

    /// <summary>
    /// Called when current is added or removed, decides whether or not the device will be turned on
    /// </summary>
    protected void HandleDeviceStatus()
    {
        if (CanTurnOn() && !deviceOn)
        {
            deviceOn = true;
            DeviceOn();
        }
        else if (!CanTurnOn() && deviceOn)
        {
            deviceOn = false;
            DeviceOff();
        }
    }

    /// <summary>
    /// Checks whether or not the device can be turned on
    /// </summary>
    /// <returns>True if the device can be turned on</returns>
    protected virtual bool CanTurnOn(){
        return hasCurrent;
    }

    /// <summary>
    /// The Device's behaviour once its turned on
    /// </summary>
    protected abstract void DeviceOn();

    /// <summary>
    /// The Device's behaviour once its turned off
    /// </summary>
    protected abstract void DeviceOff();

    public bool HasCurrent() => hasCurrent;
    public float GetPowerConsumptionRate() => powerConsumptionRate;
}
