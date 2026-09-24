using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftFlightController : MonoBehaviour
{
    InputAction thrustAction;
    InputAction pedalsAction;
    InputAction yokeAction;
    InputAction resetYokeAction;

    [SerializeField] Transform centerOfMass;

    [Tooltip("How fast the thrust changes in the plane.")]
    [SerializeField] float thrustChangeSpeedCoefficient = 5;

    [Tooltip("How fast the yoke changes to the pitch directions.")]
    [SerializeField] float yokePitchSensitivity = 0.3f;
    [Tooltip("How fast the yoke changes to the roll directions.")]
    [SerializeField] float yokeRollSensitivity = 0.3f;

    [Tooltip("Affects drag effect")]
    [SerializeField] float dragCoefficient = 0.5f;

    // Aircraft Parts
    [SerializeField] AircraftStatus status;
    [SerializeField] List<AircraftEngine> engines;
    [SerializeField] AircraftElevator elevator;
    [SerializeField] AircraftRudder rudder;
    [SerializeField] AircraftWing leftWing;
    [SerializeField] AircraftWing rightWing;

    /// <summary>
    /// Goes from -1 to 1. Controls Pitch and Roll.
    /// </summary>
    Vector2 yokeDirection;
    float thrust;
    /// <summary>
    /// Goes from 0 to 1. Controls how effective the yoke is at controlling the plane. (Low on low velocities, high on high)
    /// </summary>
    float yokeEffectiveness;

    Rigidbody rb;

    float pedalsInput;

    float currentDrag;
    public float CurrentDrag { get => currentDrag; }

    float angleOfAttack;
    public float AngleOfAttack { get => angleOfAttack; }

    void Awake()
    {
        // TODO REMOVE
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //

        rb = GetComponent<Rigidbody>();
        rb.automaticCenterOfMass = false;
        rb.centerOfMass = centerOfMass.localPosition;

        thrustAction = InputSystem.actions.FindAction("Thrust");
        pedalsAction = InputSystem.actions.FindAction("Pedals");
        yokeAction = InputSystem.actions.FindAction("Yoke");
        resetYokeAction = InputSystem.actions.FindAction("Reset Yoke");
    }

    void HandleThrust()
    {
        thrust += thrustAction.ReadValue<float>() * thrustChangeSpeedCoefficient * Time.deltaTime;
        thrust = Mathf.Clamp(thrust, 0, 1);

        // TODO: Only change one per frame?
        for (int i = 0; i < engines.Count; i++)
        {
            engines[i].Thrust = thrust;
        }
    }

    void HandleYaw()
    {
        pedalsInput = pedalsAction.ReadValue<float>();
        rudder.PartAxis = pedalsInput;
    }

    void HandleYoke()
    {
        Vector2 mouseDelta = yokeAction.ReadValue<Vector2>();

        #region Controller Like
        // yokeDirection.x = Mathf.Clamp(yokeDirection.x + (mouseDelta.x * yokeRollSensitivity * Time.deltaTime), -1, 1);
        // yokeDirection.y = Mathf.Clamp(yokeDirection.y + (mouseDelta.y * yokePitchSensitivity * Time.deltaTime), -1, 1);

        // if (resetYokeAction.WasPressedThisFrame()) yokeDirection = Vector2.zero;

        // elevator.PartAxis = yokeDirection.y;

        // leftWing.PartAxis = yokeDirection.x;
        // rightWing.PartAxis = -yokeDirection.x;
        #endregion

        #region Mouse Like
        mouseDelta.x = Mathf.Clamp(mouseDelta.x * yokeRollSensitivity * Time.deltaTime, -1, 1);
        mouseDelta.y = Mathf.Clamp(mouseDelta.y * yokePitchSensitivity * Time.deltaTime, -1, 1);

        elevator.PartAxis = mouseDelta.y;

        leftWing.PartAxis = mouseDelta.x;
        rightWing.PartAxis = -mouseDelta.x;
        #endregion
    }

    void HandlePlaneControls()
    {
        HandleThrust();
        HandleYaw();
        HandleYoke();
    }

    void HandleDrag()
    {
        // Drag formula: (may change)
        // Drag = dragCoefficient * airdensity * v2/2 * 
        currentDrag = dragCoefficient * status.AirDensity * (rb.linearVelocity.sqrMagnitude / 2);
        rb.AddForce(-rb.linearVelocity.normalized * currentDrag);
    }

    void CalculateAoA()
    {
        Vector3 aircraftLocalVelocity = rb.transform.InverseTransformDirection(rb.linearVelocity);

        // Angle of Attack (AoA) calculation:
        // Only y and z velocity will matter for AoA
        Vector2 zyLocalVelocity = new Vector2(aircraftLocalVelocity.z, aircraftLocalVelocity.y);
        Vector2 zyLocalFowardsDirection = new Vector2(1, 0);

        Debug.DrawRay(transform.position, transform.TransformDirection(aircraftLocalVelocity), Color.blue);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1)), Color.red);

        angleOfAttack = Vector2.SignedAngle(zyLocalVelocity, zyLocalFowardsDirection);
    }

    void Update()
    {
        HandlePlaneControls();
        HandleDrag();
        CalculateAoA();
        // Debug.Log("Angular Velocity: " + rb.angularVelocity);
        // Debug.Log("Linear Velocity: " + rb.linearVelocity);
    }
}