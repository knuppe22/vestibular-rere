using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : VRInteractable {

    public override void OnClick()
    {
        base.OnClick();
        Stage.instance.Retry();
    }
}
