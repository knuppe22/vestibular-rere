using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperienceBackgroundButton : VRInteractable
{
    [SerializeField]
    private bool on;

    public override void OnClick()
    {
        base.OnClick();
        GameManager.instance.experienceInstance.GetComponent<VORModExperience>().background = on;
    }
}
