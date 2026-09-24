using UnityEngine;

public class FirearmBase : MonoBehaviour
{
    [SerializeField] FirearmFiring firearmFiring;
    public FirearmFiring Firing => firearmFiring;

    [SerializeField] FirearmFeeding firearmFeeding;
    public FirearmFeeding Feeding => firearmFeeding;
    
    [Tooltip("The position the camera must be in locally to aim down the firearm's sights")]
    [SerializeField] Vector3 adsPosition;
    
    public Vector3 GetADSPosition() => adsPosition;
}
