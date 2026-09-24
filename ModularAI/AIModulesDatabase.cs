using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database for all AI Agent Modules
/// </summary>
public class AIModulesDatabase : MonoBehaviour
{
    // The Brain is a special module
    public AIBrain Brain { get; private set; }
    readonly Dictionary<Type, AIModule> _typeAIModuleKeyValuePairs = new Dictionary<Type, AIModule>();

    [SerializeField] Transform modulesParent;

    #region Initialization
    void Awake() => GatherModules();

    void GatherModules()
    {
        Brain = GetComponent<AIBrain>();
        if (Brain == null) Brain = modulesParent.GetComponentInChildren<AIBrain>();

        foreach (AIModule module in GetComponents<AIModule>())
        {
            _typeAIModuleKeyValuePairs.Add(module.GetType(), module);
            module.InitializeModulesDatabase(this);
        }
        foreach (AIModule module in modulesParent.GetComponentsInChildren<AIModule>(true))
        {
            _typeAIModuleKeyValuePairs.Add(module.GetType(), module);
            module.InitializeModulesDatabase(this);
        }
    }
    #endregion

    /// <summary>
    /// O(1) retrieval on average
    /// </summary>
    /// <typeparam name="T">The Module to get</typeparam>
    /// <returns>The Module if in the Database</returns>
    public T GetModule<T>() where T : AIModule
    {
        return (T)_typeAIModuleKeyValuePairs[typeof(T)];
    }

    /// <summary>
    /// O(1) retrieval on average
    /// </summary>
    /// <typeparam name="T">The Module to get</typeparam>
    /// <param name="module">Where the module will be stored</param>
    /// <returns>True if a module was successfully retrieved. False otherwise</returns>
    public bool TryGetModule<T>(out T module) where T : AIModule
    {
        if (_typeAIModuleKeyValuePairs.TryGetValue(typeof(T), out AIModule value))
        {
            module = (T)value;
            return true;
        }

        module = null;
        return false;
    }
}
