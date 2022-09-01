using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D coll;
    [Header("移动参数")] 
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    
    [Header("跳跃参数")] 
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    
    private float jumpTime;
    [Header("状态")] 
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;

    [Header("环境检测")] 
    public LayerMask groundLayer;
    
    
    
    
    //speed
    private float xVelocity;
    
    //key
    private bool jumpPressd;
    private bool jumpHeld; 
    private bool crouchHeld;
    
    //coll size
    private Vector2 colliderStandSize;
    private Vector2 colliserStandOffset;
    private Vector2 colliderCrouchSize;
    private Vector2 colliderCrouchOffset;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        colliderStandSize = coll.size;
        colliserStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(colliderStandSize.x, colliderStandSize.y / 2f);
        colliderCrouchOffset = new Vector2(colliserStandOffset.x, colliserStandOffset.y / 2f);
    }

    private void Update()
    {
        jumpPressd = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        if (coll.IsTouchingLayers(groundLayer))
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
    }

    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
        {
            Crouch();
        }
        else if (!crouchHeld && isCrouch)
        {
            StandUp();
        }
        else if (!isOnGround && isCrouch)
        {
            StandUp();
        }

        xVelocity = Input.GetAxis("Horizontal");;//-1 ~  1
        if (isCrouch)
        {
            xVelocity /= crouchSpeedDivisor;
        }

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FlipDirection();
    }

    void MidAirMovement()
    {
        if (jumpPressd && isOnGround && !isJump)
        {
            if (isCrouch && isOnGround)
            {
                StandUp();
                rb.AddForce(new Vector2(0f,crouchJumpBoost),ForceMode2D.Impulse);
            }
        }

        if (jumpPressd && isOnGround && !isJump)
        {
            isOnGround = false;
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration;
            rb.AddForce(new Vector2(0f,jumpForce),ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            if (jumpHeld)
            {
                rb.AddForce(new Vector2(0f,jumpHoldForce),ForceMode2D.Impulse);
            }
            if (jumpTime < Time.time)
            {
                isJump = false;
            }
        }
    }

    void FlipDirection()
    {
        if (xVelocity < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }

        if (xVelocity > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset =colliserStandOffset;
    }
}
