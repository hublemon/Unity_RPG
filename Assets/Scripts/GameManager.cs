using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�̱��� ���ٿ� ������Ƽ
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

    private static GameManager m_instance;     //��Ŭ�� �Ҵ��
    public bool isGameOver { get; private set; }

    public Timer timer;


    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);   //�ٸ� ��Ŭ���� �ִٸ� �̰� ����
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
