using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothPursuitStage : Stage {
    private Vector3 cameraPosition;
    private Vector3 baseVector;
    private Vector3 startTargetVector;
    private Vector3 endTargetVector;
    private Vector3 eyeVector;

    protected override void Init()
    {
        base.Init();

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
                    baseVector = transform.Find("Moving").position - cameraPosition;
                    startTargetVector = transform.Find("Moving").position - baseVector.magnitude * Mathf.Tan(angle / 2 * Mathf.Deg2Rad) * direction;
                    endTargetVector = transform.Find("Moving").position + baseVector.magnitude * Mathf.Tan(angle / 2 * Mathf.Deg2Rad) * direction;
                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }

                    transform.Find("Moving").position = right ? startTargetVector : endTargetVector;

                    //goalVector = initialVector.magnitude * new Vector3(Mathf.Tan(horizontalAngle * Mathf.Deg2Rad), Mathf.Tan(verticalAngle * Mathf.Deg2Rad), 0);
                    //goalGazeVector = initialVector.magnitude * new Vector3(Mathf.Tan(horizontalGazeAngle * Mathf.Deg2Rad), Mathf.Tan(verticalGazeAngle * Mathf.Deg2Rad), 0);
                    /*
                    guide.text = "고개를";
                    if (horizontalGazeAngle == 0 && verticalGazeAngle == 0)
                    {
                        guide.text += " 돌리지 말고";
                    }
                    else
                    {
                        guide.text += horizontalGazeAngle == 0 ? "" : (horizontalGazeAngle > 0 ? " 오른쪽" : " 왼쪽");
                        guide.text += verticalGazeAngle == 0 ? "으" : (verticalGazeAngle > 0 ? " 위" : " 아래");
                        guide.text += "로 돌리면서";
                    }
                    guide.text += "\n눈으로 물체를 따라가세요";
                    */
                }
                break;
            case State.Wait:
                waitTimer += Time.deltaTime;
                //MovingGuideText();
                
                if (waitTimer >= waitTime - 5)
                {
                    guide.text = Mathf.Ceil(waitTime - waitTimer).ToString();
                }
                if(waitTimer > waitTime) // Transition
                {
                    state = State.Move;

                    if (GameManager.instance.isGazing)
                    {
                        eyeVector = GameManager.instance.gazeVector;
                    }

                    guideCanvas.SetActive(false);
                }
                break;
            case State.Move:
                moveTimer += Time.deltaTime;
                Vector3 from = (right ? startTargetVector : endTargetVector);
                Vector3 to = (right ? endTargetVector : startTargetVector);
                transform.Find("Moving").position = from + (to - from) * (moveTimer / moveTime);
                //MovingGuideText();

                //Debug.DrawRay(cameraPosition, initialVector, Color.magenta);
                //Debug.DrawRay(cameraPosition, initialVector + goalVector, Color.cyan);
                if (GameManager.instance.isGazing && !coroutineCalled && moveTimer > moveTime * 0.75)
                {
                    Debug.Log(currentCycle + "th coroutine called");
                    StartCoroutine(GetBestValue(currentCycle, to - cameraPosition));
                    coroutineCalled = true;
                }
                if (moveTimer > moveTime)
                {
                    /*
                    if (GameManager.instance.gazePointer != null)
                    {
                        Vector3 currentEyeVector = GameManager.instance.gazePointer.gameObject.transform.position - cameraPosition;
                        Check(currentEyeVector, to);
                        eyeVector = currentEyeVector;
                    }
                    */
                    right = !right;
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
    void MovingGuideText()
    {
        Vector3 gazeForward = initialVector + goalGazeVector * (moveTimer / moveTime);
        if (Vector3.Angle(gazeForward, GameManager.instance.camera.forward) > maxError)
        {
            int horizontal = 0, vertical = 0;
            float errorRange = Mathf.Sin(maxError * Mathf.Deg2Rad) / Mathf.Sqrt(2);
            Vector3 rotatedVector = Quaternion.FromToRotation(GameManager.instance.camera.forward.normalized, Vector3.forward) * gazeForward.normalized;
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

            movingCanvas.SetActive(true);
            movingGuide.text = horizontal == 0 ? "" : (horizontal > 0 ? " 오른쪽" : " 왼쪽");
            movingGuide.text += vertical == 0 ? "으" : (vertical > 0 ? " 위" : " 아래");
            movingGuide.text += "로\n더 고개를 돌리세요";
        }
        else
        {
            movingCanvas.SetActive(false);
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
        Debug.Log(curCycle + " : " + eyeVector + ", " + bestEyeVector + ", " + to);
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
        transform.Find("Moving").gameObject.SetActive(false);
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
        for (int i = 0; i < 2 * cycle; i++)
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
