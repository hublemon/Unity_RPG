using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttack : MonoBehaviour
{
    List<string> attacks = new List<string>();

    Animator attackAnimator;

    GameObject monsterEntity;


    public GameObject Effect1;
    public GameObject Effect2;
    public GameObject Effect3;

    public LivingEntity monster;

    RaycastHit hit;


    //���콺���� ��ư���� ����
    //�ִϸ��̼� ���º��� �ٸ� ���� ������
    // Start is called before the first frame update
    void Start()
    {
        attacks.Add("Attack1");
        attacks.Add("Attack2");
        attacks.Add("Attack3");
        attackAnimator = GetComponent<Animator>();
        monsterEntity = FindObjectOfType<MosterController>().gameObject;
        monster = monsterEntity.GetComponent<LivingEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        Hit();
    }

    public void StartAttack()    //F������ �⺻����(playerMovement)

        //�ͻ�� ��Ų ���� ���������� �̺�Ʈ�Լ������ѵ�, �ƽ���
    {
        if (attackAnimator.GetCurrentAnimatorStateInfo(0).IsName(attacks[0]) && attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f+Time.deltaTime &&
                attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            Debug.Log(attacks[0]);
            GameObject effect1=Instantiate(Effect1, hit.transform);
            effect1.transform.position += effect1.transform.up*1.5f;
            Destroy(effect1, 1.4f);
            monster.TakeHit(30, hit.point);


        }
        if (attackAnimator.GetCurrentAnimatorStateInfo(0).IsName(attacks[1]) && attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f + Time.deltaTime &&
                attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            Debug.Log(attacks[1]);
            GameObject effect2=Instantiate(Effect2, hit.transform);
            effect2.transform.position += effect2.transform.up*1.5f;
            Destroy(effect2, 1.4f);
            monster.TakeHit(50, hit.point);

        }
        if (attackAnimator.GetCurrentAnimatorStateInfo(0).IsName(attacks[2]) && attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f + Time.deltaTime &&
                attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            Debug.Log(attacks[2]);
            GameObject effect3=Instantiate(Effect3, hit.transform);
            effect3.transform.position += effect3.transform.up*1.5f;
            Destroy(effect3, 1.4f);
            monster.TakeHit(70, hit.point);
        }
    }
    void Hit()
    {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 40f))
            {
                if (hit.collider.tag == "Monster")
                {
                    StartAttack();
                }
            }
        
    }
    
}