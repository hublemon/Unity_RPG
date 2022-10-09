using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MosterController : LivingEntity
{
    //�ִϸ��̼� �̺�Ʈ Ȱ���ϱ�(�̺�Ʈ �Լ� �ɷ��� �� target�� beingattack->�ð� ����
    [SerializeField]
    LayerMask target;

    public NavMeshAgent monsterAI;  //public���� ����
    Animator monsterAnimator;

    PlayerMovement player;
    Transform targetTrans;

    [SerializeField]
    float minDistance = 3f;  //Ÿ�ٰ��� �ּ� �Ÿ�

    [SerializeField]
    float attackdelay=3f;

    [SerializeField]
    float traceDis = 30f;  //���� �Ÿ�

    [SerializeField]
    float attackDis = 3.5f;  //���ݰŸ�

    [SerializeField]
    Slider hpSlider;

    float lastAttackTime;
    

    public float speed=5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        hpSlider.maxValue = startHP;
        hpSlider.value = health;
    }

    private void Awake()
    {
        monsterAI = GetComponent<NavMeshAgent>();
        monsterAnimator = GetComponent<Animator>();
        targetTrans = FindObjectOfType<PlayerMovement>().gameObject.transform;
        player = FindObjectOfType<PlayerMovement>().gameObject.GetComponent<PlayerMovement>();
        targetTrans = player.gameObject.transform;

    }


    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(UpdatePath());  //������ �ؾ���
        monsterAnimator.SetBool("Die", false);
        
    }

    IEnumerator UpdatePath()
    {
        if (!dead)
        {
            float dis = Vector3.Distance(targetTrans.position, transform.position);
            if (attackDis<dis&&dis <= traceDis)  //�����Ÿ� �ȿ� ������ ����
            {
                monsterAI.SetDestination(targetTrans.position);
                monsterAnimator.SetBool("Run", true);
                monsterAI.isStopped = false;
                monsterAnimator.SetBool("Attack", false);
                if (minDistance<=dis&&dis <= attackDis)
                {
                    monsterAnimator.SetTrigger("Attack");
                    MonsterAttack();
                    transform.position = targetTrans.position - transform.forward * minDistance;
                }
                //if (dis <= minDistance)
                //{
                    //transform.position = targetTrans.position - transform.forward * minDistance;
                    //monsterAnimator.SetBool("Run", false);
                    //monsterAnimator.SetBool("Attack", false);
                //}
            }
        }
        else
        {
            monsterAI.isStopped = true;
            monsterAnimator.SetBool("Run", false);

        }
            yield return new WaitForSeconds(0.5f);  //��ΰ��
            StartCoroutine(UpdatePath());
        
    }

    void MonsterAttack()
    {
        player.gameObject.GetComponent<Animator>().SetTrigger("beingAttack");
    }

    public override void TakeHit(float Damage, Vector3 point)
    {
        if (!dead)
        {
            monsterAnimator.SetTrigger("BeingAttack");
        }
        base.TakeHit(Damage, point);
        hpSlider.value = health;
    }

    public override void Die()
    {
        monsterAnimator.SetBool("Die",true);
        base.Die();
            Destroy(gameObject,4.5f);
        }

    

  
    // Update is called once per frame
    void Update()
    {
        
    }
}
