using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VORModLevelSelectCanvas : LevelSelectCanvas
{
    public bool sceneEnable = true;
    public float feedback = 1;

    public override void StartLevel()
    {
        if (level < 0)
        {
            return;
        }
        else
        {
            GameManager.SelectVORModLevel(sceneEnable, feedback, level);
        }
    }
}
