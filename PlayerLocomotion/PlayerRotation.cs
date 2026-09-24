using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adds rotation depending on the player's mouse movement
/// </summary>
public class PlayerRotation : MonoBehaviour
{
    InputAction lookAction;
    InputAction freeLookAction;
    InputAction leanLeftAction;
    InputAction leanRightAction;

    Vector2 currentRotation = Vector2.zero;
    [SerializeField] float rotationAngleClamp = 80;

    [SerializeField] float rotationSensitivity;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform rotationPivot;

    Vector2 currentFreeLookRotation = Vector2.zero;
    [SerializeField] float freeLookXRotationAngleClamp = 80;
    [SerializeField] float freeLookYRotationAngleClamp = 100;
    bool isFreeLooking;

    [Tooltip("How fast the free look rotation will be removed from the camera")]
    [SerializeField] float freeLookResetSpeed = 50;

    float currentZRotation;
    [SerializeField] float leanZRotation;
    [SerializeField] float leaningSpeed;

    #region Initialization
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        lookAction = InputSystem.actions.FindAction("Look");
        freeLookAction = InputSystem.actions.FindAction("FreeLook");
        leanLeftAction = InputSystem.actions.FindAction("Lean Left");
        leanRightAction = InputSystem.actions.FindAction("Lean Right");
    }
    #endregion

    /// <summary>
    /// Rotates the player depending on the current state of the "look" input action
    /// </summary>
    void HandleRotation()
    {
        if (!isFreeLooking)
        {
            Vector2 lookDirection = lookAction.ReadValue<Vector2>();
            //We adjust the current rotation values according to how the mouse has been moved this frame
            currentRotation.x += lookDirection.x * rotationSensitivity * Time.deltaTime;

            currentRotation.y += lookDirection.y * rotationSensitivity * Time.deltaTime;
            currentRotation.y = Mathf.Clamp(currentRotation.y, -rotationAngleClamp, rotationAngleClamp); //We clamp the rotation so that the player can't do 360s with the camera

            transform.localRotation = Quaternion.Euler(0, currentRotation.x, 0);
            rotationPivot.localRotation = Quaternion.Euler(-currentRotation.y, 0, currentZRotation);
            
            if (currentFreeLookRotation.sqrMagnitude < Mathf.Epsilon) //A more performant way of saying "currentFreeLookRotation == Vector2.zero"
            {
                playerCamera.localRotation = Quaternion.identity;
            }
            else //If there is some remaining free look rotation, we slowly remove it for a smooth feeling
            {
                currentFreeLookRotation = Vector2.Lerp(currentFreeLookRotation, Vector2.zero, freeLookResetSpeed * Time.deltaTime);
                playerCamera.localRotation = Quaternion.Euler(-currentFreeLookRotation.y, currentFreeLookRotation.x, 0);
            }
        }
    }

    /// <summary>
    /// Rotates the camera depending on the current state of the "look" and "free look" input action
    /// </summary>
    void HandleFreeLook()
    {
        if (isFreeLooking)
        {
            Vector2 lookDirection = lookAction.ReadValue<Vector2>();
            //We adjust the current rotation values according to how the mouse has been moved this frame
            currentFreeLookRotation.x += lookDirection.x * rotationSensitivity * Time.deltaTime;
            currentFreeLookRotation.x = Mathf.Clamp(currentFreeLookRotation.x, -freeLookXRotationAngleClamp, freeLookXRotationAngleClamp);

            currentFreeLookRotation.y += lookDirection.y * rotationSensitivity * Time.deltaTime;
            currentFreeLookRotation.y = Mathf.Clamp(currentFreeLookRotation.y, -freeLookYRotationAngleClamp, freeLookYRotationAngleClamp);

            playerCamera.localRotation = Quaternion.Euler(-currentFreeLookRotation.y, currentFreeLookRotation.x, 0);
        }
    }

    public Vector2 GetCurrentRotation() => currentRotation;

    /// <summary>
    /// Adds a rotation to the current rotation
    /// </summary>
    /// <param name="xRotation">X rotation to be added</param>
    /// <param name="yRotation">Y rotation to be added</param>
    public void AddRotation(float xRotation, float yRotation)
    {
        currentRotation.x += xRotation;
        currentRotation.y += yRotation;
    }

    void HandleLeaning()
    {
        if(leanLeftAction.IsPressed())
        {
            currentZRotation = Mathf.Lerp(currentZRotation,leanZRotation,leaningSpeed * Time.deltaTime);
        }
        else if(leanRightAction.IsPressed())
        {
            currentZRotation = Mathf.Lerp(currentZRotation,-leanZRotation,leaningSpeed * Time.deltaTime);
        }
        else if (currentZRotation > 0.00000000039 || currentZRotation < -0.00000000039)
        {
            currentZRotation = Mathf.Lerp(currentZRotation,0,leaningSpeed * Time.deltaTime);
        }
    }

    void Update()
    {
        if (!freeLookAction.IsPressed()) isFreeLooking = false;
        else isFreeLooking = true;

        HandleLeaning();
        HandleRotation();
        HandleFreeLook();
    }
}
