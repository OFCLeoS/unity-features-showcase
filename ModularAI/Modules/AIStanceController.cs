using UnityEngine;

/// <summary>
/// AI Module for controlling of the Agent's stance
/// </summary>
public class AIStanceController : AIModule
{
    AIRigController rigController;

    /// <summary>
    /// Goes from 0 (prone) to 1 (Standing)
    /// </summary>
    float currentStanceHeight;
    /// <summary>
    /// Goes from -1 (full left lean) to 1 (full right lean), where 0 is no leaning
    /// </summary>
    float currentLeanCoefficient;

    [SerializeField] float maxLeanAngle;

    // TODO: HANDLE COLLIDER! (prone box?) (Navmesh how?)

    #region Initialization
    void Start()
    {
        rigController = modulesDatabase.GetModule<AIRigController>();
    }
    #endregion

    /// <summary>
    /// -1 (full left lean) to 1 (full right lean), where 0 is no leaning
    /// </summary>
    public void SetLeanCoefficient(float leanCoefficient)
    {
        leanCoefficient = Mathf.Clamp(leanCoefficient, -1, 1);
        rigController.SetUpperBodyRotation(leanCoefficient * -maxLeanAngle); // - Because right leaning is negative
    }
}
