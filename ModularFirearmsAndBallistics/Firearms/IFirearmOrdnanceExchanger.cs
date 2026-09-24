using System;
using InventoryFilters;

/// <summary>
/// Handles the exchange of Firearms Ordnance. (Example: Getting a Magazine out of a rifle, putting it in a Tactical Rig and grabbing a new magazine and use it on the rifle.)
/// </summary>
public interface IFirearmOrdnanceExchanger
{
    #region Firearm Transfers
    /// <summary>
    /// Does the Exchanger contain the Ordnance
    /// </summary>
    public int ContainsOrdnance<Ordnance>(IItemFilter<Ordnance> ordenanceFilter) where Ordnance : Item;
    /// <summary>
    /// Grabs ordnance by an Index and holds it for future taking by the Firearm
    /// </summary>
    /// <returns>True if ordnance is available</returns>
    public bool RequestOrdnance<Ordnance>(int itemIndex) where Ordnance : Item;
    public Ordnance TakeOrdnance<Ordnance>() where Ordnance : Item; //TODO: Possibly need to check the type of the ordnace? (Prolly not)
    #endregion

    #region Inventory Transfers
    public bool CanGiveOrdnance<Ordnance>(Ordnance ordnance) where Ordnance : Item;
    /// <summary>
    /// Grabs ordnance and holds it for possible future taking by the exchanger inventory
    /// </summary>
    public void HoldOrdnance<Ordnance>(Ordnance ordnance) where Ordnance : Item;
    /// <summary>
    /// Stows the ordnance held by the exchanger in the inventory
    /// </summary>
    public bool StowOrdnance<Ordnance>() where Ordnance : Item;
    #endregion

    public void InterruptExchange();
}