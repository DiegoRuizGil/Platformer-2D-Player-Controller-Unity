using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectores : MonoBehaviour
{
    public CircleCollider2D cc2D;
    public LayerMask groundLayer;

    void Start()
    {

    }

    void Update()
    {
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down, cc2D.radius + 0.1f, groundLayer);

        if (raycast)
            Debug.Log("Ha encontrado suelo");
        
        Debug.DrawRay(raycast.point, raycast.normal, Color.red);
    }
}
