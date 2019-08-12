using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : VRInteractable
{
    public override void OnClick()
    {
        base.OnClick();
        transform.parent.parent.GetComponent<LevelSelectCanvas>().StartLevel();
    }
}
