using UnityEngine;

/// <summary>
/// AI Module that mnakes an AI Agent fire their firearm if aiming at the intended target.
/// </summary>
public class AIFireAtTargetBehaviour : AIModule
{
    [SerializeField] AIAimBehaviour aimBehaviour;
    [SerializeField] AIFirearmsController firearmsController;
    [SerializeField] AIThreatAssessorService threatAssessor;

    Transform target;

    #region Initialization
    void Start()
    {
        aimBehaviour = modulesDatabase.GetModule<AIAimBehaviour>();
        firearmsController = modulesDatabase.GetModule<AIFirearmsController>();
        threatAssessor = modulesDatabase.GetModule<AIThreatAssessorService>();
        enabled = false;
    }
    #endregion

    public void HandleFiring()
    {
        // If the Agent deems a shot to be acceptable, they will pull the trigger
        if (aimBehaviour.HasAcceptableShot(target.position))
        {
            // If the trigger was pressed and nothing came out, the behaviour is stopped
            // REDUNDANT? if (firearmsHandler.AttemptToFire()) StopFiring();
            firearmsController.AttemptToFire();
        }
    }

    /// <summary>
    /// Agent will start firing at a target
    /// </summary>
    public void StartFiring(Transform target)
    {
        this.target = target;
        enabled = true;
    }

    /// <summary>
    /// Agent will start firing at the Current Hostile In Focus
    /// </summary>
    public void StartFiring()
    {
        target = threatAssessor.CurrentHostileInFocus;
        enabled = true;
    }

    /// <summary>
    /// Agent will stop firing at their target
    /// </summary>
    public void StopFiring()
    {
        enabled = false;
    }

    #region Performance
    const float MIN_CHECK_TIME = 0.01f;
    const float MAX_CHECK_TIME = 0.1f;
    float timeSinceLastCheck = 0;
    float nextCheckTime = 0;
    #endregion

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= nextCheckTime)
        {
            HandleFiring();
            timeSinceLastCheck = 0;
            nextCheckTime = Random.Range(MIN_CHECK_TIME, MAX_CHECK_TIME); // TODO: Use faster library?
        }
    }
}
