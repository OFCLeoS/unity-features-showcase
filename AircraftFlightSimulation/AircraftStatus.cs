using UnityEngine;

public class AircraftStatus : MonoBehaviour
{
    float airDensity = 1.225f;
    public float AirDensity { get => airDensity; private set { airDensity = value; } }
}
