using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction { None = -1, Horizontal, Vertical, LeftUp, RightUp }
public static class DirectionMethod
{
    public static Vector3 ToVector3(this Direction dir)
    {
        Vector3 vec = Vector3.zero;
        switch (dir)
        {
            case Direction.Horizontal:
                vec = new Vector3(1, 0, 0);
                break;
            case Direction.Vertical:
                vec = new Vector3(0, 1, 0);
                break;
            case Direction.LeftUp:
                vec = new Vector3(1, -1, 0);
                break;
            case Direction.RightUp:
                vec = new Vector3(1, 1, 0);
                break;
        }
        return vec.normalized;
    }
}

public abstract class Stage : MonoBehaviour {
    public static Stage instance;
    public static float[,] velocityPreset =
    {
        { 5, 10, 20, 40, 60, 80 },
        { 20, 40, 60, 80, 100, 120 },
        { 10, 20, 40, 80, 120, 160 },
        { 10, 20, 40, 80, 120, 160 }
    };

    protected enum State { Initial, Gaze, Wait, Move, End }
    protected State state = State.Initial;

    public int level;
    protected int cycle = 10;
    protected int currentCycle = 0;
    protected int successCycle = 0;
    [SerializeField]
    protected float angle;
    [SerializeField]
    protected float velocity;
    protected Direction directionEnum;
    protected Vector3 direction;
    protected bool right;
    [SerializeField]
    protected float gazeTime;
    protected float gazeTimer = 0;
    [SerializeField]
    protected float waitTime;
    protected float waitTimer = 0;
    protected float moveTime;
    protected float moveTimer = 0;

    protected bool coroutineCalled = false;
    protected float angleError = 0.2f; 
    protected float velocityError = 0.2f;
    protected float gain = 0;
    public bool[] goRight;
    public float[] headAngle;
    public float[] headVelocity;
    public float[] eyeAngle;
    public float[] eyeVelocity;
    public bool[] selected;
    
    protected GameObject guideCanvas;
    protected GameObject resultCanvas;
    protected Text guide;

    protected virtual void Init()
    {
        direction = directionEnum.ToVector3().normalized;
        velocity = velocityPreset[(int)GameManager.instance.mode, level];
        moveTime = angle / velocity;
        right = Random.value > 0.5f;

        guideCanvas = transform.Find("Guide Canvas").gameObject;
        resultCanvas = transform.Find("Result Canvas").gameObject;
        guide = transform.Find("Guide Canvas/Image/Guide Text").GetComponent<Text>();
        resultCanvas.SetActive(false);
    }
    public abstract void Init(Direction dir, int level);

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;
    }

    public void TargetGazeEnter()
    {
        if (state == State.Initial)
        {
            state = State.Gaze;
        }
    }
    public void TargetGazeExit()
    {
        if (state == State.Gaze)
        {
            state = State.Initial;
            gazeTimer = 0;
        }
    }

    public void GoHome()
    {
        UIManager.instance.gameObject.SetActive(true);
        Destroy(gameObject);
    }
    public virtual void Retry()
    {
        GameManager.SelectLevel(directionEnum, level);
    }
    public virtual void PrevStage()
    {
        if (level > 0)
        {
            GameManager.SelectLevel(directionEnum, level - 1);
        }
    }
    public virtual void NextStage()
    {
        if (level < velocityPreset.GetLength(1) - 1)
        {
            GameManager.SelectLevel(directionEnum, level + 1);
        }
    }
}
