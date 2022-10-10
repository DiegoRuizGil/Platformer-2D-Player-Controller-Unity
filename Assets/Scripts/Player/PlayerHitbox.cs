using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [Header("Game Events")]
    public KnockbackInfoEvent kbInfoEvent;

    [Header("Configuration")]
    public Vector2 kbMovementDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            if(kbInfoEvent != null)
            {
                float xDirection = this.transform.position.x > collision.transform.position.x ? kbMovementDirection.x : -kbMovementDirection.x;
                Vector2 direction = new Vector2(xDirection, kbMovementDirection.y);
                KnockbackInfo kbInfo = new KnockbackInfo(direction);
                kbInfoEvent.Raise(kbInfo);
            }
        }
    }
}
