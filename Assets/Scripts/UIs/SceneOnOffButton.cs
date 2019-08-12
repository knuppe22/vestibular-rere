using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOnOffButton : VRInteractable
{
    [SerializeField]
    private bool sceneEnable;

    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.SetSceneEnable(sceneEnable);
    }
}
