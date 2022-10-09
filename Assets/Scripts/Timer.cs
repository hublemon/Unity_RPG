using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float time=180.0f;
    TextMeshProUGUI TimerText;

    public UnityEvent GameOver;


    // Start is called before the first frame update
    void Start()
    {
        TimerText=GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        TimerText.text = "Time: " + time.ToString("F1") + "s";
        if (time <= 0f)
        {
            time = 0f;
            GameOver.Invoke();
        }
    }
}
