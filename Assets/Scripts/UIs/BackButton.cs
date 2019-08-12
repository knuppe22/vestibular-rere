using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : VRInteractable
{
    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.ToModeSelect();
    }
}
