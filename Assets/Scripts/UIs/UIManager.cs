using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public GameObject modeSelect;
    public GameObject[] levelSelect;
    private GameObject currentLevelSelect = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        modeSelect = transform.Find("Mode Select").gameObject;
    }
    void OnEnable()
    {
        Debug.Log("Enter");
        ToModeSelect();
    }

    // Start is called before the first frame update
    void Start()
    {
        ToModeSelect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToModeSelect()
    {
        if (currentLevelSelect != null)
        {
            Destroy(currentLevelSelect);
            currentLevelSelect = null;
        }

        if (GameManager.instance == null) return;

        GameManager.instance.mode = Mode.None;
        modeSelect.SetActive(true);
    }
    public void ToLevelSelect(Mode mode)
    {
        modeSelect.SetActive(false);
        if (mode != Mode.None)
        {
            GameManager.instance.mode = mode;
            currentLevelSelect = Instantiate(levelSelect[(int)mode], transform);
        }
    }

    public void SetDirection(Direction dir)
    {
        currentLevelSelect.GetComponent<LevelSelectCanvas>().dir = dir;
    }
    public void SetLevel(int level)
    {
        currentLevelSelect.GetComponent<LevelSelectCanvas>().level = level;
    }
    public void SetSceneEnable(bool sceneEnable)
    {
        VORModLevelSelectCanvas canvas = currentLevelSelect.GetComponent<VORModLevelSelectCanvas>();
        if (canvas != null)
        {
            canvas.sceneEnable = sceneEnable;
        }
    }
    public void SetVRFeedback(float feedback)
    {
        VORModLevelSelectCanvas canvas = currentLevelSelect.GetComponent<VORModLevelSelectCanvas>();
        if (canvas != null)
        {
            canvas.feedback = feedback;
        }
    }
}
