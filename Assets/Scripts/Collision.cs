using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;
    
    [Header("Ledge Climb (Nuevo)")]
    public bool onHead; // Detecta si hay techo o aire sobre la cabeza

    [Space]
    [Header("Collision Offsets")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    public Vector2 headOffset; // Configura esto en el Inspector (Círculo amarillo)
    
    private Color debugCollisionColor = Color.red;

    void Update()
    {  
        // 1. Detectar Suelo
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        // 2. Detectar Paredes
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onWall = onRightWall || onLeftWall;
        wallSide = onRightWall ? -1 : 1;

        // 3. Detectar Cabeza (Para el Ledge Climb)
        // Calculamos la dirección basándonos en si tocamos pared derecha o izquierda
        float direction = 1;
        if (onRightWall) direction = 1;
        if (onLeftWall) direction = -1;

        // Si no tocamos ninguna pared, usamos la dirección del offset base
        // Nota: Asumimos que headOffset.x es positivo en el inspector
        Vector2 finalHeadOffset = new Vector2(Mathf.Abs(headOffset.x) * direction, headOffset.y);
        
        // Si tocamos pared derecha, usamos offset positivo. Si izquierda, negativo.
        // Pero espera, tus offsets right/left ya tienen signo. Vamos a simplificarlo:
        
        if(onRightWall)
        {
             onHead = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(headOffset.x, headOffset.y), collisionRadius, groundLayer);
        } 
        else if (onLeftWall)
        {
             // Invertimos la X para el lado izquierdo
             onHead = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-headOffset.x, headOffset.y), collisionRadius, groundLayer);
        }
        else
        {
            // Si no estamos en pared, chequeamos arriba del todo por defecto
            onHead = Physics2D.OverlapCircle((Vector2)transform.position + headOffset, collisionRadius, groundLayer);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        // Dibujar el sensor de la cabeza (Amarillo)
        // Dibujamos los dos posibles lados para que sea fácil de configurar
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(headOffset.x, headOffset.y), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(-headOffset.x, headOffset.y), collisionRadius);
    }
}