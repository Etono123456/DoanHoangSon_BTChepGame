using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private string currentAnim;

    private float hp;
    public bool isDeath => hp <= 0;
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        hp = 100;
    }   
    public virtual void OnDespawn()
    {

    }
    protected virtual void OnDead()
    {

    }
    protected void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }
    public void OnHit(float damage)
    {
        if (!isDeath)
        {
            hp -= damage;

            if (isDeath)
            {
                OnDead();
            }    
        }    
    }    
}
