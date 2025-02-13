using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsMovement
{
    [SerializeField] private float maxSpeed = 7;
    [SerializeField] private float jumpTakeOffSpeed = 7;

    [SerializeField] private AnimationCurve jumpSpeedCurve;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float jumpTime = 0.35f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
   // private float coyoteTimer;

    private bool wasGrounded;
    private bool isCoyoteTime = false;
    private bool isJumping = false;
    private bool isJumpBufferTime = false;

    private Coroutine coyoteCoroutine;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");
        if (grounded)
        {
            wasGrounded = grounded;
        }
        else if (wasGrounded && isJumping == false)
        {
            coyoteCoroutine = StartCoroutine(CoyoteCoroutine());
            wasGrounded = false;
        }
        if (Input.GetButtonDown("Jump") && (grounded || isCoyoteTime) || isJumpBufferTime && grounded)
        {
            //if (isCoyoteTime) { Debug.Log("coyoteJump"); }
           // if (isJumpBufferTime && grounded) { Debug.Log("isJumpBufferTime && grounded"); }
            StopCoroutine(coyoteCoroutine);
            isCoyoteTime = false;
            StartCoroutine(JumpCoroutine());
        }
        else if (Input.GetButtonDown("Jump"))
        {

            StartCoroutine(JumpBuffer());
        }
/*        else if (Input.GetButtonUp("Jump"))
        {
            if (Velocity.y > 0)
            {
                Velocity.y = Velocity.y * 0.5f;
            }
        }*/

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(Velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        float time = 0;
        while (time < jumpTime) 
        {
            time += Time.deltaTime;
            float lerpedTime = Mathf.Lerp(0, 1, time/jumpTime);
            Velocity.y = jumpTakeOffSpeed * jumpSpeedCurve.Evaluate(lerpedTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isJumping = false;
    }
    private IEnumerator CoyoteCoroutine()
    {
        isCoyoteTime = true;
        float time = 0;
        while (time < coyoteTime)
        {
            time += Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }
        isCoyoteTime = false;
/*        Debug.Log("coyoteTimerEnded");*/
    }

    private IEnumerator JumpBuffer()
    {
        isJumpBufferTime = true;
        float time = 0;
        while (time < jumpBufferTime)
        {
            time += Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }
        isJumpBufferTime = false;
    }
}
