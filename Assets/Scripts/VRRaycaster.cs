using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRRaycaster : MonoBehaviour {
    [SerializeField]
    private float distance = 100;

    private VRInteractable current = null;
    private VRInteractable previous = null;
    private VRInteractable selected = null;
    
    private Image gazePointer;

    // Use this for initialization
    void Start () {
        gazePointer = GameObject.Find("/HUD Canvas/Pointer").GetComponent<Image>();
    }
    
    // Update is called once per frame
    void Update () {
        previous = current;
        RaycastHit hit;
        Debug.DrawRay(transform.position, distance * transform.forward, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance)
            && hit.transform.GetComponent<VRInteractable>() != null)
        {
            //Debug.Log(hit.transform.gameObject.name);
            gazePointer.color = Color.green;
            current = hit.transform.GetComponent<VRInteractable>();

            if (previous != current)
            {
                current.OnGazeEnter();
            }
            else
            {
                current.OnGazeStay();
            }

            if (Input.GetButtonDown("Fire1"))
            {
                selected = current;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (selected == current)
                {
                    current.OnClick();
                }
                selected = null;
            }
        }
        else
        {
            gazePointer.color = Color.white;

            if (current != null)
            {
                current.OnGazeExit();
                current = null;
            }
            if (Input.GetButtonDown("Fire1"))
            {
                selected = null;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                selected = null;
            }
        }
    }
}
