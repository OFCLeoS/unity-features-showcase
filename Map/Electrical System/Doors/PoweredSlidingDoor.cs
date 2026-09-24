using UnityEngine;

public class PoweredSlidingDoor : ElectricalLoadDevice
{
    [SerializeField] SlidingObject door;
    protected override void DeviceOff()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeviceOn()
    {
        door.OnInteract();
    }
}
