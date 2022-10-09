using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpSpeed = 5f;
    public bool isTerranin = false;
    public int jumpCount = 2;  //а║га х╫╪Ж
    Rigidbody jumpRigidbody;
    Animator jumpAnimator;

    // Start is called before the first frame update
    void Start()
    {
        jumpRigidbody = GetComponent<Rigidbody>();
        jumpAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Terrain")
        {
            isTerranin = true;
            jumpCount = 2;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if (isTerranin&&jumpCount>0)
        //{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpAnimator.SetTrigger("Jump");
            jumpRigidbody.MovePosition(jumpRigidbody.position + transform.up * jumpSpeed * Time.deltaTime);
        }


        if (jumpAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
        {
            Time.timeScale = Mathf.Clamp(Time.time - 0.9f, 0.1f, 2f);
        }
         Time.timeScale = 1f; 
        //}

    }
}
