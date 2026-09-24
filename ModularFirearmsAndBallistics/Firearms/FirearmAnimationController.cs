using UnityEngine;

public class FirearmAnimationController : MonoBehaviour
{
    // int reloadTrigger = Animator.StringToHash("reload");
    // int tacReloadTrigger = Animator.StringToHash("tacReload");

    // public override void StartReloadAction()
    // {
    //     if (insertedMagazine == null || insertedMagazine.GetMagazineBulletsCount() <= 0)
    //     {
    //         firearmBase.GetFirearmHandler().GetPlayerAnimationManager().SetTrigger(reloadTrigger);
    //     }
    //     else
    //     {
    //         firearmBase.GetFirearmHandler().GetPlayerAnimationManager().SetTrigger(tacReloadTrigger);
    //     }
    // }

    // public override void EndReloadAction()
    // {
    //     TacticalRig ownerTacticalRig = firearmBase.GetFirearmHandler().GetTacticalRig();
    //     if (insertedMagazine == null || insertedMagazine.GetMagazineBulletsCount() <= 0) Reload(ownerTacticalRig);
    //     else TacticalReload(ownerTacticalRig);
    // }
}
