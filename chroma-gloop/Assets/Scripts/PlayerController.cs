using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    public Rigidbody2D playerRb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float dashForce;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isDashing;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float cooldownTime;
    public bool canDash;
    [SerializeField] private bool isCrouching;
    [SerializeField] private Vector2 facing;
    Vector2 moveDirection = Vector2.zero;
    Vector2 smoothedDirection;
    Vector2 moveDirectionSmoothedVelocity;
    [SerializeField] private float smoothDampTime;
    [SerializeField] private bool isDead;

    [Header("PowerUp Variables")]
    public int colorNum;

    [Header("Input Variables")]
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
    private InputAction crouch;

    [Header("Other Variables")]
    [SerializeField] private GroundCheck groundCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Vector2 currentRoomSpawn;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerRb = GetComponent<Rigidbody2D>();
        sprite.color = Color.cyan;
    }

    private void OnEnable()
    {
        // The move and jump actions are set to those in the playerControls action map
        move = playerControls.Player.Move;
        move.Enable();

        // Here the states for the jumping action are binded to the 2 jump events. Done by the += operator
        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
        jump.canceled += JumpReleased;

        dash = playerControls.Player.Dash;
        dash.Enable();
        dash.performed += Dash;

        crouch = playerControls.Player.Crouch;
        crouch.Enable();
        crouch.performed += Crouch;
        crouch.canceled += CrouchReleased;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        dash.Disable();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }
    }

    private void JumpReleased(InputAction.CallbackContext context)
    {
        if (playerRb.velocity.y > 0)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.y, 0);
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (isGrounded && canDash && dashCooldown >= cooldownTime)
        {
            dashCooldown = 0;
            smoothedDirection = facing;
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            playerRb.AddForce(facing * dashForce, ForceMode2D.Impulse);
        }
        else if (!isGrounded && canDash)
        {
            dashCooldown = 0;
            canDash = false;
            smoothedDirection = facing;
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            playerRb.AddForce(facing * dashForce, ForceMode2D.Impulse);
        }
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isCrouching = true;
        }
    }

    private void CrouchReleased(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isCrouching = false;
        }
    }

    public void PowerUpGain(int colourNumber)
    {
        if (colourNumber == 0)
        {
            colorNum = colourNumber;
            sprite.color = Color.yellow;
            playerRb.gravityScale = 1.1f;
            moveSpeed = 6;
            smoothDampTime = 0.075f;
        }
        else if (colourNumber == 1)
        {
            colorNum = colourNumber;
            sprite.color = Color.blue;
            playerRb.gravityScale = 1.8f;
            moveSpeed = 6;
            smoothDampTime = 1.1f;
        }
        else if (colourNumber == 2)
        {
            colorNum = colourNumber;
            sprite.color = Color.red;
            playerRb.gravityScale = 1.8f;
            moveSpeed = 9;
            smoothDampTime = 0.075f;
        }
        else
        {
            colourNumber = -1;
        }
    }

    IEnumerator DeathCoroutine()
    {
        isDead = true;
        animator.SetBool("isDead", isDead);
        yield return new WaitForSeconds(1);
        transform.position = currentRoomSpawn;
        isDead = false;
        animator.SetBool("isDead", isDead);
    }

    void Update()
    {
        isGrounded = groundCollider.isGrounded;

        if (moveDirection.x > 0)
        {
            facing = Vector2.right;
            sprite.flipX = false;

        }
        else if (moveDirection.x < 0)
        {
            facing = Vector2.left;
            sprite.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        smoothedDirection = Vector2.SmoothDamp(
            smoothedDirection,
            moveDirection,
            ref moveDirectionSmoothedVelocity,
            smoothDampTime);

        if (colorNum == 1)
        {
            if (!isGrounded)
            {
                if (smoothDampTime > 0.075f)
                {
                    smoothDampTime -= 0.01666666667f;
                }
                //smoothDampTime = 0.075f;
            }
            else
            {
                smoothDampTime = 1.1f;
            }
        }

        if (!isDead)
        {
            if (!isCrouching)
            {
                if (dashCooldown >= cooldownTime / 2.5f || (canDash && dashCooldown > cooldownTime))
                {
                    playerRb.velocity = new Vector2(smoothedDirection.x * moveSpeed, playerRb.velocity.y);
                    isDashing = false;
                }
                else
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                    isDashing = true;
                }
            }
            else
            {
                playerRb.velocity = new Vector2(smoothedDirection.x * moveSpeed / 2, playerRb.velocity.y);
            }
        }
        else
        {
            playerRb.velocity = Vector2.zero;
        }

        if (dashCooldown < cooldownTime)
        {
            dashCooldown += 0.01666666667f;
        }
        else if (dashCooldown >= cooldownTime)
        {
            dashCooldown = cooldownTime;

        }

        if (isGrounded && animator.GetBool("isJumping") == true)
        {
            animator.SetBool("isJumping", false);
        }

        animator.SetFloat("xVelocity", Math.Abs(playerRb.velocity.x));
        animator.SetFloat("yVelocity", playerRb.velocity.y);
        animator.SetBool("isDashing", isDashing);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            animator.SetBool("isJumping", false);
        }
        else if (collision.gameObject.layer == 4)
        {
            WaterTile waterTile = collision.gameObject.GetComponent<WaterTile>();
            if (waterTile.frozen)
            {
                animator.SetBool("isJumping", false);
            }
        }
        else if (collision.gameObject.layer == 7)
        {
            StartCoroutine(DeathCoroutine());
        }

        if (crouch.IsInProgress())
        {
            isCrouching = true;
        }

        if (collision.gameObject.CompareTag("PowerUp"))
        {
            PowerUp powerUp = collision.gameObject.GetComponent<PowerUp>();
            PowerUpGain(powerUp.colourNum);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("DashOrb") && !canDash)
        {
            DashOrb dashOrb = collision.gameObject.GetComponent<DashOrb>();
            if (!dashOrb.collected)
            {
                dashOrb.StartCoroutine(dashOrb.Collected());
                canDash = true;
            }
        }

        if (collision.gameObject.CompareTag("Room"))
        {
            RoomManager currentRoom = collision.gameObject.GetComponent<RoomManager>();
            currentRoomSpawn = currentRoom.spawnPosition;
        }

        if (collision.gameObject.layer == 4 && colorNum == 1)
        {
            WaterTile waterTile = collision.gameObject.GetComponent<WaterTile>();
            waterTile.Freeze();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isCrouching = false;
            animator.SetBool("isJumping", true);
        }
        else if (collision.gameObject.layer == 4)
        {
            WaterTile waterTile = collision.gameObject.GetComponent<WaterTile>();
            if (waterTile.frozen)
            {
                isCrouching = false;
                animator.SetBool("isJumping", true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 4 && colorNum == 2)
        {
            WaterTile waterTile = collision.gameObject.GetComponent<WaterTile>();
            waterTile.StartCoroutine(waterTile.Melting());
        }
    }
}
