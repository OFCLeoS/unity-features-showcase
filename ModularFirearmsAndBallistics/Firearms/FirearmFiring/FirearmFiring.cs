using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base of all firing modules
/// </summary>
public abstract class FirearmFiring : MonoBehaviour
{
    //TODO: Add Dictionary to cache hit GOs?

    protected FirearmBase firearmBase;
    protected FirearmFeeding firearmFeeding;
    // TODO: !!! [SerializeField] ReactiveSoundEmitter firingSoundEmitter;
    [SerializeField] float minXRecoil = 0.05f;
    [SerializeField] float maxXRecoil = 0.1f;
    [SerializeField] float minYRecoil = 0.1f;
    [SerializeField] float maxYRecoil = 0.39f;

    protected float timeSinceLastShot = 5;
    protected float shootingCooldown = 0.125f; //The 0.125 is equivalent to an RPM of 480, this is done to counter macros
    protected bool hasReleasedTrigger = true;

    [SerializeField] protected Transform barrelExit;
    public Transform BarrelExit => barrelExit;

    #region Initialization
    protected virtual void Awake()
    {
        firearmBase = GetComponent<FirearmBase>();
        firearmFeeding = GetComponent<FirearmFeeding>();
        // if (!firingSoundEmitter)
        // {
        //     throw new MissingComponentException("The firearms Firing Sound Emitter was not found");
        // }
    }
    #endregion

    /// <summary>
    /// Fires the chambered bullet if possible
    /// </summary>
    /// <param name="xRecoil">The recoil on the x axis that the firearm outputted</param>
    /// <param name="yRecoil">The recoil on the y axis that the firearm outputted</param>
    /// <returns>True if the Firearm was fired</returns>
    public bool FireAction(out float xRecoil, out float yRecoil)
    {
        xRecoil = 0;
        yRecoil = 0;
        if (CanFire())
        {
            hasReleasedTrigger = false;
            // TODO: PROBLEM HERE?
            Bullet? bullet = firearmFeeding.GetChamberedBullet();
            if (bullet.HasValue)
            {
                //BULLET IS FIRED HERE
                BallisticsSystem.FireBullet(bullet.Value, barrelExit.position, barrelExit.forward);
                GenerateRecoil(out xRecoil, out yRecoil);
                //Debug.Log("BANG");//////////////DEBUG////////////////////////
                timeSinceLastShot = 0;
                // TODO: CHANGE !!!!!! firingSoundEmitter.EmitSound();
                firearmFeeding.ChamberBullet();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Releases the trigger
    /// </summary>
    public virtual void ReleaseTriggerAction() => hasReleasedTrigger = true;

    /// <summary>
    /// Basic fire check. Counters macros and checks for trigger release
    /// </summary>
    protected virtual bool CanFire()
    {
        return hasReleasedTrigger && timeSinceLastShot > shootingCooldown;
    }

    void GenerateRecoil(out float xRecoil, out float yRecoil)
    {
        int negativeXRecoil = Random.Range(0, 2);

        xRecoil = Random.Range(minXRecoil, maxXRecoil);
        yRecoil = Random.Range(minYRecoil, maxYRecoil);

        if (negativeXRecoil == 1) xRecoil = -xRecoil;
    }

    protected virtual void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (timeSinceLastShot >= 1) timeSinceLastShot = 5;
    }
}
