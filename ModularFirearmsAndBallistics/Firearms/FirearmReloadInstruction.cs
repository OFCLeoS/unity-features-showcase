using System;

/// <summary>
/// A reload instruction contains a FirearmReloadStage (for animation) and an Action
/// </summary>
public struct FirearmReloadInstruction
{
    public FirearmReloadStage reloadStage;
    public Action reloadAction;

    public FirearmReloadInstruction(FirearmReloadStage reloadStage, Action reloadAction)
    {
        this.reloadStage = reloadStage;
        this.reloadAction = reloadAction;
    }
}