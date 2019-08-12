using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectButton : VRInteractable {
    [SerializeField]
    private Mode mode;

    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.ToLevelSelect(mode);
    }
}
