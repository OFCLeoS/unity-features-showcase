using UnityEngine;

public abstract class AircraftDualMotionPart : MonoBehaviour
{
    [SerializeField] protected Rigidbody aircraftRB;
    [SerializeField] protected AircraftStatus aircraftStatus;

    [Tooltip("The velocity in the x and z axis a part needs to be fully effective.")]
    [SerializeField] protected float partVelocityGoal = 200;
    [Tooltip("How much this part can influence the aircraft's motion.")]
    [SerializeField] protected float maxPartInfluence = 39;

    float squaredVelocityGoal;

    /// <summary>
    /// Goes from -1 to 1. 
    /// </summary>
    public float PartAxis { protected get; set; }

    /// <summary>
    /// How much does the part affecting the aircraft movement
    /// </summary>
    protected float partEffectivenessCoefficient = 0;

    /// <summary>
    /// The current force the part is exerting on the plane. Mainly used for UI.
    /// </summary>
    protected float currentPartForce;
    public float CurrentForce{get => currentPartForce;}

    void Awake()
    {
        // We use squared velocity so we don't have to calculate a square root
        squaredVelocityGoal = partVelocityGoal * partVelocityGoal;
        PartAxis = 0;
    }

    protected virtual float GetPartEffectivenessCoefficient()
    {
        // TODO: Change formula?

        // Y velocity will not matter for part effectiveness (horizontal airflow is needed for a part to be effective)
        float xzSquaredVelocity = new Vector2(aircraftRB.linearVelocity.x, aircraftRB.linearVelocity.z).sqrMagnitude;

        partEffectivenessCoefficient = xzSquaredVelocity / squaredVelocityGoal;

        if (partEffectivenessCoefficient > 1) partEffectivenessCoefficient = 1;

        return partEffectivenessCoefficient;
    }

    /// <summary>
    /// Calculates the force that the part is exerting on the aircraft
    /// </summary>
    protected virtual float GetPartForce()
    {
        // Force formula: (may change)
        // Force = (max part influence * Part Axis) * air density * part effectiveness
        currentPartForce = maxPartInfluence * PartAxis * GetPartEffectivenessCoefficient() * aircraftStatus.AirDensity;
        return currentPartForce;
    }

    /// <summary>
    /// Calculates the force vector that the part is exerting on the aircraft
    /// </summary>
    protected abstract Vector3 GetPartForceVector();

    protected virtual Vector3 GetForcePosition() => transform.position;

    protected virtual void FixedUpdate()
    {
        if (PartAxis == 0) return;
        GetPartEffectivenessCoefficient();
        aircraftRB.AddForceAtPosition(GetPartForceVector(), GetForcePosition(), ForceMode.Force);
    }
}