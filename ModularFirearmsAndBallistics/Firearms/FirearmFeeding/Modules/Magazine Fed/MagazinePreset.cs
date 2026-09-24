using UnityEngine;

namespace Blueprints
{
    [CreateAssetMenu(fileName = "Magazine Preset", menuName = "Scriptable Objects/Magazine Preset")]
    public class MagazinePreset : ScriptableObject
    {
        [SerializeField] public BulletBlock[] magazineComposition;
    }
}