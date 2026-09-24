using System;
using System.Collections.Generic;
using UnityEngine;

public static class BallisticsSystem
{
    const int MAX_RAYCAST_HITS = 12;

    /// <summary>
    /// Pre-allocated array to avoid GC pressure. For use with Physics.RaycastNonAlloc
    /// </summary>
    static RaycastHit[] hitsCache = new RaycastHit[MAX_RAYCAST_HITS];

    static readonly IComparer<RaycastHit> raycastHitDistanceComparer =
        Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance));

    public static void FireBullet(Bullet bullet, Vector3 firingPosition, Vector3 firingDirection)
    {
        // Handles Bullet Path
        Debug.DrawRay(firingPosition, firingDirection * 1000, Color.red, 0.5f);//////////////DEBUG/////////////////////
        int hitCount = Physics.RaycastNonAlloc(firingPosition, firingDirection, hitsCache, 1000);
        if (hitCount > 0)
        {
            Array.Sort(hitsCache, 0, hitCount, raycastHitDistanceComparer);
            for (int i = 0; i < hitCount; i++)
            {
                // Debug.Log("Impacted " + hitsCache[i].transform.name);//////////////DEBUG////////////////////////

                //Shootables are used by objects that can be shot (Walls, people, doors, etc...)
                IShootable shootable = hitsCache[i].transform.GetComponent<IShootable>();
                if (shootable != null) bullet.penetrationPotential -= shootable.OnShot(bullet.Damage, bullet.penetrationPotential, bullet.penetrationPotential);

                if (bullet.penetrationPotential <= 0)//If the bullet has no more penetration potential, it stops
                {
                    //Debug.Log("NO MORE PEN POWER");/////////////////////DEBUG////////////////////////
                    break;
                }
            }
        }
    }
}