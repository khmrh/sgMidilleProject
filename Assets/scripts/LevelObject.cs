using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObject : MonoBehaviour
{
    public string nextLevel;

    public void MoveToNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
