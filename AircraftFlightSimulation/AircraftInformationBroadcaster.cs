using System.Collections.Generic;
using UnityEngine;

public class AircraftInformationBroadcaster : MonoBehaviour
{
    // TODO: AUTOMATE GATHERING THESE!!!
    [SerializeField] List<IAircraftLocomotionListener> locomotionListeners = new List<IAircraftLocomotionListener>();
    Vector3 lastPosition;
    Quaternion lastRotation;

    void Awake()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        // TODDO: REMOVE!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Debug.LogError("IMPLEMENT THIS!!!");
        // locomotionListeners.Add(FindAnyObjectByType<AircraftLocomotionStrategy>());
    }

    void Update() // TODO: Possible FixedUpdate switch?
    {
        for (int i = 0; i < locomotionListeners.Count; i++)
        {
            locomotionListeners[i].ListenForAircraftLocomotion(transform.position - lastPosition, Quaternion.Inverse(lastRotation) * transform.rotation);
        }
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}