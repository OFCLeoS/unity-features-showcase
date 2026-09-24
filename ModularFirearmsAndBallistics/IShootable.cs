/// <summary>
/// Adds behaviour to an object when shot
/// </summary>
public interface IShootable
{
    /// <summary>
    /// Performs "I just got shot at" action from this object
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="penetrationPotential"></param>
    /// <param name="currentPenetrationPotential"></param>
    /// <returns>The Penetration Resistance of the material of this object</returns>
    public abstract float OnShot(float damage, float penetrationPotential, float currentPenetrationPotential);
}
