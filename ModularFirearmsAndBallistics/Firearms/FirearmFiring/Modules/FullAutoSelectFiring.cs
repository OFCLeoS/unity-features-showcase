using UnityEngine;
using UnityEngine.InputSystem;

public class FullAutoSelectFiring : FirearmFiring
{
    InputAction fireAction;
    
    [Tooltip("How many rounds can this firearm fire per minute if in full auto firemode")]
    [SerializeField] int rpm;

    bool fullAuto = true;

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        fireAction = InputSystem.actions.FindAction("Fire");
        shootingCooldown = 1.0f/(rpm/60.0f); // 1/(RPM/60) to turn RPM into SPR (seconds per round)
    }
    #endregion

    /// <summary>
    /// Ignores the need to release the trigger if full auto is selected, otherwise performs the basic fire check.
    /// </summary>
    protected override bool CanFire()
    {
        if(fullAuto) return timeSinceLastShot > shootingCooldown;
        else return base.CanFire();
    }
}
