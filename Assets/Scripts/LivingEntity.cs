using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable
{
    //�������׸� �ϴ� �ֱ�

    public float startHP = 200;

    public float health { get; protected set; }

    protected bool dead;


    public virtual void TakeHit(float damage, Vector3 point)    //�ٸ����� �޴� �Ŷ� public�ؾ���
    {
        health -= damage;

        if (health <= 0 & !dead)
        {
            Die();  //invoke�� �������� �ȵǳ�
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
