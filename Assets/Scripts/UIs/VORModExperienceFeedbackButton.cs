using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperienceFeedbackButton : VRInteractable
{
    [SerializeField]
    private float feedback;

    public override void OnClick()
    {
        base.OnClick();
        GameManager.SetVRFeedback(feedback);
    }
}
