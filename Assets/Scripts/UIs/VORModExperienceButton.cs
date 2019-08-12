using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperienceButton : VRInteractable
{
    public override void OnClick()
    {
        base.OnClick();
        GameManager.instance.StartVORModExperience();
    }
}