using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class SlidingObject : MonoBehaviour, IInteractable
{
    [SerializeField] float movementTime = 1f;
    [SerializeField] bool startOpen;
    bool isOpen = false;
    bool canInteract = true;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;

    void Start()
    {
        if (startOpen)
        {
            HandleSliding();
        }
    }

    #region player interaction
    public void OnInteract()
    {

        HandleSliding();

    }
    #endregion

    #region Open and Close Sliding Object scripts
    IEnumerator SlideOpen()
    {
        canInteract = false;
        float timePassed = 0;
        float t = 0;
        while (t <= 1)
        {
            t = timePassed / movementTime;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            timePassed += Time.deltaTime;
            yield return null;
        }
        canInteract = true;
        isOpen = true;
    }

    IEnumerator SlideClose()
    {
        canInteract = false;
        float timePassed = 0;
        float t = 0;
        while (t <= 1)
        {
            t = timePassed / movementTime;
            transform.localPosition = Vector3.Lerp(endPosition, startPosition, t);
            timePassed += Time.deltaTime;
            yield return null;
        }
        canInteract = true;
        isOpen = false;
    }
    
    public void HandleSliding()
    {
        if (canInteract)
        {
            if (!isOpen)
            {
                StartCoroutine(SlideOpen());
            }
            else
            {
                StartCoroutine(SlideClose());
            }
        }
    }
    
    #endregion
}