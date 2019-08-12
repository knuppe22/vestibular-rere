using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRInteractable : MonoBehaviour {
    public virtual void OnGazeEnter() { }
    public virtual void OnGazeStay() { }
    public virtual void OnGazeExit() { }
    public virtual void OnClick()
    {
        Selectable s = GetComponent<Selectable>();
        if (s != null)
        {
            s.Select();
        }
    }
}
