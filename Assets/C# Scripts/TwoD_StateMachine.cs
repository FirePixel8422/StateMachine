using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(Animator))]

public class TwoD_StateMachine : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;


    #region animationStrings

    [Header("Start Animation")]
    [SerializeField]
    private string currentAnimation;


    [Header("Animation Names")]

    [SerializeField]
    private string idleAnimation;
    [SerializeField]
    private string runAnimation;

    [SerializeField]
    private string jumpAnimation;
    [SerializeField]
    private string fallAnimation;

    [SerializeField]
    private string[] attackAnimations;
    public float resetComboDelay;

    [SerializeField]
    private string hurtAnimation;
    [SerializeField]
    private string deathAnimation;
    #endregion

    #region animationStates

    public bool jumping;
    public bool dead;
    public bool hurt;
    public bool attacking;
    public int attackComboId;
    #endregion

    public bool CanPlayerTurn
    {
        get
        {
            return attacking == false;
        }
    }



    private Coroutine comboTimerCO;
    private Coroutine jumpTimerCO;


    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    #region Change Animation Function

    private void ChangeAnimation(string animationString, int layer, out bool failed)
    {
        if (currentAnimation == animationString && attackAnimations.Contains(animationString) == false)
        {
            failed = true;
            return;
        }

        failed = false;
        currentAnimation = animationString;

        anim.speed = 1;
        anim.Play(animationString, layer);
    }

    private void ChangeAnimation(string animationString, int layer, float speed, out bool failed)
    {
        if (currentAnimation == animationString && attackAnimations.Contains(animationString) == false)
        {
            failed = true;
            return;
        }

        failed = false;
        currentAnimation = animationString;

        anim.speed = speed;
        anim.Play(animationString, layer);
    }

    private void ChangeAnimation(string animationString, int layer, out bool failed, float transitionDuration)
    {
        if (currentAnimation == animationString)
        {
            failed = true;
            return;
        }

        failed = false;
        currentAnimation = animationString;

        anim.CrossFade(animationString, transitionDuration, layer);
    }
    #endregion



    public void Idle()
    {
        if (jumping || dead || attacking || hurt)
        {
            return;
        }
        ChangeAnimation(idleAnimation, 0, out _);
    }

    
    public void Run()
    {
        if (jumping || dead || attacking || hurt)
        {
            return;
        }
        ChangeAnimation(runAnimation, 0, out _);
    }


    #region Jump And Fall Animation

    public void JumpAndAutoDetectFall()
    {
        if (dead || hurt)
        {
            return;
        }


        if (jumpTimerCO != null)
        {
            StopCoroutine(jumpTimerCO);
        }

        jumpTimerCO = StartCoroutine(JumpAndAutoDetectFallTimer());
    }
    private IEnumerator JumpAndAutoDetectFallTimer()
    {
        jumping = true;

        if (attacking == false)
        {
            ChangeAnimation(jumpAnimation, 0, out bool failed);

            if (failed)
            {
                yield break;
            }
        }

        //start fall down animation when player starts falling
        while (true)
        {
            yield return null;

            if (rb.velocity.y < 0)
            {
                break;
            }
        }

        //wait until player hits the ground and end function
        //also keep force updating the falling animation so animations transition back to this one.
        while (true)
        {
            yield return null;
            if (dead == false && attacking == false)
            {
                ChangeAnimation(fallAnimation, 0, out _);
            }

            if (rb.velocity.y == 0)
            {
                jumping = false;
                yield break;
            }
        }
    }
    #endregion


    #region Attack and Combo's Animation

    public void Attack(float attackSpeedMultiplier)
    {
        if (dead || attacking || hurt)
        {
            return;
        }
        ChangeAnimation(attackAnimations[attackComboId], 0, attackSpeedMultiplier, out bool failed);

        if (failed)
        {
            return;
        }

        attacking = true;

        attackComboId += 1;
        if (attackComboId == attackAnimations.Length)
        {
            attackComboId = 0;
        }


        if (comboTimerCO != null)
        {
            StopCoroutine(comboTimerCO);
        }
        comboTimerCO = StartCoroutine(EndAttackTimer(attackSpeedMultiplier));
    }
    public IEnumerator EndAttackTimer(float attackSpeedMultiplier)
    {
        yield return new WaitForEndOfFrame();

        float clipTime = anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(clipTime);

        attacking = false;

        if (attackAnimations.Length > 1)
        {
            yield return new WaitForSeconds(resetComboDelay / attackSpeedMultiplier);
            attackComboId = 0;
        }
    }
    #endregion


    public void Hurt()
    {
        if (dead)
        {
            return;
        }

        hurt = true;
        ChangeAnimation(hurtAnimation, 0, out _);


        attacking = false;
        attackComboId = 0;
        jumping = false;

        StopAllCoroutines();
        StartCoroutine(EndHurtTimer());
    }
    private IEnumerator EndHurtTimer()
    {
        float clipTime = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipTime);
        hurt = false;
    }


    public void Death()
    {
        dead = true;
        ChangeAnimation(deathAnimation, 0, out _);
    }    
}