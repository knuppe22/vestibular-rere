using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperience : MonoBehaviour
{
    public GameObject scene;

    public bool background = true;
    public float feedback = 1f;
    public bool exit = false;

    void Awake()
    {
        scene = GameObject.Find("/Scene");
    }

    void Update()
    {
        if (exit)
        {
            GameManager.instance.EndVORModExperience();
            return;
        }

        SceneOnOff(background);
        GameManager.SetVRFeedback(feedback);
    }

    public void SceneOnOff(bool on)
    {
        scene.SetActive(on);
        GameManager.instance.camera.GetComponent<Camera>().clearFlags =
            on ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
    }
}
