using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : VRInteractable {
    
    public override void OnGazeEnter()
    {
        Stage.instance.TargetGazeEnter();
    }
    public override void OnGazeExit()
    {
        Stage.instance.TargetGazeExit();
    }
}
