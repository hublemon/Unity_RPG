using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //싱글턴 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    private static GameManager m_instance;     //싱클턴 할당됨
    public bool isGameOver { get; private set; }

    public Timer timer;


    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);   //다른 싱클톤이 있다면 이걸 삭제
        }
    }

    public void EndGame()
    {
        isGameOver = true;
        SceneManager.LoadScene("EndingScene");
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
