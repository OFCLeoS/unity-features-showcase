using UnityEngine;

/// <summary>
/// Assures that the player is constantly getting grounded
/// </summary>
public class PlayerGrounding : MonoBehaviour
{
    PlayerMovementController playerMovementController;

    CharacterController characterController;
    float playerYVelocity;

    [SerializeField] float gravityMultiplier = 2;
    [Tooltip("How fast will the movement the player retained slow down")]
    [SerializeField] float movementSlowDownCoefficient = 0.998f;

    #region Initialization
    void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        characterController = GetComponent<CharacterController>();
    }
    #endregion

    /// <summary>
    /// Grounds the player if they are not grounded
    /// </summary>
    void HandleGrounding()
    {
        if (characterController.isGrounded /*&& playerYVelocity < 0*/)
        {
            playerYVelocity = -0.39f; //We add a constant gravity to avoid grounded/not grounded jitter

            playerMovementController.SetYVelocity(playerYVelocity);
        }
        else
        {
            playerYVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            playerMovementController.AddVelocity(Vector3.up * playerYVelocity);
            //We slowly slow down previous x and z velocity
            playerMovementController.SetXVelocity(playerMovementController.GetVelocity().x * movementSlowDownCoefficient);
            playerMovementController.SetZVelocity(playerMovementController.GetVelocity().z * movementSlowDownCoefficient);
        }
    }

    void Update()
    {
        HandleGrounding();
    }
}
