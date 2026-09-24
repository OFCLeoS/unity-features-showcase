using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] int cellSize;
    Dictionary<Vector2Int, HashSet<ISoundReceptor>> soundReceptorGrid;
    Dictionary<ISoundReceptor, Vector2Int> movingSoundReceptorsPositions;

    #region Initialization
    void Awake() => InitializeSoundManager();

    void InitializeSoundManager()
    {
        soundReceptorGrid = new Dictionary<Vector2Int, HashSet<ISoundReceptor>>();
        movingSoundReceptorsPositions = new Dictionary<ISoundReceptor, Vector2Int>();
        GatherSoundReceptors();
    }
    #endregion

    void GatherSoundReceptors()
    {
        HashSet<ISoundReceptor> inspectedSet;
        foreach (ISoundReceptor soundReceptor in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<ISoundReceptor>())
        {
            int x = Mathf.FloorToInt(soundReceptor.GetTransform().position.x / cellSize);
            int z = Mathf.FloorToInt(soundReceptor.GetTransform().position.z / cellSize);
            Vector2Int gridPos = new Vector2Int(x, z);

            if (!soundReceptorGrid.TryGetValue(gridPos, out inspectedSet))
            {
                inspectedSet = new HashSet<ISoundReceptor>();
                soundReceptorGrid.Add(gridPos, inspectedSet);
            }
            inspectedSet.Add(soundReceptor);

            if (soundReceptor.Moves())
            {
                movingSoundReceptorsPositions.Add(soundReceptor, gridPos);
            }
        }
    }

    // Should be called once per X seconds, and only update Y/Z of receptors in each iteration
    void UpdateMovingSoundReceptors()
    {
        HashSet<ISoundReceptor> inspectedSet;

        foreach (ISoundReceptor soundReceptor in movingSoundReceptorsPositions.Keys.ToArray())
        {
            Vector3 soundReceptorWorldPos = soundReceptor.GetTransform().position;

            Vector2Int oldGridPos = movingSoundReceptorsPositions[soundReceptor];

            int x = Mathf.FloorToInt(soundReceptorWorldPos.x / cellSize);
            int z = Mathf.FloorToInt(soundReceptorWorldPos.z / cellSize);
            Vector2Int gridPos = new Vector2Int(x, z);

            // TODO: Is this really an optimization, or is it not worth it?
            if (gridPos != oldGridPos)
            {
                // Debug.Log("Sound receptor \"" + soundReceptor.GetTransform().name + "\" has moved from " + oldGridPos + " to " + gridPos + "...");
                soundReceptorGrid[oldGridPos].Remove(soundReceptor);
                if (!soundReceptorGrid.TryGetValue(gridPos, out inspectedSet))
                {
                    inspectedSet = new HashSet<ISoundReceptor>();
                    soundReceptorGrid.Add(gridPos, inspectedSet);
                }
                inspectedSet.Add(soundReceptor);
                movingSoundReceptorsPositions[soundReceptor] = gridPos;
            }
        }
    }

    // TODO: Ring Search can be generic, lots of repetition
    public void EmitSound(Vector3 soundEmitionPosition, float soundIntensity, string soundTag)
    {
        int x = Mathf.FloorToInt(soundEmitionPosition.x / cellSize);
        int z = Mathf.FloorToInt(soundEmitionPosition.z / cellSize);
        Vector2Int gridEmitionPosition = new Vector2Int(x, z);

        // Debug.Log("SoundManager: Emiting \"" + soundTag + "\" sound from " + soundEmitionPosition + " (A.K.A " + gridEmitionPosition + ") of intensity " + soundIntensity + "...");

        int soundRadius = Mathf.CeilToInt(soundIntensity / cellSize);

        HashSet<ISoundReceptor> inspectedSet;

        Vector2Int cellToInspect = gridEmitionPosition;
        for (int currentRadius = 0; currentRadius <= soundRadius; currentRadius++)
        {
            if (currentRadius == 0 && soundReceptorGrid.TryGetValue(cellToInspect, out inspectedSet))
            {
                foreach (ISoundReceptor soundReceptor in inspectedSet)
                {
                    soundReceptor.HearSound(soundEmitionPosition, soundIntensity, soundTag);
                }
            }
            else
            {
                // Top and Bottom sides
                for (int dx = -currentRadius; dx <= currentRadius; dx++)
                {
                    cellToInspect = gridEmitionPosition + new Vector2Int(dx, currentRadius);
                    // Top Side
                    if (soundReceptorGrid.TryGetValue(cellToInspect, out inspectedSet))
                    {
                        foreach (ISoundReceptor soundReceptor in inspectedSet)
                        {
                            soundReceptor.HearSound(soundEmitionPosition, soundIntensity, soundTag);
                        }
                    }
                    cellToInspect = gridEmitionPosition + new Vector2Int(dx, -currentRadius);
                    // Bottom Side
                    if (soundReceptorGrid.TryGetValue(cellToInspect, out inspectedSet))
                    {
                        foreach (ISoundReceptor soundReceptor in inspectedSet)
                        {
                            soundReceptor.HearSound(soundEmitionPosition, soundIntensity, soundTag);
                        }
                    }
                }

                // Left and Right sides (excluding corners)
                for (int dz = -currentRadius + 1; dz <= currentRadius - 1; dz++)
                {
                    cellToInspect = gridEmitionPosition + new Vector2Int(currentRadius, dz);
                    // Right Side
                    if (soundReceptorGrid.TryGetValue(cellToInspect, out inspectedSet))
                    {
                        foreach (ISoundReceptor soundReceptor in inspectedSet)
                        {
                            soundReceptor.HearSound(soundEmitionPosition, soundIntensity, soundTag);
                        }
                    }
                    cellToInspect = gridEmitionPosition + new Vector2Int(-currentRadius, dz);
                    // Left Side
                    if (soundReceptorGrid.TryGetValue(cellToInspect, out inspectedSet))
                    {
                        foreach (ISoundReceptor soundReceptor in inspectedSet)
                        {
                            soundReceptor.HearSound(soundEmitionPosition, soundIntensity, soundTag);
                        }
                    }
                }
            }
        }
    }

    float timeTillLast = 0;
    void Update()
    {
        // TODO: ACTUAL INTERVAL SYSTEM, THIS IS TEMPORARY DEBUGGING
        timeTillLast += Time.deltaTime;
        if (timeTillLast > 1)
        {
            UpdateMovingSoundReceptors();
            timeTillLast = 0;
        }
    }
}
