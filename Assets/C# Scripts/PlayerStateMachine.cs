using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Animator))]

public class PlayerStateMachine : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;


    #region animationStrings

    [Header("Start Animation")]
    [SerializeField]
    private string currentAnimation = "Idle";


    [Header("Animation Names")]

    [SerializeField]
    private string idleAnimation = "Idle";
    [SerializeField]
    private string runAnimation = "Walk";

    [SerializeField]
    private string jumpAnimation = "Jump";
    [SerializeField]
    private string fallAnimation = "Fall";


    [SerializeField]
    private string deathAnimation = "Death";
    #endregion

    #region animationStates

    public bool jumping;
    public bool dead;

    #endregion




    private Coroutine jumpTimerCO;


    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }




    #region Change/Transition Animation Function

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationString"></param>
    /// <param name="speed"></param>
    /// <param name="layer"></param>
    /// <returns>true if the animation has changed, false otherwise</returns>
    private bool TryChangeAnimation(string animationString, float speed = 1, int layer = 0)
    {
        if (currentAnimation == animationString)
        {
            return false;
        }

        currentAnimation = animationString;

        anim.speed = speed;
        anim.Play(animationString, layer);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationString"></param>
    /// <param name="transitionDuration"></param>
    /// <param name="speed"></param>
    /// <param name="layer"></param>
    /// <returns>true if the animation has changed, false otherwise</returns>
    private bool TryTransitionAnimation(string animationString, float transitionDuration, float speed = 1, int layer = 0)
    {
        if (currentAnimation == animationString)
        {
            return false;
        }

        currentAnimation = animationString;

        anim.speed = speed;
        anim.CrossFade(animationString, transitionDuration, layer);

        return true;
    }

    #endregion



    public void Idle()
    {
        if (jumping || dead)
        {
            return;
        }
        TryChangeAnimation(idleAnimation);
    }


    public void Run()
    {
        if (jumping || dead)
        {
            return;
        }
        TryChangeAnimation(runAnimation);
    }


    #region Jump And Fall Animation

    public void JumpAndAutoDetectFall()
    {
        if (dead)
        {
            return;
        }


        bool animationChanged = TryChangeAnimation(jumpAnimation);

        if (animationChanged)
        {
            jumping = true;

            if (jumpTimerCO != null)
            {
                StopCoroutine(jumpTimerCO);
            }
            jumpTimerCO = StartCoroutine(JumpAndAutoDetectFall_FixedUpdateLoop());
        }
    }
    private IEnumerator JumpAndAutoDetectFall_FixedUpdateLoop()
    {
        //start fall down animation when player starts falling
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (rb.velocity.y < 0)
            {
                break;
            }
        }

        //wait until player hits the ground and end function
        //also keep force updating the falling animation so animations transition back to this one.
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (dead == false)
            {
                TryChangeAnimation(fallAnimation);
            }

            if (rb.velocity.y == 0)
            {
                jumping = false;
                yield break;
            }
        }
    }
    #endregion


    public void Death()
    {
        dead = true;
        TryChangeAnimation(deathAnimation);
    }
}