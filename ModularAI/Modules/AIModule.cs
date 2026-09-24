using UnityEngine;

/// <summary>
/// Base class for all AI Modules
/// </summary>
public class AIModule : MonoBehaviour
{
    protected AIModulesDatabase modulesDatabase;

    /// <summary>
    /// Should be called by AIModulesDatabase on Awake()
    /// </summary>
    public void InitializeModulesDatabase(AIModulesDatabase modulesDatabase) => this.modulesDatabase = modulesDatabase;
}