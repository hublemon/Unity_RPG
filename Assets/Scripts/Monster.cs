using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public ScriptableMonster monsterdata;   //���⼭ �ް�
    public ScriptableMonster MonsterData { set { monsterdata = value; } }  //����Ǵ� ��

    MosterController monsterController;
    LivingEntity monsterEntity;

    public void WatchMonsterInfo()
    {
        monsterEntity.startHP = monsterdata.HP;   //���� �� ����
        monsterController.monsterAI.speed = monsterdata.Speed;
    }

    // Start is called before the first frame update
    void Awake()
    {
        monsterController = FindObjectOfType<MosterController>().gameObject.GetComponent<MosterController>();
        monsterEntity = FindObjectOfType<LivingEntity>().gameObject.GetComponent<LivingEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
