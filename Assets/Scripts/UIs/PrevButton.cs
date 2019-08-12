using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevButton : VRInteractable {

    public override void OnClick()
    {
        base.OnClick();
        Stage.instance.PrevStage();
    }
}
