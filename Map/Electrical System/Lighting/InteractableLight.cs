using UnityEngine;

[RequireComponent(typeof(Light))]
public class InteractableLight : ElectricalLoadDevice
{
    new Light light;
    float startingIntensity;

    #region Initialization
    protected override void Awake()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
        base.Awake();
    }
    #endregion

    /// <summary>
    /// Turns on the light
    /// </summary>
    protected override void DeviceOn()
    {
        //Realistic spasms later?
        light.intensity = startingIntensity;
    }

    /// <summary>
    /// Turns off the light
    /// </summary>
    protected override void DeviceOff()
    {
        light.intensity = 0;
    }
}
