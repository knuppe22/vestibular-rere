using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButton : VRInteractable {

    public override void OnClick()
    {
        base.OnClick();
        Stage.instance.NextStage();
    }
}
