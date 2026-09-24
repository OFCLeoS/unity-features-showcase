using UnityEngine;

public class AircraftEngine : MonoBehaviour
{
    [SerializeField] Rigidbody planeRB;
    [SerializeField] Transform thrustCenter;

    [SerializeField] float engineMaxPower = 39;

    public float Thrust { private get; set; }

    float currentEnginePower;
    public float CurrentEnginePower { get => currentEnginePower; }

    void Awake()
    {
        Thrust = 0;
    }

    void FixedUpdate()
    {
        if (Thrust <= 0) return;
        currentEnginePower = engineMaxPower * Thrust;
        planeRB.AddForceAtPosition(thrustCenter.forward * currentEnginePower, thrustCenter.position);
    }
}