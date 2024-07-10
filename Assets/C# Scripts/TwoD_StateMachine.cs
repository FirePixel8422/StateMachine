using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]

public class TwoD_StateMachine : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;



    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Header("Start Animation")]
    [SerializeField]
    private string currentAnimation;



    [Header("Animation Names")]

    [SerializeField]
    private string idleAnimation;
    [SerializeField]
    private string jumpAnimation;
    [SerializeField]
    private string fallAnimation;
    [SerializeField]
    private string runAnimation;
    [SerializeField]
    private string attackAnimation;
    [SerializeField]
    private string hurtAnimation;
    [SerializeField]
    private string deathAnimation;

    public bool jumping;
    public bool dead;
    public bool attack;


    private void ChangeAnimation(string animationString, int layer, out bool failed)
    {
        failed = false;

        if (currentAnimation == animationString)
        {
            failed = true;
            return;
        }
        currentAnimation = animationString;

        anim.Play(animationString, layer);
    }


    public void Idle()
    {
        if (jumping || dead || attack)
        {
            return;
        }
        ChangeAnimation(idleAnimation, 0, out _);
    }
    public void Run()
    {
        if (jumping || dead || attack)
        {
            return;
        }
        ChangeAnimation(runAnimation, 0, out _);
    }
    public void Attack(float delay)
    {
        if (dead || attack)
        {
            return;
        }
        ChangeAnimation(attackAnimation, 0, out _);
        attack = true;
        StartCoroutine(EndAttack(delay));
    }
    public IEnumerator EndAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        attack = false;
    }


    public void Death()
    {
        dead = true;
        ChangeAnimation(deathAnimation, 0, out _);
    }
    public void Hurt()
    {
        if (dead)
        {
            return;
        }
        ChangeAnimation(hurtAnimation, 1, out _);
    }


    public void JumpAndAutoDetectFall(Rigidbody2D playerRigidbody)
    {
        if (dead || attack)
        {
            return;
        }
        StartCoroutine(JumpAndAutoDetectFallTimer(playerRigidbody));
    }
    private IEnumerator JumpAndAutoDetectFallTimer(Rigidbody2D playerRigidbody)
    {
        jumping = true;
        ChangeAnimation(jumpAnimation, 0, out bool failed);

        if (failed)
        {
            yield break;
        }

        while (true)
        {
            yield return null;
            if (playerRigidbody.velocity.y < 0)
            {
                ChangeAnimation(fallAnimation, 0, out _);
                break;
            }
        }
        while (true)
        {
            yield return null;
            if (playerRigidbody.velocity.y == 0)
            {
                jumping = false;
                yield break;
            }
        }
    }
}
