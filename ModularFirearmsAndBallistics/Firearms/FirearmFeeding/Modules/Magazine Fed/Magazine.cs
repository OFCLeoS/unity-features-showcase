using System.Collections.Generic;
using Blueprints;
using UnityEngine;

public class Magazine : Item
{
    /// <summary>
    /// REQUIRED
    /// </summary>
    [SerializeField] MagazineBlueprint magazineBlueprint;
    /// <summary>
    /// Optional if some sort of bullet preset wants to be loaded in this magazine.
    /// </summary>
    [SerializeField] MagazinePreset magazinePreset;

    MagazineType _magazineType;
    int _capacity;
    Stack<Bullet> magazineBullets;

    #region Initialization
    void Awake()
    {
        if (magazineBlueprint == null) Debug.LogError("A Magazine Blueprint was not found for " + name);
        else InitializeBlueprint();
    }

    void InitializeBlueprint()
    {
        _itemID = magazineBlueprint._itemID;
        _itemName = magazineBlueprint._itemName;
        _itemDescription = magazineBlueprint._itemDescription;
        _magazineType = magazineBlueprint._magazineType;
        _capacity = magazineBlueprint._capacity;
        magazineBullets = new Stack<Bullet>(_capacity);
        magazineBlueprint = null;
        if (magazinePreset == null) return;

        foreach (BulletBlock bulletBlock in magazinePreset.magazineComposition)
        {
            Bullet blockBullet = new Bullet(bulletBlock.bullet);
            for (int i = 0; i < bulletBlock.count; i++)
            {
                if (!TryInsertBullet(blockBullet))
                {
                    Debug.LogWarning("The Magazine Preset at " + name + " has more bullets than it's Blueprint's capacity.");
                    magazinePreset = null;
                    return;
                }
            }
        }
        magazinePreset = null;
    }
    #endregion

    /// <summary>
    /// </summary>
    /// <returns>True if bullet was inserted</returns>
    public bool TryInsertBullet(Bullet bullet)
    {
        if (GetMagazineBulletsCount() >= _capacity) return false;
        magazineBullets.Push(bullet);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <returns>True if bullet was gotten</returns>
    public bool TryGetTopBullet(out Bullet topBullet)
    {
        if (magazineBullets.TryPop(out topBullet))
        {
            return true;
        }
        else return false;
    }

    public bool IsEmpty() => GetMagazineBulletsCount() <= 0;

    public MagazineType GetMagazineType() => _magazineType;
    public int GetMagazineBulletsCount() => magazineBullets.Count;
}
