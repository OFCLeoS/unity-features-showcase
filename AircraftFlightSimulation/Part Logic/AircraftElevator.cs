using UnityEngine;

public class AircraftElevator : AircraftDualMotionPart
{
    protected override Vector3 GetPartForceVector()
    {
        return transform.up * GetPartForce();
    }
}
