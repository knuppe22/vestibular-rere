using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaccadeStage : Stage {
    private Transform leftTarget;
    private Transform rightTarget;

    private Vector3 cameraPosition;
    private Vector3 baseVector;
    private Vector3 eyeVector;

    /*
    private GameObject guideCanvas;
    private GameObject lowerCanvas;
    private GameObject resultCanvas;
    private Text guide;
    private Text lowerGuide;
    */

    protected override void Init()
    {
        base.Init();

        leftTarget = transform.Find("Left Target");
        rightTarget = transform.Find("Right Target");

        goRight = new bool[2 * cycle];
        eyeAngle = new float[2 * cycle];
        eyeVelocity = new float[2 * cycle];
        selected = new bool[2 * cycle];
    }
    public override void Init(Direction dir, int level)
    {
        directionEnum = dir;
        this.level = level;
        Init();
    }

    protected override void Awake()
    {
        base.Awake();
    }
    
    // Update is called once per frame
    void Update () {
        switch (state)
        {
            case State.Gaze:
                gazeTimer += Time.deltaTime;

                if (gazeTimer > gazeTime) // Transition
                {
                    state = State.Wait;

                    GameManager.instance.HUDCanvas.SetActive(false);

                    cameraPosition = GameManager.instance.camera.position;
                    baseVector = leftTarget.position - cameraPosition;
                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }

                    leftTarget.position = leftTarget.position - baseVector.magnitude * Mathf.Tan(angle / 2 * Mathf.Deg2Rad) * direction;
                    rightTarget.position = rightTarget.position + baseVector.magnitude * Mathf.Tan(angle / 2 * Mathf.Deg2Rad) * direction;
                    right = !right;
                    SetAlpha();
                }
                break;
            case State.Wait:
                waitTimer += Time.deltaTime;

                //LowerGuideText();
                
                if (waitTimer >= waitTime - 5)
                {
                    guide.text = Mathf.Ceil(waitTime - waitTimer).ToString();
                }
                if (waitTimer > waitTime) // Transition
                {
                    state = State.Move;
                    startedTime = Time.time;

                    //Transform target = left ? rightTarget : leftTarget;

                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }

                    right = !right;
                    SetAlpha();
                    /*
                    guideCanvas.SetActive(false);
                    lowerCanvas.SetActive(false);
                    */
                    guideCanvas.SetActive(false);
                }
                break;
            case State.Move:
                moveTimer += Time.deltaTime;

                if (GameManager.instance.isGazing)
                {
                    WriteLog();
                }

                if (GameManager.instance.isGazing && !coroutineCalled && moveTimer > moveTime * 0.75)
                {
                    Vector3 to = (right ? rightTarget : leftTarget).position - cameraPosition;
                    StartCoroutine(GetBestValue(currentCycle, to));
                    coroutineCalled = true;
                }
                if (moveTimer > moveTime)
                {
                    /*
                    if (GameManager.instance.gazePointer != null)
                    {
                        Vector3 currentEyeVector = GameManager.instance.gazePointer.gameObject.transform.position - cameraPosition;
                        Check(currentEyeVector);
                        eyeVector = currentEyeVector;
                    }
                    if (currentCycle % 2 == 1)
                    {
                        // Do something between cycles
                    }
                    */
                    right = !right;
                    SetAlpha();

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

    void SetAlpha()
    {
        leftTarget.GetComponent<Renderer>().material.SetColor("_BaseColor", right ? new Color(0, 0, 0, 0.3f) : Color.white);
        rightTarget.GetComponent<Renderer>().material.SetColor("_BaseColor", right ? Color.white : new Color(0, 0, 0, 0.3f));
    }
    /*
    void LowerGuideText()
    {
        if (Vector3.Angle(baseVector, GameManager.instance.camera.forward) > maxError)
        {
            int horizontal = 0, vertical = 0;
            float errorRange = Mathf.Sin(maxError * Mathf.Deg2Rad) / Mathf.Sqrt(2);
            Vector3 rotatedVector = Quaternion.FromToRotation(GameManager.instance.camera.forward.normalized, Vector3.forward) * baseVector.normalized;
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
    IEnumerator GetBestValue(int curCycle, Vector3 to)
    {
        Vector3 bestEyeVector = Vector3.zero;
        if (curCycle == 2 * cycle - 1)
        {
            while ((curCycle == currentCycle && moveTimer > moveTime * 0.75f))
            {
                Vector3 currentEyeVector = GameManager.instance.gazeVector;
                if (bestEyeVector == Vector3.zero
                    || Vector3.Angle(currentEyeVector, to) < Vector3.Angle(bestEyeVector, to))
                    bestEyeVector = currentEyeVector;
                yield return null;
            }
            Check(curCycle, bestEyeVector, to);
            EndState();
        }
        else
        {
            while ((curCycle == currentCycle && moveTimer > moveTime * 0.75f)
                   || (curCycle == currentCycle - 1 && moveTimer < moveTime * 0.25f))
            {
                Vector3 currentEyeVector = GameManager.instance.gazeVector;
                if (bestEyeVector == Vector3.zero
                    || Vector3.Angle(currentEyeVector, to) < Vector3.Angle(bestEyeVector, to))
                    bestEyeVector = currentEyeVector;
                yield return null;
            }
            Check(curCycle, bestEyeVector, to);
            eyeVector = bestEyeVector;
        }
    }
    
    void Check(int curCycle, Vector3 bestEyeVector, Vector3 to)
    {
        goRight[curCycle] = right;
        eyeAngle[curCycle] = Vector3.Angle(eyeVector, bestEyeVector);
        eyeVelocity[curCycle] = eyeAngle[curCycle] / moveTime;
        bool goodAngle = Vector3.Angle(bestEyeVector, to) < angle * angleError;
        /*
        bool goodAngle = eyeAngle[currentCycle] > angle * (1 - angleError)
                         && eyeAngle[currentCycle] < angle * (1 + angleError);
        bool goodVelocity = eyeVelocity[currentCycle] > velocity * (1 - velocityError)
                            && eyeVelocity[currentCycle] < velocity * (1 + velocityError);
        */
        selected[curCycle] = goodAngle;
    }

    void EndState()
    {
        leftTarget.gameObject.SetActive(false);
        rightTarget.gameObject.SetActive(false);
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
    }
    
    void CalculateScore()
    {
        for(int i = 0; i < 2 * cycle; i++)
        {
            if (selected[i])
            {
                successCycle++;
                gain += eyeAngle[i] / angle;
            }
        }
        gain /= successCycle;
        gain = Mathf.Round(gain * 100) / 100;
    }
}
