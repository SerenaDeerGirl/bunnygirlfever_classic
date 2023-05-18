using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class PlayerMovement : MonoBehaviour
{   
    // Gets UI
    public TMP_Text uiVelocity;

    // Gets various objects and components related to Player
    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public GameObject player;
    public LayerMask groundLayer;

    // Player properties
    public float jumpHeight = 8f;
    public float maxSpeed = 9f;
    public int coyoteLeniency = 8;
    public float extraHVelocity;
    public float extraVVelocity;

    // Animation parameters
    public bool isJumping = false;
    private bool isDashing = false;
    private bool isStomping = false;
    private bool isBouncing = false;
    private bool isCheckingBounce = false;
    private bool isSuperStomping = false;
    private bool CanDoubleJump = false;
    private float stretchXVelocity;
    private float stretchYVelocity;
    public float dirX;
    
    // Sound effects getters
    public AudioClip _jump;
    public AudioClip _stomp;
    public AudioClip _superstomp;
    public AudioClip _dash;
    public AudioClip _superdash;
    public AudioClip _wallslide;

    // Start is called before the first frame update
    // Gets and sets internal components.
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Framerate cap. Proceed with caution.
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    // Controls movement, collision and calls animation control script. Also controls squash/stretch.
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        float horizontalVelocity = dirX * maxSpeed;
        anim.SetFloat("speed", Math.Abs(horizontalVelocity));
        rb.velocity = new Vector2(horizontalVelocity + extraHVelocity, rb.velocity.y + extraVVelocity);
        uiVelocity.text = "Velocity: " + Mathf.Round(rb.velocity.x);
        extraVVelocity = 0;

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

        

        //if(Math.Abs(rb.velocity.x) > 20)
        //{
        //    if(Math.Abs(rb.velocity.x) <= 30)
        //    {
        //        stretchXVelocity = Math.Abs((0.004f * Math.Abs(rb.velocity.x - 20)));
        //    }
        //    else
        //    {
        //        stretchXVelocity =  0.08f;
        //    }
        //}
        //else
        //{
        //    stretchXVelocity = 0;
        //}

        //player.transform.localScale = new Vector3(0.8999999f + stretchXVelocity, 0.8f + stretchYVelocity, 1);

        if(isJumping)
        {
            if(rb.velocity.y < 0)
            {
                anim.SetInteger("jumpstate", 2);
                if(IsGrounded(dirX))
                {
                    isJumping = false;
                }
            }
            else
            {
                anim.SetInteger("jumpstate", 1);
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            if (IsGrounded(dirX) && !isBouncing && !isCheckingBounce)
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
                isCheckingBounce = false;
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
            if(!isStomping)
            {
                dashingCoyote(dirX);
            }
        }
        if(isBouncing)
        {
            anim.SetBool("bouncedashing", false);
            if(!isStomping)
            {
            dashingCoyote(dirX);
            }
            isBouncing = false;
        }
        if(isStomping)
        {
            isStomping = false;
            stompingCoyote();
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

    // NOTE: THIS BARELY WORKS!!!! pls replace with a box-cast, future me <3
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
    // this code is probably the most unoptimized part of the project. actually, nah.. it DEFINITELY is.
    // proceed with caution LOL
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
    // (how exciting!!)
    void jump()
    {
        isJumping = true;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight);
        anim.SetInteger("jumpstate", 1);
        PlayerSounds.instance.PlaySound(_jump, .5f);
    }

    // Double jump code
    // (how BORING!!!!!!!!!!!)
    void doubleJump()
    {
        CanDoubleJump = false;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight / 1.5f);
        isStomping = false;
        isJumping = true;
        PlayerSounds.instance.PlaySound(_jump, .5f);
    }

    // Dash code
    // (somehow one of the simpler scripts??)
    void dash(float dirX)
    {
        isDashing = true;
        extraHVelocity = dirX * maxSpeed * 1.3f;
        rb.velocity = new Vector3(dirX * maxSpeed * 1.5f, 5f);
        PlayerSounds.instance.PlaySound(_dash, .6f);
    }

    // Coyote time for Bounce Dash
    // Essentially, this just runs a for loop for coyoteLeniency times
    // that checks once per frame if the input is being pressed. 
    async void dashingCoyote(float dirX)
    {
        isCheckingBounce = true;
        for (int i = 0; i < coyoteLeniency; i++) 
        {
            if (isCheckingBounce == false)
            {
                break;
            }
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
        isCheckingBounce = false;
    }

    // Coyote time for Super Stomp
    // Essentially, this just runs a for loop for coyoteLeniency times
    // that checks once per frame if the input is being pressed. 
    // (ditto as dashing coyote script)
    async void stompingCoyote()
    {
        for (int i = 0; i < coyoteLeniency; i++) 
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
    // WEEEEEEEEEEEEEEEEEEEEE
    void stomp()
    {
        isStomping = true;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight * -3);
        PlayerSounds.instance.PlaySound(_stomp, .7f);
    }

    // Checks if the player is at / below the kill plane
    // Note: future me pls replace this with a trigger,,,, ty!!!!!! love u future me mwah mwah <3
    void killplaneCheck()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        if(transform.position.y <= -100f)
        {
            rb.velocity = new Vector3(0f, 0f);
            extraHVelocity = 0;
            transform.position = new Vector3(-8f, 6.5f, 0f);
        }
    }

    // Kills the player
    // (DEATH DEATH MURDER KILL PERMENANT OUCHIE CAUSER CODE!!!!!!!!!!!!!)
    async void kill()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); 
        rb.velocity = new Vector3(0f, 0f);
        extraHVelocity = 0;
        transform.position = new Vector3(-8f, 6.5f, 0f);
    }


}
