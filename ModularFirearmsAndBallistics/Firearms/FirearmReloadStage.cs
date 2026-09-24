public enum FirearmReloadStage : byte
{
    NONE = 0,

    /// <summary>
    /// Quickly take the ordnance out of the weapon (e.g. Drop the magazine on the floor)
    /// </summary>
    EJECT_ORDNANCE = 1,
    /// <summary>
    /// Grab the ordnance out of the weapon
    /// </summary>
    EXTRACT_ORDNANCE = 2,
    STOW_ORDNANCE = 3,
    /// <summary>
    /// Grabs ordnance out of an inventory
    /// </summary>
    GRAB_ORDNANCE = 4,
    INSERT_ORDNANCE = 5,

    RACK = 6,
    RELEASE_ACTION = 7,
}