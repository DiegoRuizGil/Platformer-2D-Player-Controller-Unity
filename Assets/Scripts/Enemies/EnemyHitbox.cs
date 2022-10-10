using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitbox : MonoBehaviour
{

    [Header("Game Events")]
    public VoidEvent JumpOverEnemyGameEvent;

    [Header("Unity Events")]
    public UnityEvent OnHitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (JumpOverEnemyGameEvent != null)
                JumpOverEnemyGameEvent.Raise();

            if (OnHitEvent != null)
                OnHitEvent.Invoke();

            GameObject parent = this.transform.parent.gameObject;
            Destroy(parent, 0.05f);
        }
    }
}
