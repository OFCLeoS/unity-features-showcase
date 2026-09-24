using UnityEngine;

public class AircraftWing : AircraftDualMotionPart
{
    [Tooltip("The wing area of the aircraft. This is not meant to be exact, it should be tuned for lift changes.")]
    [SerializeField] float wingArea = 161;
    [Tooltip("Any AoA past this will cause an aerodynamic stall.")]
    [SerializeField] float criticalAngleOfAttack = 17;
    [Tooltip("Lift Coefficient when the AoA is 0 Degrees.")]
    [SerializeField] float liftCoefficientAt0AoA = 0.01f;
    [Tooltip("How quickly the lift coefficient increases with AoA.")]
    [SerializeField] float liftCoefficientIncreaseCoefficient = 1;
    [SerializeField] AircraftStatus status;
    [SerializeField] AircraftFlightController flightController;

    float lift;
    public float Lift { get => lift; }

    protected override Vector3 GetPartForceVector()
    {
        return transform.up * GetPartForce();
    }

    void CalculateAndApplyLift()
    {
        float aoa = flightController.AngleOfAttack;
        // Stall
        // TODO: Research heavy AoA
        if (aoa > criticalAngleOfAttack || aoa < -criticalAngleOfAttack)
        {
            // For now, nothing will happen, which will essential cause a sudden drop in the aircraft, this can be changed later
            // with smooth lift loss
        }
        else
        {
            float liftCoefficient = liftCoefficientAt0AoA + (liftCoefficientIncreaseCoefficient * (aoa * Mathf.Deg2Rad));
            float airDensity = status.AirDensity;
            // We only get the xz velocity, ignoring vertical a/descent
            float velocitySquared = new Vector2(aircraftRB.linearVelocity.x, aircraftRB.linearVelocity.z).sqrMagnitude;

            lift = 0.5f * airDensity * (velocitySquared / 2) * wingArea * liftCoefficient;

            aircraftRB.AddForce(Vector3.up * lift);
        }
    }

    protected override void FixedUpdate()
    {
        CalculateAndApplyLift();
        base.FixedUpdate();
    }
}
