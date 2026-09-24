using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        Debug.Log("PLAYER HAS DIED!!!");
    }
}
