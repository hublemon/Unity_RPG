using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum MonsterType { Monster_Basic,Monster_Speeder,Monster_Heavy}
    public List<ScriptableMonster> MonsterDatas = new List<ScriptableMonster>();

    public GameObject monsterPrefab;
    public Monster monster;

    IEnumerator Spawn(int Count)
    {
        for (int i= 0; i < Count; i++)
        {
            int random = Random.Range(0, 3);
            var monster = SpawnFunc((MonsterType)random);
            monster.WatchMonsterInfo();
            yield return new WaitForSeconds(20);
            int r = Random.Range(2, 5);
            StartCoroutine(Spawn(r));
        }
    }

    Monster SpawnFunc(MonsterType type)
    {
        var newMonster = Instantiate(monsterPrefab).GetComponent<Monster>();
        newMonster.MonsterData = MonsterDatas[(int)(type)];  //type¿« ¿Œµ¶Ω∫
        return newMonster;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn(3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
