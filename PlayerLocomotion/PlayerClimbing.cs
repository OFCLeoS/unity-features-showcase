using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adds player controlled climbing abilities to an object
/// </summary>
public class PlayerClimbing : MonoBehaviour
{
    InputAction jumpAction;

    bool isJumping;

    PlayerStamina playerStamina;
    PlayerMovementController playerMovementController;

    CharacterController characterController;

    [SerializeField] float jumpForce = 3;
    [SerializeField] float jumpStaminaDecrease = 15;

    #region Initialization
    void Awake()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        playerStamina = GetComponent<PlayerStamina>();
        playerMovementController = GetComponent<PlayerMovementController>();
        characterController = GetComponent<CharacterController>();
    }
    #endregion

    /// <summary>
    /// Handles jumping depending on several conditions
    /// </summary>
    void HandleJumping()
    {
        if (jumpAction.WasPressedThisFrame() && characterController.isGrounded && playerStamina.GetLegStamina() > jumpStaminaDecrease) //Leg stamina should be more than it takes to jump to be able to do so
        {
            playerStamina.DecreaseLegStamina(jumpStaminaDecrease);
            playerMovementController.AddVelocity(Vector3.up * jumpForce);
        }

    }

    void Update()
    {
        HandleJumping();
    }
}
