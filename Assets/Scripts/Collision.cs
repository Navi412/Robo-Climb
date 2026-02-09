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

    [Header("Ledge Climb")]
    public bool onHead;

    [Space]
    [Header("Collision Offsets")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    public Vector2 headOffset;

    private Color debugCollisionColor = Color.red;

    void Update()
    {
        // Detectar Suelo y Paredes con circulos
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onWall = onRightWall || onLeftWall;
        wallSide = onRightWall ? -1 : 1;

        // Logica para detectar bordes (Ledge Climb)
        float direction = 1;
        if (onRightWall) direction = 1;
        if (onLeftWall) direction = -1;

        if (onRightWall)
        {
            onHead = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(headOffset.x, headOffset.y), collisionRadius, groundLayer);
        }
        else if (onLeftWall)
        {
            onHead = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-headOffset.x, headOffset.y), collisionRadius, groundLayer);
        }
        else
        {
            onHead = Physics2D.OverlapCircle((Vector2)transform.position + headOffset, collisionRadius, groundLayer);
        }
    }

    void OnDrawGizmos()
    {
        // Dibujado de los sensores en el editor
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(headOffset.x, headOffset.y), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(-headOffset.x, headOffset.y), collisionRadius);
    }
}