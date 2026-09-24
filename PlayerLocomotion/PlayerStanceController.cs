using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStanceController : MonoBehaviour
{
    InputAction crouchAction;
    InputAction proneAction;
    InputAction adjustAction;

    PlayerLocomotion playerLocomotion;
    CharacterController characterController;
    PlayerStances currentPlayerStance;

    [SerializeField] Transform rotationPivot;
    Vector3 initialPivotPosition;

    float standingHeight;
    [SerializeField] float crouchingHeight = 1.39f;
    [SerializeField] float proneHeight = 1f;
    float targetHeight;

    [SerializeField] float crouchingSpeed = 9;
    [SerializeField] float unCrouchingSpeed = 7;
    [SerializeField] float proningSpeed = 6;
    float targetSpeed;

    [Tooltip("The value that one scroll unit adds/subtracts to the crouch height (NOT YET IMPLEMENTED)")]
    [SerializeField] float crouchHeightIncrementation = 0.1f;

    #region Initialization
    void Awake()
    {
        crouchAction = InputSystem.actions.FindAction("Crouch");
        proneAction = InputSystem.actions.FindAction("Prone");
        adjustAction = InputSystem.actions.FindAction("Adjust");

        playerLocomotion = GetComponent<PlayerLocomotion>();
        characterController = GetComponent<CharacterController>();

        standingHeight = characterController.height;
        targetHeight = standingHeight;
        targetSpeed = unCrouchingSpeed;
        initialPivotPosition = rotationPivot.localPosition;
    }
    #endregion


    /// <summary>
    /// Handles the adjustments made to the crouching stance by the player
    /// </summary>
    void HandleStanceAdjustments()
    {
        
    }

    /// <summary>
    /// Handles the current player stance depending on various factors
    /// </summary>
    void HandleStances()
    {
        if (crouchAction.WasPressedThisFrame())
        {
            switch (currentPlayerStance)
            {
                case PlayerStances.STANDING:
                case PlayerStances.PRONED:
                    currentPlayerStance = PlayerStances.CROUCHED;
                    targetSpeed = crouchingSpeed;
                    targetHeight = crouchingHeight;
                    break;
                case PlayerStances.CROUCHED:
                    currentPlayerStance = PlayerStances.STANDING;
                    targetSpeed = unCrouchingSpeed;
                    targetHeight = standingHeight;
                    break;
            }
            playerLocomotion.ChangeStanceSpeedCoefficient(currentPlayerStance);
        }
        if (proneAction.WasPressedThisFrame())
        {
            switch (currentPlayerStance)
            {
                case PlayerStances.STANDING:
                case PlayerStances.CROUCHED:
                    currentPlayerStance = PlayerStances.PRONED;
                    targetSpeed = proningSpeed;
                    targetHeight = proneHeight;
                    break;
                case PlayerStances.PRONED:
                    currentPlayerStance = PlayerStances.STANDING;
                    targetSpeed = proningSpeed;
                    targetHeight = standingHeight;
                    break;
            }
            playerLocomotion.ChangeStanceSpeedCoefficient(currentPlayerStance);
        }
        AdjustHeight();
    }

    /// <summary>
    /// Adjusts the height of the character controller according to the target height
    /// </summary>
    void AdjustHeight()
    {
        //If the fact that the character never 100% reaches its target height affects gameplay, add a check to it
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, targetSpeed * Time.deltaTime);

        //We adjust the rotation pivot, arms and weapon to fit the new height
        Vector3 halfHeightDifference = new Vector3(0, (standingHeight - characterController.height) / 2, 0);
        rotationPivot.localPosition = initialPivotPosition - halfHeightDifference;
    }

    void Update()
    {
        //HandleStanceAdjustments();
        HandleStances();
    }
}
