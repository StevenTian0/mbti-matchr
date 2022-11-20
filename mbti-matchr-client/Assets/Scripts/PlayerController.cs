using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    public float positionX;
    public float positionY;
    public float positionZ;


    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 17f;

    public enum MovementState { idle, running, jumping, falling }
    public MovementState state;

    [SerializeField] private AudioSource jumpSoundEffect;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (this.tag == "Moose") jumpableGround = LayerMask.GetMask("Structure", "Racoon");
        else jumpableGround = LayerMask.GetMask("Structure", "Moose");
    }

    // Update is called once per frame
    private void Update()
    {
        positionX = transform.position.x;
        positionY = transform.position.y;
        positionZ = transform.position.z;

        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            //jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("States", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon") {
            if (transform.position.y > other.gameObject.transform.position.y) {
                transform.position += new Vector3(0f,0f,1f);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon") {
            if (transform.position.y >  other.gameObject.transform.position.y) {
                transform.position -= new Vector3(0f,0f,1f);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
