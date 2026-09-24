using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blueprints
{
    /// <summary>
    /// Blueprint of a bullet, not to be used in-game
    /// </summary>
    [CreateAssetMenu(fileName = "Bullet Blueprint", menuName = "Scriptable Objects/Bullet Blueprint")]
    public class BulletBlueprint : ItemBlueprint
    {
        public BulletCalibre _calibre;
        public float _damage;
        public float _penetrationPotential;
        public GameObject _bullet;
        public GameObject _shellCasing;
    }
}