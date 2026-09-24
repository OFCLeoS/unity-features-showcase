using UnityEngine;

/// <summary>
/// The base class for all devices which control one or several electrical devices
/// </summary>
public abstract class ElectricalControlDevice : ElectricalDevice, IInteractable
{
    [SerializeField] protected ElectricalBreaker assignedBreaker;
    [SerializeField] protected ElectricalLoadDevice[] connectedElectricalDevices;

    #region Initialization
    protected override void Awake()
    {
        InitializeControlDevice();
        base.Awake();
    }

    /// <summary>
    /// Basic starting checks and behaviour
    /// </summary>
    void InitializeControlDevice()
    {
        if (connectedElectricalDevices.Length > 0)
        {
            CalculatePowerConsumptionRate();
            if (assignedBreaker is null)
            {
                Debug.LogError(transform.name + " does not have a breaker assigned to it. Controlling the device will not be possible.");
                hasCurrent = false;
            }
            else
            {
                assignedBreaker.AddElectricalControlDevice(this);
                AssignControlDeviceToConnectedDevices();
            }
        }
        else
        {
            powerConsumptionRate = 0;
            Debug.LogWarning(transform.name + " does not have any electrical devices connected to it, it is obsolete.");
        }

    }
    /// <summary>
    /// Calculates how much power this control device consumes once on (sum of the power consumption of all connected devices)
    /// </summary>
    void CalculatePowerConsumptionRate()
    {
        powerConsumptionRate = 0;
        for (int i = 0; i < connectedElectricalDevices.Length; i++)
        {
            powerConsumptionRate += connectedElectricalDevices[i].GetPowerConsumptionRate();
        }
    }
    /// <summary>
    /// Assigns this control device as the control device of all connected electrical devices
    /// </summary>
    void AssignControlDeviceToConnectedDevices()
    {
        for (int i = 0; i < connectedElectricalDevices.Length; i++)
        {
            connectedElectricalDevices[i].SetControlDevice(this);
        }
    }
    #endregion

    /// <summary>
    /// Turns on all the electrical devices that are connected to this control device
    /// </summary>
    protected override void DeviceOn()
    {
        for (int i = 0; i < connectedElectricalDevices.Length; i++)
        {
            connectedElectricalDevices[i].AddCurrent();
        }
        assignedBreaker.UpdateCurrentPowerConsumption();
    }

    /// <summary>
    /// Turns off all the electrical devices that are connected to this control device
    /// </summary>
    protected override void DeviceOff()
    {
        for (int i = 0; i < connectedElectricalDevices.Length; i++)
        {
            connectedElectricalDevices[i].RemoveCurrent();
        }
        assignedBreaker.UpdateCurrentPowerConsumption();
    }

    public abstract void OnInteract();

    public bool IsDeviceOn() => deviceOn;
}
