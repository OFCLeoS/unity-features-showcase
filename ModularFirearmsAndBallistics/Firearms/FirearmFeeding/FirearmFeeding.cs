using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base of all feeding modules
/// </summary>
public abstract class FirearmFeeding : MonoBehaviour
{
    protected FirearmBase firearmBase;
    protected Bullet? chamberedBullet;
    protected bool isReloading;

    protected List<FirearmReloadInstruction> reloadInstructionsCache;

    [Tooltip("Assure the red arrow is facing where the bullets will go towards")]
    [SerializeField] protected Transform chamberEjectionTransform;

    public bool HasOneInTheChamber => chamberedBullet != null;

    #region Initialization
    protected virtual void Awake()
    {
        firearmBase = GetComponent<FirearmBase>();
        reloadInstructionsCache = new List<FirearmReloadInstruction>();
    }
    #endregion

    /// <summary>
    /// Should be called by an IFirearmOrdnanceExchanger before the first step of the reload is completed
    /// </summary>
    public void StartReload() => isReloading = true;
    /// <summary>
    /// Should be called by an IFirearmOrdnanceExchanger when the last step of the reload is completed
    /// </summary>
    public void FinishReload() => isReloading = false;

    /// <summary>
    /// Default Bullet Chambering action.
    /// </summary>
    /// <returns>True if a bullet was chambered</returns>
    public abstract bool ChamberBullet();

    /// <summary>
    /// Ejects the shell from the chamber
    /// </summary>
    public virtual void ChamberEject()
    {
        if (!chamberedBullet.HasValue) return;
        GameObject emptyShell = Instantiate(chamberedBullet.Value.ShellCasingObject, chamberEjectionTransform.position, Quaternion.Euler(90, 0, 0));
        emptyShell.GetComponent<Rigidbody>().AddForce(emptyShell.transform.right * 100);
        chamberedBullet = null;
    }

    public Bullet? GetChamberedBullet() => chamberedBullet;

    /// <summary>
    /// Gets the Instructions for executing a reload for this firearm
    /// </summary>
    public List<FirearmReloadInstruction> GetReloadInstructions(bool fastReload, IFirearmOrdnanceExchanger ordnanceExchanger)
    {
        if (fastReload) return FastReload(ordnanceExchanger);
        else return NormalReload(ordnanceExchanger);
    }

    /// <summary>
    /// Returns the next stage to perform a Normal Reload
    /// </summary>
    protected abstract List<FirearmReloadInstruction> NormalReload(IFirearmOrdnanceExchanger ordnanceExchanger);
    /// <summary>
    /// Returns the next stage to perform a Fast Reload
    /// </summary>
    protected abstract List<FirearmReloadInstruction> FastReload(IFirearmOrdnanceExchanger ordnanceExchanger);
}