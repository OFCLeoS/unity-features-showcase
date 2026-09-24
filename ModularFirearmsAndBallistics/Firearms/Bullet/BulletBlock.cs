using System;

namespace Blueprints
{
    /// <summary>
    /// A bullet block. To be used for blueprint creations
    /// </summary>
    [Serializable]
    public struct BulletBlock
    {
        public BulletBlueprint bullet;
        public int count;
    }
}