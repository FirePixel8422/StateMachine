using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public TwoD_StateMachine anim;
    public Rigidbody2D rb;

    public Transform[] groundChecks;
    public LayerMask groundLayer;

    public HitBox[] hitBoxes;

    public float attackSpeedMultiplier;

    public float moveSpeed;
    public float jumpForce;

    public float health;


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


    private Vector2 dir;
    public void OnMove(InputAction.CallbackContext ctx)
    {
        dir = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && IsOnGround)
        {
            anim.JumpAndAutoDetectFall();
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private bool attackHeld;
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            attackHeld = true;
        }
        if (ctx.canceled)
        {
            attackHeld = false;
        }
    }


    private void Update()
    {
        if (anim.dead || anim.hurt)
        {
            return;
        }

        //flip player sprite
        if (anim.CanPlayerTurn)
        {
            if (dir.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
            }
        }
        
        if (dir.x == 0)
        {
            anim.Idle();
        }
        else
        {
            anim.Run();
        }

        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

        if (attackHeld)
        {
            Attack();
        }
    }

    private void Attack()
    {
        anim.Attack(attackSpeedMultiplier);
    }

    public void EnableCollider(int id)
    {
        StartCoroutine(DisableColider(id));
    }
    private IEnumerator DisableColider(int id)
    {
        hitBoxes[id].gameObject.SetActive(true);

        yield return new WaitForSeconds(hitBoxes[id].hitTime);

        hitBoxes[id].gameObject.SetActive(false);
        hitBoxes[id].transform.localPosition = Vector3.zero;
    }
}
