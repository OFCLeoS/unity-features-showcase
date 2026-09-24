using System.Collections.Generic;
using UnityEngine;

public class ElectricalBreaker : MonoBehaviour, IInteractable
{
    List<ElectricalControlDevice> connectedElectricalControlDevices = new List<ElectricalControlDevice>();

    [SerializeField] bool isOn;

    [Tooltip("The minimum treshold this breaker can have in Ls")]
    [SerializeField] float minimumConsumption;
    [Tooltip("The maximum treshold this breaker can have in Ls")]
    [SerializeField] float maximumConsumption;

    float powerConsumptionTreshold;
    float currentPowerConsumption;

    #region Initialization
    void Start()
    {
        RandomizeConsumptionTreshold();
        if (isOn) FlipOn();
        else FlipOff();
    }
    void RandomizeConsumptionTreshold()
    {
        //REPLACE WITH ANOTHER FORMULA LATER
        powerConsumptionTreshold = Random.Range(minimumConsumption, maximumConsumption);
    }
    #endregion

    /// <summary>
    /// If the breaker is on, checks if the current grid is not too strenuous
    /// </summary>
    /// <returns>True if the breaker remains on after the update</returns>
    public bool UpdateCurrentPowerConsumption()
    {
        currentPowerConsumption = 0;
        if (isOn)
        {
            for (int i = 0; i < connectedElectricalControlDevices.Count; i++)
            {
                if (connectedElectricalControlDevices[i].IsDeviceOn())
                {
                    currentPowerConsumption += connectedElectricalControlDevices[i].GetPowerConsumptionRate();
                }
                if (currentPowerConsumption > powerConsumptionTreshold)
                {
                    TripBreaker();
                    return false;
                }
            }
            return true; //If code reaches here, the breaker was not tripped
        }
        else return false;
    }

    /// <summary>
    /// Flips on the breaker, adding current to everything that is connected to it and tripping if the power consumption exceeds the threshold
    /// </summary>
    void FlipOn()
    {
        isOn = true;
        int i = 0;
        while (i < connectedElectricalControlDevices.Count && isOn) //isOn will be false if breaker is tripped (I think it works idk)
        {
            Debug.Log(i);
            connectedElectricalControlDevices[i].AddCurrent();
            i++;
        }
    }

    /// <summary>
    /// Flips off the breaker, removing current from everything that is connected to it
    /// </summary>
    void FlipOff()
    {
        isOn = false;
        for (int i = 0; i < connectedElectricalControlDevices.Count; i++)
        {
            connectedElectricalControlDevices[i].RemoveCurrent();
        }
    }

    /// <summary>
    /// Trips the circuit breaker
    /// </summary>
    void TripBreaker() => FlipOff();

    public void OnInteract()
    {
        if (isOn) FlipOff();
        else FlipOn();
    }

    /// <summary>
    /// Connects an electrical control device to the breaker
    /// </summary>
    /// <param name="electricalControlDevice">The electrical control device to connect</param>
    public void AddElectricalControlDevice(ElectricalControlDevice electricalControlDevice)
    {
        connectedElectricalControlDevices.Add(electricalControlDevice);
    }
}
