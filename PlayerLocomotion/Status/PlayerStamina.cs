using UnityEngine;

/// <summary>
/// Adds stamina functionality that can be used by the actions that require it
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    float maxLegStamina = 100;
    float maxArmStamina = 100;

    float legStamina;
    float armStamina;

    float staminaRecoveryCooldown = 3;
    float currentLegStaminaRecoveryTime = 0;
    float currentArmStaminaRecoveryTime = 0;

    [SerializeField] float legStaminaRecoveryRate = 0.2f;
    [SerializeField] float armStaminaRecoveryRate = 0.2f;

    #region Initialization
    void Awake()
    {
        legStamina = maxLegStamina;
        armStamina = maxArmStamina;
        //We set the the current recovery times to be equals to the cooldown due to the way they function
        currentLegStaminaRecoveryTime = staminaRecoveryCooldown;
        currentArmStaminaRecoveryTime = staminaRecoveryCooldown;
    }
    #endregion

    public void IncreaseLegStamina(float increaseValue) 
    {
        legStamina += increaseValue;
        CapLegStamina();
    }
    public void IncreaseArmStamina(float increaseValue)
    {
        armStamina += increaseValue;
        CapArmStamina();
    }
    
    public void DecreaseLegStamina(float decreaseValue) 
    {
        legStamina -= decreaseValue;
        currentLegStaminaRecoveryTime = 0; //We start the regen cooldown
        CapLegStamina();
    }
    public void DecreaseArmStamina(float decreaseValue)
    {
        armStamina -= decreaseValue;
        currentArmStaminaRecoveryTime = 0; //We start the regen cooldown
        CapArmStamina();
    }

    /// <summary>
    /// Checks whether or not the player needs to regenerate stamina and regenerates it if the cooldown is done
    /// </summary>
    void HandleStaminaRecovery()
    {
        if(currentLegStaminaRecoveryTime < staminaRecoveryCooldown) //We check if the cooldown is over
        {
            currentLegStaminaRecoveryTime += Time.deltaTime; //We add how much time it has been since last frame, this works because this method goes on the Update() method
        }
        else if(legStamina < maxLegStamina) //We check if we are already on max stamina
        {
            IncreaseLegStamina(legStaminaRecoveryRate * Time.deltaTime);
        }

        //Same concept as before
        if(currentArmStaminaRecoveryTime < staminaRecoveryCooldown)
        {
            currentArmStaminaRecoveryTime += Time.deltaTime;
        }
        else if(armStamina < maxArmStamina)
        {
            IncreaseArmStamina(legStaminaRecoveryRate * Time.deltaTime);
        }  
    }

    void CapLegStamina() => legStamina = Mathf.Clamp(legStamina,0,maxLegStamina);
    void CapArmStamina() => armStamina = Mathf.Clamp(armStamina,0,maxArmStamina);

    #region Getters
    public float GetLegStamina() => legStamina;
    public float GetArmStamina() => armStamina;
    #endregion

    
    void Update()
    {
        HandleStaminaRecovery();
    }
}
