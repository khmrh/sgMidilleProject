using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameend : MonoBehaviour
{
    public void gotoGameEnd()
    {
        Application.Quit();

        //테스트용
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
