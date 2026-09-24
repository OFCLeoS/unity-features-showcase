using UnityEngine;
using System.Collections;
using Unity.AppUI.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;


public class DoorLogic : MonoBehaviour, IInteractable
{
    [SerializeField] bool toggle;
    [SerializeField] Animator anim;
    [SerializeField] float doorInteractDelay = 2;
    [SerializeField] float autoCloseDoorTime;

    bool canInteractDoor = true;

    #region player interaction
    public void OnInteract()
    {
        
        openclose(); 
        
    }
    #endregion

    #region wait for time period before allowing player to interact with door
    IEnumerator WaitInteractionTime()
    {
        yield return new WaitForSeconds(doorInteractDelay);
        canInteractDoor = true;
    }
    #endregion
    
    #region door open and close logic
    public void openclose()
    {
        if(canInteractDoor)
        {
            toggle = !toggle;
            if(toggle == false)
            {
                anim.ResetTrigger("open");
                anim.SetTrigger("close");
            }
            if(toggle == true)
            {
                anim.ResetTrigger("close");
                anim.SetTrigger("open");
            }
            canInteractDoor = false;
            StartCoroutine(WaitInteractionTime());
        }
    }
    #endregion
}
