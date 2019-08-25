using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperienceBackButton : VRInteractable
{
    public override void OnClick()
    {
        base.OnClick();
        GameManager.instance.experienceInstance.GetComponent<VORModExperience>().exit = true;
    }
}
