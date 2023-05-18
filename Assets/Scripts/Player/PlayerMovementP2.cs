using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class PlayerMovementP2 : MonoBehaviour
{
    // UI
    public TMP_Text uiVelocity;

    // Gets various objects and components
    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public GameObject player;
    public LayerMask groundLayer;

    // Player properties
    public float jumpHeight = 8f;
    public float maxSpeed = 9f;
    public float extraHVelocity;

    // Animation parameters
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isStomping = false;
    private bool isBouncing = false;
    private bool isSuperStomping = false;
    private bool CanDoubleJump = false;
    private float stretchXVelocity;
    private float stretchYVelocity;
    
    // Sound effects
    public AudioClip _jump;
    public AudioClip _stomp;
    public AudioClip _superstomp;
    public AudioClip _dash;
    public AudioClip _superdash;
    public AudioClip _wallslide;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");
        float horizontalVelocity = dirX * maxSpeed;
        rb.velocity = new Vector2(horizontalVelocity + extraHVelocity, rb.velocity.y);
        uiVelocity.text = "Velocity: " + Mathf.Round(rb.velocity.x);

        updateAnim(dirX);
        killplaneCheck();
        IsTouchingWall(dirX);

        if(rb.velocity.y < -10)
        {
            if(rb.velocity.y >= -30)
            {
                stretchYVelocity = Math.Abs(0.003f * (Math.Abs(rb.velocity.y) - 10));
            }
            else
            {
                stretchYVelocity = 0.06f;
            }
        }
        else
        {
            stretchYVelocity = 0;
        }

        if(Math.Abs(rb.velocity.x) > 10)
        {
            if(Math.Abs(rb.velocity.x) <= 30)
            {
                stretchXVelocity = Math.Abs((0.004f * Math.Abs(rb.velocity.x - 10)));
            }
            else
            {
                stretchXVelocity =  0.08f;
            }
        }
        else
        {
            stretchXVelocity = 0;
        }

        player.transform.localScale = new Vector3(0.8999999f + stretchXVelocity, 0.8f + stretchYVelocity, 1);

        if(isJumping)
        {
            if(rb.velocity.y < 0)
            {
                anim.SetInteger("jumpstate", 2);
            }
            else
            {
                anim.SetInteger("jumpstate", 1);
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            if (IsGrounded(dirX))
            {
                jump();
            }
            else if(CanDoubleJump)
            {
                doubleJump();
            }
        }
        if(Input.GetButtonDown("Dash"))
        {
            if (!IsGrounded(dirX) && !isDashing)
            {
                dash(dirX);
            }
        }
        if(Input.GetButtonDown("Stomp"))
        {
            if (!IsGrounded(dirX) && !isStomping)
            {
                stomp();
            }
        }
        if(Input.GetButtonDown("Pause"))
        {
           kill();
        }
        
    }

    // Creates a vertical raycast to determine if the player is touching the ground.
    // credit to kylewbanks for the base script :]
    bool IsGrounded(float dirX)
    {
    Vector2 position = transform.position;
    Vector2 direction = Vector2.down;
    float distance = 0.5f;
    
    RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
    if (hit.collider != null) 
    {
        if(isDashing)
        {
            isDashing = false;
            dashingCoyote(dirX);
        }
        if(isStomping)
        {
            isStomping = false;
            stompingCoyote();
        }
        if(isBouncing)
        {
            anim.SetBool("bouncedashing", false);
            dashingCoyote(dirX);
            isBouncing = false;
        }
        if(isSuperStomping)
        {
            anim.SetBool("superstomping", false);
            isSuperStomping = false;
        }
        if(!CanDoubleJump)
        {
            CanDoubleJump = true;
        }
        extraHVelocity = 0f;
        return true;
    }

        return false;
    }

    // Creates a horizontal raycast to determine if the player is touching a wall.
    // credit to kylewbanks for the base script :]
    bool IsTouchingWall(float dirX)
    {
    Vector2 position = transform.position;
    Vector2 direction = Vector2.right;
    float distance = 0.5f;
    
    RaycastHit2D hit_right = Physics2D.Raycast(position, direction, distance, groundLayer);
    if (hit_right.collider != null) 
    {
        extraHVelocity = 0f;
        return true;
    }

    direction = Vector2.left;

    RaycastHit2D hit_left = Physics2D.Raycast(position, direction, distance, groundLayer);
    if (hit_left.collider != null) 
    {
        extraHVelocity = 0f;
        return true;
    }
    
    return false;
    }

    // Controls the Animator of Player
    void updateAnim(float dirX)
    {
        
        if(!isBouncing)
        {
            anim.SetBool("bouncedashing", false);
        }
        
        if (dirX > 0f && IsGrounded(dirX))
        {
            anim.SetBool("walking", true);
            anim.SetBool("grounded", true);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = true;
        }
        else if (dirX < 0f && IsGrounded(dirX))
        {
            anim.SetBool("walking", true);
            anim.SetBool("grounded", true);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = false;
        }
        else if (IsGrounded(dirX))
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", true);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
        }
        else if(dirX < 0f && isStomping)
        {
            anim.SetBool("stomping", true);
            anim.SetBool("dashing", false);
            sprite.flipX = false;
        }
        else if(dirX > 0f && isStomping)
        {
            anim.SetBool("stomping", true);
            anim.SetBool("dashing", false);
            sprite.flipX = true;
        }
        else if(isStomping)
        {
            anim.SetBool("stomping", true);
            anim.SetBool("dashing", false);
        }
        else if (dirX < 0f && isDashing)
        {
            anim.SetBool("dashing", true);
            anim.SetBool("stomping", false);
            sprite.flipX = false;
        }
        else if (dirX > 0f && isDashing)
        {
            anim.SetBool("dashing", true);
            anim.SetBool("stomping", false);
            sprite.flipX = true;
        }
        else if (dirX < 0f && !IsGrounded(dirX) && !isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = false;
        }
        else if (dirX > 0f && !IsGrounded(dirX) && !isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = true;
        }
        else if (dirX < 0f && !IsGrounded(dirX) && isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", true);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = false;
        }
        else if (dirX > 0f && !IsGrounded(dirX) && isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", true);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
            sprite.flipX = true;
        }
        else if (!(dirX < 0f) && !(dirX > 0f) && !IsGrounded(dirX) && isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", true);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
        }
        else if (!(dirX < 0f) && !(dirX > 0f) && !IsGrounded(dirX) && !isJumping)
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", false);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
        }
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("grounded", true);
            anim.SetBool("jumping", false);
            anim.SetBool("dashing", false);
            anim.SetBool("stomping", false);
        }
    }

    // Jump code
    void jump()
    {
        isJumping = true;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight);
        anim.SetInteger("jumpstate", 1);
        PlayerSounds.instance.PlaySound(_jump, .5f);
    }

    // Double jump code
    void doubleJump()
    {
        CanDoubleJump = false;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight / 1.5f);
        isStomping = false;
        PlayerSounds.instance.PlaySound(_jump, .5f);
    }

    // Dash code
    void dash(float dirX)
    {
        isDashing = true;
        extraHVelocity = dirX * maxSpeed * 1.3f;
        rb.velocity = new Vector3(dirX * maxSpeed * 1.5f, 5f);
        PlayerSounds.instance.PlaySound(_dash, .6f);
    }

    // Coyote time for Bounce Dash
    async void dashingCoyote(float dirX)
    {
        for (int i = 0; i < 6; i++) 
        {
            if(Input.GetButtonDown("Jump"))
            {
                extraHVelocity = dirX * maxSpeed * 2.2f;
                rb.velocity = new Vector3(rb.velocity.x, 0);
                transform.position = new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z);
                rb.velocity = new Vector3(dirX * maxSpeed * 2.2f, 5f);
                isBouncing = true;
                anim.SetBool("bouncedashing", true);
                PlayerSounds.instance.PlaySound(_superdash, .4f);
                break;
            }
            await Task.Delay(1);
        }
    }

    // Coyote time for Super Stomp
    async void stompingCoyote()
    {
        for (int i = 0; i < 6; i++) 
        {
            if(Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0);
                transform.position = new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z);
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight * 1.4f);
                isSuperStomping = true;
                anim.SetBool("superstomping", true);
                break;
            }
            await Task.Delay(1);
        }
    }

    // Stomp code
    void stomp()
    {
        isStomping = true;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight * -3);
        PlayerSounds.instance.PlaySound(_stomp, .7f);
    }

    // Checks if the player is at / below the kill plane
    void killplaneCheck()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        if(transform.position.y <= -60f)
        {
            rb.velocity = new Vector3(0f, 0f);
            extraHVelocity = 0;
            transform.position = new Vector3(-8f, 6.5f, 0f);
        }
    }

    // Kills the player
    async void kill()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); 
        rb.velocity = new Vector3(0f, 0f);
        extraHVelocity = 0;
        transform.position = new Vector3(-8f, 6.5f, 0f);
    }


}
