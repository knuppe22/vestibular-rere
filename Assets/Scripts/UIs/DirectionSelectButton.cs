using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSelectButton : VRInteractable
{
    [SerializeField]
    private Direction dir;

    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.SetDirection(dir);
    }
}
