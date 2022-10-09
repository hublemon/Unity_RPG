using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable/MonsterData",fileName ="MonsterData")]
public class ScriptableMonster : ScriptableObject
{
    public float hp = 200f;
    public float HP { get { return hp; } set { hp = value; } }

    public float speed = 2f;
    public float Speed { get { return speed; } set { speed = value; } }
    // Start is called before the first frame update
 
}
