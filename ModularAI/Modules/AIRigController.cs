using UnityEngine;

[DefaultExecutionOrder(10000)]
public class AIRigController : AIModule
{
    [SerializeField] Transform upperBody;

    public void SetUpperBodyRotation(Quaternion rotation)
    {
        upperBody.rotation = rotation;
    }

    /// <summary>
    /// Used for Euler Rotations on the x and y axis of the Upper Body
    /// </summary>
    public void SetUpperBodyRotation(float xRotation, float yRotation)
    {
        upperBody.rotation = Quaternion.Euler(xRotation, yRotation, upperBody.eulerAngles.z);
    }

    /// <summary>
    /// Used for Euler Rotations on the z axis of the Upper Body
    /// </summary>
    public void SetUpperBodyRotation(float zRotation)
    {
        upperBody.rotation = Quaternion.Euler(upperBody.eulerAngles.x, upperBody.eulerAngles.y, zRotation);
    }
}
