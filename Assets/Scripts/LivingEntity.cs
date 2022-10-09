using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable
{
    //몬스터한테만 일단 주기

    public float startHP = 200;

    public float health { get; protected set; }

    protected bool dead;


    public virtual void TakeHit(float damage, Vector3 point)    //다른데서 받는 거라 public해야함
    {
        health -= damage;

        if (health <= 0 & !dead)
        {
            Die();  //invoke는 여가서는 안되네
        }
    }
    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        health = startHP;
    }

    public virtual void Die()
    {
        dead = true;
    }

}
