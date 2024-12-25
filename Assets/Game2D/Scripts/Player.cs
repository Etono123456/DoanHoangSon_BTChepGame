using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;

    private float moveSpeed = 7;
    private float jumpForce = 1500; 
    private float horizontal;
    private int coin = 0;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private Vector3 savePoint;
    void Start()
    {
        // Lấy component Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 9.81f;

        SavePoint();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }  
        // Check chạm đất
        isGrounded = CheckGround();

        // Lấy input từ bàn phím
        horizontal = Input.GetAxisRaw("Horizontal");
        
        if (isAttacking)
        {
            return;
        }

        if (isGrounded)
        {
            // SPACE = nhảy
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            } 
            // Tấn công với nút C
            if (Input.GetKeyDown(KeyCode.C))
            {
                Attack();
            }
            // Ném với nút V
            if (Input.GetKeyDown(KeyCode.V))
            {
                Throw();
            }
            //Idle
            else if (!isAttacking && !isJumping)
            {
                Idle();
            }    
        }
        else
        {
            if (rb.linearVelocity.y < 0)
            {
                JumpEnd();
            }
        }

        // Di chuyển nhân vật
        if (!isAttacking && Mathf.Abs(horizontal) > 0.1f)
        {
            Run();
        }   
    }

    public override void OnInit()
    {
        base.OnInit();
        isDead = false;
        isAttacking = false;

        transform.position = savePoint;
        ChangeAnim("idle");
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
    }

    protected override void OnDead()
    {
        base.OnDead();
    }

    private bool CheckGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        return hit.collider != null;
    }

    private void Run()
    {
        if (!isJumping)
        {
            ChangeAnim("run");
        }
        //Di chuyển
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        //Xoay
        transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
    }

    private void Attack()
    {
        rb.linearVelocity = Vector3.zero;
        ChangeAnim("attack");
        isAttacking = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void Throw()
    {
        rb.linearVelocity = Vector2.zero;
        ChangeAnim("throw");
        isAttacking = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void ResetAttack()
    {
        ChangeAnim("idle");
        isAttacking = false;
    }

    private void Jump()
    {
        isJumping = true;
        ChangeAnim("jumpstart");
        rb.AddForce(jumpForce * Vector2.up);
    }

    private void JumpEnd()
    {   
        ChangeAnim("jumpend");
        isJumping = false;
    }    

    private void Idle()
    {
        ChangeAnim("idle");
        rb.linearVelocity = Vector2.zero;
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coin++;
            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            isDead = true;
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }
}