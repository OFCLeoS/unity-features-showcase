using UnityEngine;

/// <summary>
/// The controller for all movement related actions for a player. Must be used by most movement modules.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    CharacterController characterController;
    Vector3 playerVelocity;

    #region Initialization
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    #endregion

    public void AddVelocity(Vector3 velocity) => playerVelocity += velocity;

    #region Setters
    public void SetXVelocity(float xVelocity) => playerVelocity.x = xVelocity;
    public void SetYVelocity(float yVelocity) => playerVelocity.y = yVelocity;
    public void SetZVelocity(float zVelocity) => playerVelocity.z = zVelocity;
    #endregion

    public Vector3 GetVelocity() => playerVelocity;

    void Update()
    {
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
