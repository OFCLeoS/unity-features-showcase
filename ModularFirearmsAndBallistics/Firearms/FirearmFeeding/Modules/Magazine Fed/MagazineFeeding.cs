using System;
using System.Collections.Generic;
using InventoryFilters;
using UnityEngine;

public class MagazineFeeding : FirearmFeeding
{
    [SerializeField] MagazineType compatibleMagazineType;
    [Tooltip("Optional")]
    [SerializeField] Magazine startingMagazine;

    Magazine insertedMagazine;
    /// <summary>
    /// Filter used to check if exchanger can obtain a magazine compatible with this feeding system
    /// </summary>
    MagazineFilter magazineFilter;

    #region Initialization
    protected override void Awake()
    {
        if(startingMagazine != null)
        {
            InsertMagazine(startingMagazine);
            ChamberBullet();
            startingMagazine = null;
        }
        magazineFilter = new MagazineFilter(compatibleMagazineType);
        base.Awake();
    }
    #endregion

    /// <summary>
    /// </summary>
    /// <returns>The Removed Magazine</returns>
    public Magazine RemoveMagazine()
    {
        Magazine tempRef = insertedMagazine;
        insertedMagazine = null;
        return tempRef;
    }

    /// <summary>
    /// </summary>
    /// <returns>True if Successful Insertion</returns>
    public bool InsertMagazine(Magazine magazine)
    {
        if (magazine.GetMagazineType() == compatibleMagazineType && insertedMagazine == null)
        {
            insertedMagazine = magazine;
            return true;
        }
        else return false;
    }

    public override bool ChamberBullet()
    {
        if (chamberedBullet != null) base.ChamberEject(); //Bullet could be null if player is just chambering due to a normal reload
        if (insertedMagazine.TryGetTopBullet(out Bullet nextBullet))
        {
            chamberedBullet = nextBullet;
            return true;
        }
        return false;
    }

    // TODO: FIX STAGED RELOAD LOGIC!!
    protected override List<FirearmReloadInstruction> NormalReload(IFirearmOrdnanceExchanger ordnanceExchanger)
    {
        reloadInstructionsCache.Clear();
        int ordnaceIndex = ordnanceExchanger.ContainsOrdnance<Magazine>(magazineFilter);
        if (ordnaceIndex != -1)
        {
            if (insertedMagazine != null)
            {
                if (!isReloading)
                {
                    Debug.Log("RELOAD INSTRUCTIONS: Extract -> Stow -> Grab -> Insert");
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.EXTRACT_ORDNANCE, () => ordnanceExchanger.HoldOrdnance<Magazine>(RemoveMagazine())));
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.STOW_ORDNANCE, () => ordnanceExchanger.StowOrdnance<Magazine>()));
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.GRAB_ORDNANCE, () => ordnanceExchanger.RequestOrdnance<Magazine>(ordnaceIndex)));
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.INSERT_ORDNANCE, () => InsertMagazine(ordnanceExchanger.TakeOrdnance<Magazine>())));
                }
            }
            else
            {
                Debug.Log("RELOAD INSTRUCTIONS: Grab -> Insert");
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.GRAB_ORDNANCE, () => ordnanceExchanger.RequestOrdnance<Magazine>(ordnaceIndex)));
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.INSERT_ORDNANCE, () => InsertMagazine(ordnanceExchanger.TakeOrdnance<Magazine>())));
            }

            if (chamberedBullet == null)
            {
                Debug.Log("RELOAD INSTRUCTIONS: Release");
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.RELEASE_ACTION, () => ChamberBullet()));
            }
            return reloadInstructionsCache;
        }
        Debug.Log("No magazine in inventory");
        // If code reaches here, a reload action cannot be performed
        return reloadInstructionsCache;
    }

    protected override List<FirearmReloadInstruction> FastReload(IFirearmOrdnanceExchanger ordnanceExchanger)
    {
        reloadInstructionsCache.Clear();
        int ordnaceIndex = ordnanceExchanger.ContainsOrdnance<Magazine>(magazineFilter);
        if (ordnaceIndex != -1)
        {
            if (insertedMagazine != null)
            {
                if (!isReloading)
                {
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.EJECT_ORDNANCE, () => RemoveMagazine()));
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.GRAB_ORDNANCE, () => ordnanceExchanger.RequestOrdnance<Magazine>(ordnaceIndex)));
                    reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.INSERT_ORDNANCE, () => InsertMagazine(ordnanceExchanger.TakeOrdnance<Magazine>())));
                }
            }
            else
            {
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.GRAB_ORDNANCE, () => ordnanceExchanger.RequestOrdnance<Magazine>(ordnaceIndex)));
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.INSERT_ORDNANCE, () => InsertMagazine(ordnanceExchanger.TakeOrdnance<Magazine>())));
            }

            if (chamberedBullet == null)
            {
                reloadInstructionsCache.Add(new FirearmReloadInstruction(FirearmReloadStage.RELEASE_ACTION, () => ChamberBullet()));
            }
            return reloadInstructionsCache;
        }
        // If code reaches here, a reload action cannot be performed
        return reloadInstructionsCache;
    }
}