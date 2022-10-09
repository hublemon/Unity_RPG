using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public ScriptableMonster monsterdata;   //여기서 받고
    public ScriptableMonster MonsterData { set { monsterdata = value; } }  //변경되는 값

    MosterController monsterController;
    LivingEntity monsterEntity;

    public void WatchMonsterInfo()
    {
        monsterEntity.startHP = monsterdata.HP;   //받은 걸 전달
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
