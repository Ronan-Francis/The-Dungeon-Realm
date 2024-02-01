using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Movement parameters
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private float lastHorizontalInput;

    private Rigidbody2D rb;
    public Image dashCooldownBar;
    private bool isGrounded = true;
    private int jumpCount = 0;
    private float dashCooldownTimer = 0f;

    // Initialize the player's Rigidbody2D
    void Start()
    {
        dashCooldownBar.fillAmount = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    // Handle input and cooldown timers
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
        UpdateCooldownBar();
    }

    // Horizontal movement
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(moveX, 0, 0);
        if (Mathf.Abs(moveX) > 0.1f) // Update only if there's significant movement
        {
            lastHorizontalInput = moveX;
        }
    }

    // Jumping logic
    void HandleJump()
    {
        if ((Input.GetButtonDown("Jump") && isGrounded) || (Input.GetButtonDown("Jump") && jumpCount < 2))
        {
            Jump();
        }
    }

    // Jump action
    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
        jumpCount++;
    }

    // Dash input and cooldown management
    void HandleDash()
    {
        if (Input.GetButtonDown("Fire1") && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void UpdateCooldownBar()
    {
        // If dash is on cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownBar.fillAmount = 1 - (dashCooldownTimer / dashCooldown);
        }
        // If dash is not on cooldown, ensure the bar is full
        else
        {
            dashCooldownBar.fillAmount = 1;
        }
    }
    // Dash action
    IEnumerator Dash()
    {
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        // Dash in the direction of the last horizontal input
        Vector2 dashDirection = new Vector2(Mathf.Sign(lastHorizontalInput), 0).normalized;
        rb.velocity = dashDirection * dashForce;

        dashCooldownTimer = dashCooldown;
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        //isDashing = false;
    }

    // Detect collision with the ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    // Detect when leaving the ground
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!(collision.gameObject.CompareTag("Ground")))
        {
            isGrounded = false;
        }
    }
}
