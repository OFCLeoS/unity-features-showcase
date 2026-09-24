using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

/// <summary>
/// AI Controller Module that allows an AI Agent to manipulate Firearms.
/// </summary>
[RequireComponent(typeof(AIFirearmReloadHandler))]
public class AIFirearmsController : AIModule
{
    #region Blackboard References
    SerializableGUID hasAmmoBoolGUID;
    #endregion

    AIRigController rigController;
    AIFirearmReloadHandler reloadHandler;

    BehaviorGraphAgent agentBehaviourTree;

    // Used to not be constantly modifying the blackboard variables
    bool previouslyFired = false;

    [SerializeField] FirearmBase equippedFirearm;
    public FirearmBase EquippedFirearm => equippedFirearm;

    #region Initialization
    void Start()
    {
        agentBehaviourTree = modulesDatabase.Brain.GetAgentBehaviourTree();
        if (!agentBehaviourTree.GetVariableID("Has Ammo", out hasAmmoBoolGUID))
        {
            throw new BlackboardVariableNotFoundException("Has Ammo");
        }

        rigController = modulesDatabase.GetModule<AIRigController>();

        reloadHandler = GetComponent<AIFirearmReloadHandler>();
        reloadHandler.InitializeReloadHandler(hasAmmoBoolGUID, agentBehaviourTree);

        if (equippedFirearm != null && equippedFirearm.Feeding.HasOneInTheChamber)
        {
            agentBehaviourTree.SetVariableValue(hasAmmoBoolGUID, true);
        }
        else
        {
            agentBehaviourTree.SetVariableValue(hasAmmoBoolGUID, false);
        }
    }
    #endregion

    void ApplyRecoil(float xRotation, float yRotation)
    {
        Quaternion rotation = Quaternion.Euler(equippedFirearm.Firing.BarrelExit.eulerAngles.x - yRotation, equippedFirearm.Firing.BarrelExit.eulerAngles.y + xRotation, 0);
        Quaternion targetBodyRotation = rotation * Quaternion.Inverse(equippedFirearm.Firing.BarrelExit.localRotation);

        Vector3 eulerRotation = targetBodyRotation.eulerAngles;

        rigController.SetUpperBodyRotation(eulerRotation.x, eulerRotation.y);
    }

    public void StartReload()
    {
        reloadHandler.StartReload(false, equippedFirearm.Feeding);
    }

    public bool AttemptToFire()
    {
        bool bulletFired = false;

        float xRecoil;
        float yRecoil;
        if (equippedFirearm.Firing.FireAction(out xRecoil, out yRecoil))
        {
            ApplyRecoil(xRecoil, yRecoil);
            bulletFired = true;
            // TODO: RECOIL DEPENDS ON SKILL!
            // TODO: ADD MUZZLE DISCIPLINE
            previouslyFired = true;
        }

        equippedFirearm.Firing.ReleaseTriggerAction();
        if (!bulletFired && previouslyFired && !equippedFirearm.Feeding.HasOneInTheChamber)
        {
            previouslyFired = false;
            // TODO: RN THE ONLY REASON FOR THIS IS NO AMMO, IMPLEMENT JAMMING AS WELL!!!
            agentBehaviourTree.SetVariableValue(hasAmmoBoolGUID, false);
        }
        return bulletFired;
    }
}