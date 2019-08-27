using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VORModStage : Stage {
    private bool sceneEnable;
    private float vorRate;
    private GameObject scene;
    /*
    [SerializeField]
    private AudioClip[] sounds;
    private AudioSource audioSource;
    */
    private Vector3 cameraPosition;
    private Vector3 baseVector;
    private Vector3 headVector;
    private Vector3 eyeVector;
    private Vector3 cameraForward;

    [SerializeField]
    private GameObject HUDMarkers;
    private GameObject markerInstance;
    private Image[] markers;
    private Transform currentMarker;

    protected override void Init()
    {
        base.Init();

        if (!sceneEnable)
        {
            scene = GameObject.Find("/Scene");
            scene.SetActive(false);
            GameManager.instance.camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        }
        GameManager.SetVRFeedback(vorRate);

        //audioSource = GameManager.instance.camera.GetComponent<AudioSource>();
        markerInstance = Instantiate(HUDMarkers, GameManager.instance.camera);
        markers = new Image[2];
        markers[0] = markerInstance.transform.Find("Left/Image").GetComponent<Image>();
        markers[1] = markerInstance.transform.Find("Right/Image").GetComponent<Image>();
        currentMarker = markerInstance.transform.Find("Current");
        markers[0].gameObject.SetActive(false);
        markers[1].gameObject.SetActive(false);
        currentMarker.gameObject.SetActive(false);

        goRight = new bool[2 * cycle];
        headAngle = new float[2 * cycle];
        headVelocity = new float[2 * cycle];
        eyeAngle = new float[2 * cycle];
        eyeVelocity = new float[2 * cycle];
        selected = new bool[2 * cycle];
    }
    public override void Init(Direction dir, int level)
    {
        GoHome();
    }
    public void Init(bool sceneEnable, float feedback, int level)
    {
        this.sceneEnable = sceneEnable;
        vorRate = feedback;
        this.level = level;
        Init();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Gaze:
                gazeTimer += Time.deltaTime;

                if (gazeTimer > gazeTime) // Transition
                {
                    state = State.Wait;

                    GameManager.instance.HUDCanvas.SetActive(false);
                    
                    cameraPosition = GameManager.instance.camera.position;
                    baseVector = transform.Find("Target").position - cameraPosition;
                    headVector = GameManager.instance.camera.forward;
                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }

                    right = !right;
                    SetMarkerAlpha();
                    currentMarker.localEulerAngles = new Vector3(0, (right ? -1 : 1) * angle / 2, 0);
                    markers[0].gameObject.SetActive(true);
                    markers[1].gameObject.SetActive(true);
                    currentMarker.gameObject.SetActive(true);
                }
                break;
            case State.Wait:
                waitTimer += Time.deltaTime;
                
                if (waitTimer >= waitTime - 5)
                {
                    guide.text = Mathf.Ceil(waitTime - waitTimer).ToString();

                }
                if (waitTimer > waitTime) // Transition
                {
                    state = State.Move;
                    startedTime = Time.time;

                    headVector = GameManager.instance.camera.forward;
                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }
                    
                    right = !right;
                    // PlayAudio();
                    SetMarkerAlpha();
                    guideCanvas.SetActive(false);
                }
                break;
            case State.Move:
                moveTimer += Time.deltaTime;

                if (GameManager.instance.isGazing)
                {
                    WriteLog();
                }
                /*
                cameraForward = baseVector + (left ? -1 : 1) * (-goalCameraVector + 2 * goalCameraVector * (moveTimer / interval));
                Debug.DrawRay(GameManager.instance.camera.position, cameraForward);
                currentMarker.localRotation = Quaternion.FromToRotation(cameraForward, initialVector);
                Debug.DrawRay(cameraPosition, cameraForward);
                */
                /*
                if (moveCounter < tutorialCount)    // Tutorial
                    LowerGuideText();
                else
                    lowerCanvas.SetActive(false);
                */
                float rotationY = (right ? -1 : 1) * (-angle / 2 + angle * (moveTimer / moveTime));
                currentMarker.localEulerAngles = new Vector3(0, rotationY, 0);

                if (GameManager.instance.isGazing && !coroutineCalled && moveTimer > moveTime * 0.75)
                {
                    StartCoroutine(GetBestValue(currentCycle));
                    coroutineCalled = true;
                }
                if (moveTimer > moveTime)
                {
                    /*
                    if (GameManager.instance.gazePointer != null)
                    {
                        Vector3 currentHeadVector = GameManager.instance.camera.forward;
                        Vector3 currentEyeVector = GameManager.instance.gazePointer.gameObject.transform.position - cameraPosition;
                        Check(currentHeadVector, currentEyeVector);
                        headVector = currentHeadVector;
                        eyeVector = currentEyeVector;
                    }
                    */
                    right = !right;
                    // PlayAudio();
                    SetMarkerAlpha();

                    moveTimer = 0;
                    coroutineCalled = false;
                    currentCycle++;
                }
                
                if (currentCycle >= 2 * cycle) // Transition
                {
                    state = State.End;
                    if (!GameManager.instance.isGazing)
                    {
                        EndState();
                    }
                }
                break;
        }
    }
    /*
    void PlayAudio()
    {
        audioSource.time = 0;
        audioSource.clip = sounds[right ? 1 : 0];
        audioSource.Play();
    }
    */
    void SetMarkerAlpha()
    {
        markers[right ? 0 : 1].color = new Color(1, 1, 0, 150f / 255f);
        markers[right ? 1 : 0].color = Color.clear;
    }
    /*
    void LowerGuideText()
    {
        if (Vector3.Angle(cameraForward, GameManager.instance.camera.forward) > maxError)
        {
            int horizontal = 0, vertical = 0;
            float errorRange = Mathf.Sin(maxError * Mathf.Deg2Rad) / Mathf.Sqrt(2);
            Vector3 rotatedVector = Quaternion.FromToRotation(GameManager.instance.camera.forward.normalized, Vector3.forward) * cameraForward.normalized;
            if (rotatedVector.x >= errorRange)
            {
                horizontal = 1;
            }
            else if (rotatedVector.x <= -errorRange)
            {
                horizontal = -1;
            }
            if (rotatedVector.y >= errorRange)
            {
                vertical = 1;
            }
            else if (rotatedVector.y <= -errorRange)
            {
                vertical = -1;
            }

            lowerCanvas.SetActive(true);
            lowerGuide.text = horizontal == 0 ? "" : (horizontal > 0 ? " 오른쪽" : " 왼쪽");
            lowerGuide.text += vertical == 0 ? "으" : (vertical > 0 ? " 위" : " 아래");
            lowerGuide.text += "로\n더 고개를 돌리세요";
        }
        else
        {
            lowerCanvas.SetActive(false);
        }
    }
    */
    IEnumerator GetBestValue(int curCycle)
    {
        Vector3 bestEyeVector = Vector3.zero;
        Vector3 bestHeadVector = Vector3.zero;
        if (curCycle == 2 * cycle - 1)
        {
            while ((curCycle == currentCycle && moveTimer > moveTime * 0.75f))
            {
                Vector3 currentHeadVector = GameManager.instance.camera.forward;
                Vector3 currentEyeVector = GameManager.instance.gazeVector;
                if (bestHeadVector == Vector3.zero
                    || Vector3.Angle(currentHeadVector, baseVector) > Vector3.Angle(bestHeadVector, baseVector))
                    bestHeadVector = currentHeadVector;
                if (bestEyeVector == Vector3.zero
                    || Vector3.Angle(currentEyeVector, baseVector) < Vector3.Angle(bestEyeVector, baseVector))
                    bestEyeVector = currentEyeVector;
                yield return null;
            }
            Check(curCycle, bestHeadVector, bestEyeVector);
            EndState();
        }
        else
        {
            while ((curCycle == currentCycle && moveTimer > moveTime * 0.75f)
                   || (curCycle == currentCycle - 1 && moveTimer < moveTime * 0.25f))
            {
                Vector3 currentHeadVector = GameManager.instance.camera.forward;
                Vector3 currentEyeVector = GameManager.instance.gazeVector;
                if (bestHeadVector == Vector3.zero
                    || Vector3.Angle(currentHeadVector, baseVector) > Vector3.Angle(bestHeadVector, baseVector))
                    bestHeadVector = currentHeadVector;
                if (bestEyeVector == Vector3.zero
                    || Vector3.Angle(currentEyeVector, baseVector) < Vector3.Angle(bestEyeVector, baseVector))
                    bestEyeVector = currentEyeVector;
                yield return null;
            }
            Check(curCycle, bestHeadVector, bestEyeVector);
            headVector = bestHeadVector;
            eyeVector = bestEyeVector;
        }
    }

    void Check(int curCycle, Vector3 bestHeadVector, Vector3 bestEyeVector)
    {
        goRight[curCycle] = right;
        headAngle[curCycle] = Vector3.Angle(headVector, bestHeadVector);
        headVelocity[curCycle] = headAngle[curCycle] / moveTime;
        eyeAngle[curCycle] = Vector3.Angle(eyeVector, bestEyeVector);
        eyeVelocity[curCycle] = eyeAngle[curCycle] / moveTime;
        bool goodAngle = Vector3.Angle(baseVector, bestHeadVector) > angle / 2 * (1 - angleError)
                         && Vector3.Angle(baseVector, bestEyeVector) < angle * angleError;
        /*
        bool goodAngle = headAngle[currentCycle] > angle * (1 - angleError)
                         && headAngle[currentCycle] < angle * (1 + angleError);
        bool goodVelocity = headVelocity[currentCycle] > velocity * (1 - velocityError)
                         && headVelocity[currentCycle] < velocity * (1 + velocityError);
        */
        selected[curCycle] = goodAngle;
    }

    void EndState()
    {
        Destroy(markerInstance);
        // audioSource.Stop();
        transform.Find("Target").gameObject.SetActive(false);
        GameManager.instance.HUDCanvas.SetActive(true);
        resultCanvas.SetActive(true);

        transform.Find("Result Canvas/Text").GetComponent<Text>().text = "Level " + (level + 1) + " 완료";
        if (GameManager.instance.isGazing)
        {
            CalculateScore();
            transform.Find("Result Canvas/Text").GetComponent<Text>().text += "(" + successCycle + "/" + 2 * cycle + ")";
            transform.Find("Result Canvas/Score").GetComponent<Text>().text = "Score: " + gain;
        }
        else
        {
            transform.Find("Result Canvas/Score").GetComponent<Text>().text = "";
        }

        GameManager.SetVRFeedback(1);
        if (!sceneEnable)
        {
            scene.SetActive(true);
            GameManager.instance.camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        }
    }

    void CalculateScore()
    {
        for (int i = 0; i < 2 * cycle; i++)
        {
            if (selected[i])
            {
                successCycle++;
                gain += 1 - ((Mathf.Abs(headAngle[i] - angle) + Mathf.Abs(eyeAngle[i]))) / 2 / angle;
            }
        }
        gain /= successCycle;
        gain = Mathf.Round(gain * 100) / 100;
    }

    public override void Retry()
    {
        GameManager.SelectVORModLevel(sceneEnable, vorRate, level);
    }
    public override void PrevStage()
    {
        if (level > 0)
        {
            GameManager.SelectVORModLevel(sceneEnable, vorRate, level - 1);
        }
    }
    public override void NextStage()
    {
        if (level < velocityPreset.GetLength(1) - 1)
        {
            GameManager.SelectVORModLevel(sceneEnable, vorRate, level + 1);
        }
    }
}
