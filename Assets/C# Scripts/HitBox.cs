using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private PlayerController player;

    public float damage;
    public float hitTime;


    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController p) && p != player)
        {
            p.health -= damage;
            p.rb.velocity = Vector2.zero;

            if (p.health <= 0)
            {
                p.anim.Death();
            }
            else
            {
                p.anim.Hurt();
            }
        }
    }
}
