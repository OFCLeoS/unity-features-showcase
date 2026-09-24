using UnityEngine;

public class AircraftRudder : AircraftDualMotionPart
{
    protected override Vector3 GetPartForceVector()
    {
        return -transform.right * GetPartForce();
    }
}
