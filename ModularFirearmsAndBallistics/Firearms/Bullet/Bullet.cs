using Blueprints;
using UnityEngine;

public struct Bullet
{
    BulletBlueprint bulletProfile;
    public float penetrationPotential;

    public Bullet(BulletBlueprint bulletProfile)
    {
        this.bulletProfile = bulletProfile;
        penetrationPotential = bulletProfile._penetrationPotential;
    }

    public float Damage { get => bulletProfile._damage; }
    public GameObject BulletObject { get => bulletProfile._bullet; }
    public GameObject ShellCasingObject { get => bulletProfile._shellCasing; }
}