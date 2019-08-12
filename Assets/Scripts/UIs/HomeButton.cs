using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButton : VRInteractable {

    public override void OnClick()
    {
        base.OnClick();
        Stage.instance.GoHome();
    }
}
