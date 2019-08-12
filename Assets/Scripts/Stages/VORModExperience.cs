using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModExperience : MonoBehaviour
{
    public GameObject scene;

    void Awake()
    {
        scene = GameObject.Find("/Scene");
    }

    void Update()
    {

    }

    public void SceneOnOff(bool on)
    {
        scene.SetActive(on);
        GameManager.instance.camera.GetComponent<Camera>().clearFlags =
            on ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
    }
}
