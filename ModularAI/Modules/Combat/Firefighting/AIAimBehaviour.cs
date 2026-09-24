using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI Behaviour Module related to an AI's aiming capabilities using firearms
/// </summary>
public class AIAimBehaviour : AIModule
{
    #region Normalization
    /// <summary>
    /// The minimum Dot Product for trigger discipline (DOF)
    /// </summary>
    const float MIN_DOT_PRODUCT = 0.96f;
    /// <summary>
    /// The maximum Dot Product for trigger discipline (DOF)
    /// </summary>
    const float MAX_DOT_PRODUCT = 0.9995f;
    #endregion

    /// <summary>
    /// The maximum dot product between the fowards direction of the weapon and the desired rotation. Anything above this should result in an early exit.
    /// </summary>
    const float MAX_FOWARDS_DOT = 0.99993f;

    AIFirearmsController firearmsController;
    AIRigController rigController;

    [Tooltip("How fast will the Agent rotate towards the target")]
    [SerializeField] float rotationSpeed;

    // TODO: REPLACE WITH AI BEHAVIOUR PROFILES
    [Tooltip("How far from the target can the weapon be before the Agent starts shooting (0 - Horrendous Trigger Discipline, 1 - Fire only when 100% certain)")]
    [Range(0f, 1f)]
    [SerializeField] float allowedDOF;

    /// <summary>
    /// Who is the Agent Currently Aiming at
    /// </summary>
    Vector3 aimTarget;
    bool hasAimTarget = false;

    #region Initialization
    void Start()
    {
        firearmsController = modulesDatabase.GetModule<AIFirearmsController>();
        rigController = modulesDatabase.GetModule<AIRigController>();
    }
    #endregion

    public void SetAimTarget(Vector3 target)
    {
        if (target == aimTarget) return;

        aimTarget = target;
        hasAimTarget = true;
    }
    public void ResetAimTarget()
    {
        hasAimTarget = false;
    }

    /// <summary>
    /// Rotates the parent so that the weapon barrel faces towards a certain rotation
    /// </summary>
    void Rotate(Quaternion desiredBarrelRotation)
    {
        Quaternion targetBodyRotation = desiredBarrelRotation * Quaternion.Inverse(firearmsController.EquippedFirearm.Firing.BarrelExit.localRotation);

        Vector3 eulerRotation = targetBodyRotation.eulerAngles;
        // We should not rotate on the z axis

        rigController.SetUpperBodyRotation(eulerRotation.x, eulerRotation.y);
    }

    /// <summary>
    /// Aims towards the target. Takes into account the AI's skill with firearms.
    /// </summary>
    void AimTowardsTarget()
    {
        Vector3 directionToTarget = aimTarget - firearmsController.EquippedFirearm.Firing.BarrelExit.position;
        // If direction is 0, an error will occur if it goes through LookRotation!

        // Early Exit
        Vector3 desiredForward = directionToTarget.normalized;

        // Debug.DrawLine(firearmsController.EquippedFirearm.Firing.BarrelExit.position, aimTarget, Color.green, 1.0f);

        if (Vector3.Dot(firearmsController.EquippedFirearm.Firing.BarrelExit.forward, desiredForward) > MAX_FOWARDS_DOT)
        {
            //Debug.Log("I LEAAAAVE");
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, firearmsController.EquippedFirearm.Firing.BarrelExit.up);

        //TODO: Add noise to target rotation -> GET CURRENT ACCURACY MODIFER ON AI
        Quaternion desiredWeaponBarrelRotation = Quaternion.RotateTowards(firearmsController.EquippedFirearm.Firing.BarrelExit.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Rotate(desiredWeaponBarrelRotation);
    }


    /// <summary>
    /// Checks whether or not shooting in this situation is acceptable. Takes into account the AI's skill with firearms (such as trigger hapiness).
    /// </summary>
    /// <param name="target">The shooting target.</param>
    /// <returns>True if the shot is acceptable</returns>
    public bool HasAcceptableShot(Vector3 target)
    {
        //TODO: POSSIBLE NEGLIGEABLE DISTANCE!!!
        Vector3 directionToTarget = (target - firearmsController.EquippedFirearm.Firing.BarrelExit.position).normalized;

        float alignment = Vector3.Dot(firearmsController.EquippedFirearm.Firing.BarrelExit.forward, directionToTarget);

        // Degree Of Flawlessness: How perfect is the alignment of the weapon to the target (0: Awful, 1: Perfect)
        float dof = (alignment - MIN_DOT_PRODUCT) / (MAX_DOT_PRODUCT - MIN_DOT_PRODUCT);

        // Debug.Log("New Unit: " + dof);
        // Debug.Log("Alignment: " + alignment);
        return dof >= allowedDOF;
    }

    // TODO: Needed?
    #region Performance
    const float MIN_CHECK_TIME = 0.3f;
    const float MAX_CHECK_TIME = 0.6f;
    float timeSinceLastCheck = 0;
    float nextCheckTime = 0;
    #endregion

    void Update()
    {
        if (hasAimTarget) AimTowardsTarget();
    }
}