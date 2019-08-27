using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeMarker : MonoBehaviour
{
    void Update()
    {
        if (GameManager.instance.isGazing)
        {
            transform.localPosition = 0.75f * GameManager.instance.localGazeVector;
        }
    }
}
