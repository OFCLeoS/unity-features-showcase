using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adds player controlled locomotion abilities to an object
/// </summary>
public class PlayerLocomotion : MonoBehaviour
{
    InputAction moveAction;
    InputAction sprintAction;
    InputAction adjustAction;

    CharacterController characterController;
    PlayerMovementController playerMovementController;
    PlayerStamina playerStamina;
    PlayerStances playerStance = PlayerStances.STANDING;

    [SerializeField] float fowardsSpeed = 0.15f;
    [SerializeField] float backwardsSpeed = 0.05f;
    [SerializeField] float sideSpeed = 0.1f;

    float stanceSpeedCoefficient = 1;
    [SerializeField] float crouchSpeedCoefficient = 0.5f;
    [SerializeField] float proneSpeedCoefficient = 0.2f;

    float speedCoefficient = 1;
    [SerializeField] float minimumSpeedCoefficient = 0.25f;

    [Tooltip("The value that one scroll unit adds/subtracts to the speed coefficient")]
    [SerializeField] float speedCoefficientIncrement = 0.1f;

    [SerializeField] float runningCoefficient = 1.5f;
    [SerializeField] float runningStaminaDecreaseRate = 0.9f;

    [SerializeField] float crouchingStaminaDecreaseRate = 0.3f;

    [SerializeField] HumanAnimationManager playerAnimationManager;
    int isRunningBool = Animator.StringToHash("isRunning");

    // TODO: !!! [SerializeField] ReactiveSoundEmitter footstepsEmitter;
    [SerializeField] float baseTimePerFootstep = 0.75f;
    float timePerFootstep = 1;
    float timePassed;

    #region Initialization
    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        adjustAction = InputSystem.actions.FindAction("Adjust");

        characterController = GetComponent<CharacterController>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerStamina = GetComponent<PlayerStamina>();

        if (baseTimePerFootstep <= 0) baseTimePerFootstep = 0.75f;
        timePerFootstep = baseTimePerFootstep;
    }
    #endregion

    /// <summary>
    /// Moves the player depending on the current state of the "move" input action
    /// </summary>
    void HandleLocomotion()
    {
        if (characterController.isGrounded) //We will only move if the player is grounded
        {
            //We get the movement vector
            Vector2 movementDirection = moveAction.ReadValue<Vector2>().normalized;
            float playerDepthSpeed = movementDirection.y > 0 ? fowardsSpeed : backwardsSpeed;
            float playerSideSpeed = sideSpeed;

            playerDepthSpeed *= stanceSpeedCoefficient;
            playerSideSpeed *= stanceSpeedCoefficient;

            if (sprintAction.IsPressed())
            {
                speedCoefficient = 1;
                if (playerStamina.GetLegStamina() > 0)
                {
                    playerAnimationManager.SetBool(isRunningBool, true);
                    playerDepthSpeed *= runningCoefficient;
                    playerSideSpeed *= runningCoefficient;
                    playerStamina.DecreaseLegStamina(runningStaminaDecreaseRate * Time.deltaTime);
                }
                else playerAnimationManager.SetBool(isRunningBool, false);
            }
            else playerAnimationManager.SetBool(isRunningBool, false);

            playerDepthSpeed *= speedCoefficient;
            playerSideSpeed *= speedCoefficient;

            if (movementDirection == Vector2.zero) timePassed = 0;
            else
            {
                timePassed += Time.deltaTime;
                HandleSoundEmition();

                if (playerStance == PlayerStances.CROUCHED) playerStamina.DecreaseLegStamina(crouchingStaminaDecreaseRate * Time.deltaTime);
            }

            Vector3 movementVector = transform.forward * (movementDirection.y * playerDepthSpeed) + transform.right * (movementDirection.x * playerSideSpeed);
            //We feed the movement to the PlayerMovementController so that it can interact with the other movement features
            playerMovementController.SetXVelocity(movementVector.x);
            playerMovementController.SetZVelocity(movementVector.z);
        }
    }

    void HandleSoundEmition()
    {
        if (timePassed >= timePerFootstep)
        {
            timePassed = 0;
            // TODO: !!! footstepsEmitter.EmitSound();
        }
    }

    /// <summary>
    /// Changes the speed coefficient depending on the adjust action
    /// </summary>
    void HandleSpeedCoefficient()
    {
        float adjustValue = adjustAction.ReadValue<float>(); //This will return -1, 0 or 1 depending on the scroll wheel (for PC) change in the current frame
        speedCoefficient += speedCoefficientIncrement * adjustValue;
        speedCoefficient = Mathf.Clamp(speedCoefficient, minimumSpeedCoefficient, 1);
        timePerFootstep = baseTimePerFootstep / speedCoefficient;
    }

    /// <summary>
    /// Changes the stance speed coefficient depending on a stance
    /// </summary>
    public void ChangeStanceSpeedCoefficient(PlayerStances stance)
    {
        playerStance = stance;
        switch (stance)
        {
            case PlayerStances.STANDING:
                stanceSpeedCoefficient = 1;
                break;
            case PlayerStances.CROUCHED:
                stanceSpeedCoefficient = crouchSpeedCoefficient;
                break;
            case PlayerStances.PRONED:
                stanceSpeedCoefficient = proneSpeedCoefficient;
                break;
            default:
                stanceSpeedCoefficient = 1;
                break;
        }
    }

    void Update()
    {
        HandleSpeedCoefficient();
        HandleLocomotion();
    }
}