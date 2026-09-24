using System.Collections;
using UnityEngine;

public class HingedObject : MonoBehaviour, IInteractable
{
    [SerializeField] float movementTime = 1f;
    [SerializeField] bool startOpen;
    bool isOpen = false;
    bool canInteract = true;
    [SerializeField] Vector3 startRotation;
    [SerializeField] Vector3 endRotation;
    [SerializeField] Transform hinge;

    void Awake()
    {
        if(hinge == null)
        {
            hinge = transform;
        }
    }
    void Start()
    {
        if (startOpen)
        {
            HandleRotation();
        }
    }

    #region player interaction
    public void OnInteract()
    {

        HandleRotation();

    }
    #endregion

    #region Open and Close Sliding Object scripts
    IEnumerator RotateOpen()
    {
        canInteract = false;
        float timePassed = 0;
        float t = 0;
        while (t <= 1)
        {
            t = timePassed / movementTime;
            hinge.localRotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
            timePassed += Time.deltaTime;
            yield return null;
        }
        canInteract = true;
        isOpen = true;
    }

    IEnumerator RotateClose()
    {
        canInteract = false;
        float timePassed = 0;
        float t = 0;
        while (t <= 1)
        {
            t = timePassed / movementTime;
            hinge.localRotation = Quaternion.Lerp(Quaternion.Euler(endRotation), Quaternion.Euler(startRotation), t);
            timePassed += Time.deltaTime;
            yield return null;
        }
        canInteract = true;
        isOpen = false;
    }
    
    public void HandleRotation()
    {
        if (canInteract)
        {
            if (!isOpen)
            {
                StartCoroutine(RotateOpen());
            }
            else
            {
                StartCoroutine(RotateClose());
            }
        }
    }
    
    #endregion
}
