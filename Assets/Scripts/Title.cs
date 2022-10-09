using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public string sceneName = "GameScene";

    public void ClickStart()
    {
        Debug.Log("·Îµù");
        SceneManager.LoadScene(sceneName);
    }
}