using Unity.VisualScripting;
using UnityEngine;

public interface ISoundReceptor
{
    public bool Moves();
    public void HearSound(Vector3 soundEmitionPosition, float soundIntensity, string soundTag);

    public Transform GetTransform() => ((MonoBehaviour)this).transform;
        
}
