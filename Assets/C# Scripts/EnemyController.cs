using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public TwoD_StateMachine anim;
    public Rigidbody2D rb;

    public Transform[] groundChecks;
    public LayerMask groundLayer;

    public HitBox[] hitBoxes;

    public PlayerController player;


    public float moveSpeed;

    public float attackInterval;
    public float range;

    public float jumpForce;


    public bool inAttackRange;
    public bool attackReady = true;


    public bool IsOnGround
    {
        get
        {
            bool onGround = false;
            if (Physics2D.OverlapCircleAll(groundChecks[0].position, 0.1f, groundLayer).Length != 0)
            {
                onGround = true;
            }
            if (Physics2D.OverlapCircleAll(groundChecks[1].position, 0.1f, groundLayer).Length != 0)
            {
                onGround = true;
            }
            return onGround;
        }
    }

    private void Start()
    {
        anim = GetComponent<TwoD_StateMachine>();
        rb = GetComponent<Rigidbody2D>();
    }




    private void Update()
    {
        if (player != null)
        {
            MoveToPlayer(transform.position);

            if (attackReady && inAttackRange)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void MoveToPlayer(Vector2 enemyPos)
    {
        if (Vector2.Distance(enemyPos, player.transform.position) < range)
        {
            inAttackRange = true;
            return;
        }
        float newPos = Mathf.MoveTowards(enemyPos.x, player.transform.position.x, moveSpeed * Time.deltaTime);


        float dir = enemyPos.x - newPos;
        if (anim.CanPlayerTurn)
        {
            if (dir != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(dir), 1, 1);
            }
        }

        if (dir == 0)
        {
            anim.Idle();
        }
        else
        {
            anim.Run();
        }

        transform.position = new Vector2(newPos, transform.position.y);
    }
    private IEnumerator Attack()
    {
        attackReady = false;
        print("start");

        anim.Attack();

        yield return new WaitForSeconds(attackInterval);
        print("stopeed");
        attackReady = true;
    }
}
