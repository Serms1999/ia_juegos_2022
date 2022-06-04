using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Si se quiere una agente con animación simple, busca un modelo con dos estados de animación.
///  Crea una máquina de estados para esos dos estados. Idle y Walk
///  Pasa de uno a otro y de otro a uno usando un parámetro real que llamarás "Velocity"
///  Arrastra la animación al atributo animator de esta clase y ya tienes un personaje con movimiento.
/// </summary>
[RequireComponent(typeof(Animation))]

public class AgentPlayerWithAnimation : AgentPlayer //MonoBehaviour
{
    Animator animator;

    // Use this for initialization
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    
    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        animator.SetFloat("Velocity", Speed); // Speed, propiedad que calcula el módulo de Velocity
    }
}
