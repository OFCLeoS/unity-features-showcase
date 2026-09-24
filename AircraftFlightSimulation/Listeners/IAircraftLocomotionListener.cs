using UnityEngine;

public interface IAircraftLocomotionListener
{
    public void ListenForAircraftLocomotion(Vector3 movementDelta, Quaternion deltaRotation);
}