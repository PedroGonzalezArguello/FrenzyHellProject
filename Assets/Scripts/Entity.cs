using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//De entity va a heredar todo lo que sea un ser vivo. Tiene vida? Puede morir? Es entity
//ABSTRACT: La el script solo sirve para herencias, no lo podés tirar en escena
public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float _health;
    [SerializeField] protected bool isAlive;

    [Header("PopUp")]
    [SerializeField] protected string _popUp = "X";
    public virtual void TakeDamage(int dmg) 
    {
        _health -= dmg;
        if( _health <= 0)
        {
            Death();
        }
    }
    
    // Abstract Death: Obligo a todas las entitys a tener una funcion Death. No le doy cuerpo xq cada entity tiene su death particular
    // (Cuerpo: lo que está entre las llaves { })
    protected abstract void Death();


    public void PopUp()
    {
        PopUpManager._current.PopUp(transform.position, _popUp, Color.red);
    }
    
}
