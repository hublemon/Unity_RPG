using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public string SceneName = "GameScene";
    public void ReClickStart()
    {
        Debug.Log("�ε�");
        SceneManager.LoadScene(SceneName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
