using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blueprints
{
    [CreateAssetMenu(fileName = "Magazine Blueprint", menuName = "Scriptable Objects/Magazine Blueprint")]
    public class MagazineBlueprint : ItemBlueprint
    {
        public MagazineType _magazineType;
        public int _capacity;
    }
}