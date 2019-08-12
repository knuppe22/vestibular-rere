using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCanvas : MonoBehaviour
{
    public int level = -1;
    public Direction dir = Direction.None;

    public virtual void StartLevel()
    {
        if (level < 0 || dir == Direction.None)
        {
            return;
        }
        else
        {
            GameManager.SelectLevel(dir, level);
        }
    }
}
