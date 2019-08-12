using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFeedbackSelectButton : VRInteractable
{
    [SerializeField]
    private float feedback;

    public override void OnClick()
    {
        base.OnClick();
        UIManager.instance.SetVRFeedback(feedback);
    }
}
