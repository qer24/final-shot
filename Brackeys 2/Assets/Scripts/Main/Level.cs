using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public string levelName = "Level1";

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
