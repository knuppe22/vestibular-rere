using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : VRInteractable {
    [SerializeField]
    private int level;

    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.SetLevel(level);
    }
}
