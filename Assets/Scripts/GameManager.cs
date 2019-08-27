using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using PupilLabs;

public enum Platform { None, GearVR, Vive };

public enum Mode
{
    None = -1,
    SmoothPursuit,
    Saccade,
    VOR,
    VORModulation
}

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    // Project & build setting
    public Platform platform;

    public new Transform camera;
    public GameObject HUDCanvas;

    public Vector3 localGazeVector;
    public Vector3 gazeVector;

    public GameObject[] stages;
    public Mode mode = Mode.None;
    public GameObject experienceStage;
    public GameObject experienceInstance;
    
    public bool isGazing = false;
    public float confidenceThreshold = 0.0f;

    public SubscriptionsController subscriptionsController;
    public CalibrationController calibrationController;
    GazeListener gazeListener = null;

    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        camera = GameObject.Find("/Player/Camera").transform;
        HUDCanvas = GameObject.Find("/HUD Canvas");
    }
    // Use this for initialization
    void Start () {
        if (platform == Platform.Vive)
        {
            GameObject.Find("/Player").transform.position = Vector3.zero;

            if (gazeListener == null)
            {
                gazeListener = new GazeListener(subscriptionsController);
            }

            gazeListener.OnReceive3dGaze += UpdateGazeVector;
            calibrationController.OnCalibrationStarted += StartCalibration;
            calibrationController.OnCalibrationSucceeded += StartTracking;
        }
        else if (platform == Platform.None)
        {
            XRSettings.enabled = false;
            camera.localPosition = Vector3.zero;
        }
    }
    
    // Update is called once per frame
    void Update () {
        if (SceneManager.GetActiveScene() != gameObject.scene)
            SceneManager.SetActiveScene(gameObject.scene);

        if (isGazing)
        {
            Debug.DrawRay(camera.position, gazeVector, Color.blue);
        }
    }

    public void StartCalibration()
    {
        isGazing = false;

        if (Stage.instance != null)
        {
            Destroy(Stage.instance.gameObject);
        }
    }

    public void StartTracking()
    {
        isGazing = true;
    }

    void UpdateGazeVector(GazeData gazeData)
    {
        if (gazeData.Confidence >= confidenceThreshold)
        {
            localGazeVector = gazeData.GazeDirection;
            gazeVector = camera.TransformDirection(localGazeVector);
        }
    }

    public void StartVORModExperience()
    {
        experienceInstance = Instantiate(experienceStage);
        UIManager.instance.gameObject.SetActive(false);
    }

    public void EndVORModExperience()
    {
        if (experienceInstance == null) return;

        experienceInstance.GetComponent<VORModExperience>().SceneOnOff(true);
        GameManager.SetVRFeedback(1);
        Destroy(experienceInstance);
        UIManager.instance.gameObject.SetActive(true);
    }
    
    public static void SelectLevel(Direction dir, int level)
    {
        Mode mode = GameManager.instance.mode;
        if (mode == Mode.None)
            return;
        GameObject stageInstance = Instantiate(instance.stages[(int)mode]);
        stageInstance.GetComponent<Stage>().Init(dir, level);
        UIManager.instance.gameObject.SetActive(false);
    }
    public static void SelectVORModLevel(bool sceneEnable, float feedback, int level)
    {
        Mode mode = GameManager.instance.mode;
        if (mode != Mode.VORModulation)
            return;
        GameObject stageInstance = Instantiate(instance.stages[(int)mode]);
        stageInstance.GetComponent<VORModStage>().Init(sceneEnable, feedback, level);
        UIManager.instance.gameObject.SetActive(false);
    }

    public static void SetVRFeedback(float value)
    {
        GameManager.instance.camera.GetComponent<CameraController>().VRFeedback = value;
    }
}
