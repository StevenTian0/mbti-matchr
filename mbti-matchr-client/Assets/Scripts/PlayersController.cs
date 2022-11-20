using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersController : MonoBehaviour
{
    //reqeuest related
    GameObject thisPlayer;
    GameObject otherPlayer;
    PostPlayerMBTI postPlayerMBTI;
    GameObject dataTransferListener;
    public static readonly HttpClient client = new HttpClient();
    PlayerActionDto playerActionDtoReponse;
    /// <summary>
    /// //////////////////////////////////////////////////////////////
    /// </summary>
    //players controller related
    private Rigidbody2D thisrb;
    private Rigidbody2D otherrb;
    private BoxCollider2D thiscoll;
    private BoxCollider2D othercoll;
    private SpriteRenderer thissprite;
    private SpriteRenderer othersprite;
    private Animator thisAnim;
    private Animator otherAnim;
    public float thispositionX;
    public float otherpositionX;
    public float thispositionY;
    public float otherpositionY;
    public float thispositionZ;
    public float otherpositionZ;

    [SerializeField] private LayerMask jumpableGround;

    private float thisdirX = 0f;
    private float otherdirX = 0f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 17f;
    bool GameIsStarted = false;

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////
    /// </summary>
    public enum MovementState { idle, running, jumping, falling }
    public MovementState thisState;
    public MovementState otherState;

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////
    /// </summary>
    private void Awake()
    {
        dataTransferListener = GameObject.Find("Data Transfer Listener");
        postPlayerMBTI = dataTransferListener.GetComponent<PostPlayerMBTI>();

        //Debug.Log("Room id: " + postPlayerMBTI.res.gameroom_index);
        //Debug.Log("Local pid: " + postPlayerMBTI.pid);

        if (postPlayerMBTI.pid.Equals("1"))
        {
            thisPlayer = GameObject.FindGameObjectWithTag("Moose");
            otherPlayer = GameObject.FindGameObjectWithTag("Racoon");
        }
        else
        {
            thisPlayer = GameObject.FindGameObjectWithTag("Racoon");
            otherPlayer = GameObject.FindGameObjectWithTag("Moose");
        }
        //Debug.Log("Deactivating the other player's controller...");

    }

    private void Start()
    {
        thisrb = thisPlayer.GetComponent<Rigidbody2D>();
        thiscoll = thisPlayer.GetComponent<BoxCollider2D>();
        thissprite = thisPlayer.GetComponent<SpriteRenderer>();
        thisAnim = thisPlayer.GetComponent<Animator>();

        otherrb = otherPlayer.GetComponent<Rigidbody2D>();
        othercoll = otherPlayer.GetComponent<BoxCollider2D>();
        othersprite = otherPlayer.GetComponent<SpriteRenderer>();
        otherAnim = otherPlayer.GetComponent<Animator>();

        if (this.tag == "Moose") jumpableGround = LayerMask.GetMask("Structure", "Racoon");
        else jumpableGround = LayerMask.GetMask("Structure", "Moose");
    }

    public void Update()
    {
        //Debug.Log("Updating ...... ");
        thisdirX = Input.GetAxisRaw("Horizontal");
        thisrb.velocity = new Vector2(thisdirX * moveSpeed, thisrb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            thisrb.velocity = new Vector2(thisrb.velocity.x, jumpForce);
        }
        getOtherPlayerMovementInfo();
        UpdatethisAnimationState();
        UpdateOtherAnimationState();                     
    }

    private async void getOtherPlayerMovementInfo()
    {
        //Debug.Log("Updating .....");
        string playerActionJsonResponse = await SendPlayerActionRequest();
        //Debug.Log("Receiving.....");
        playerActionDtoReponse = PlayerActionDto.CreateFromJSON(playerActionJsonResponse);
        //Debug.Log(playerActionDtoReponse.positionX);
        //Debug.Log(playerActionDtoReponse.positionY);

        otherPlayer.transform.position = new Vector3(playerActionDtoReponse.positionX, playerActionDtoReponse.positionY,
            playerActionDtoReponse.positionZ);

        otherState = (MovementState)playerActionDtoReponse.state;
    }

    private void UpdatethisAnimationState()
    {
        if (thisdirX > 0f)
        {
            thisState = MovementState.running;
            thissprite.flipX = false;
        }
        else if (thisdirX < 0f)
        {
            thisState = MovementState.running;
            thissprite.flipX = true;
        }
        else
        {
            thisState = MovementState.idle;
        }

        if (thisrb.velocity.y > .1f)
        {
            thisState = MovementState.jumping;
        }
        else if (thisrb.velocity.y < -.1f)
        {
            thisState = MovementState.falling;
        }

        thisAnim.SetInteger("States", (int)thisState);
    }

    private void UpdateOtherAnimationState()
    {
        if (otherdirX > 0f)
        {
            otherState = MovementState.running;
            othersprite.flipX = false;
        }
        else if (otherdirX < 0f)
        {
            otherState = MovementState.running;
            othersprite.flipX = true;
        }
        else
        {
            otherState = MovementState.idle;
        }

        if (thisrb.velocity.y > .1f)
        {
            otherState = MovementState.jumping;
        }
        else if (thisrb.velocity.y < -.1f)
        {
            otherState = MovementState.falling;
        }

        otherAnim.SetInteger("States", (int)otherState);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon")
        {
            if (transform.position.y > other.gameObject.transform.position.y)
            {
                transform.position += new Vector3(0f, 0f, 1f);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon")
        {
            if (transform.position.y > other.gameObject.transform.position.y)
            {
                transform.position -= new Vector3(0f, 0f, 1f);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(thiscoll.bounds.center, thiscoll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private async Task<string> SendPlayerActionRequest()
    {
        PlayerActionDto playerActionDto = new PlayerActionDto
        (
            thisPlayer.transform.position.x,
            thisPlayer.transform.position.y,
            thisPlayer.transform.position.z,
            (int)thisState
        );

        string playerActionDtoJson = playerActionDto.SaveToString();

        var values = new Dictionary<string, string>
        {
            {"roomId", postPlayerMBTI.res.gameroom_index.ToString()},
            {"pid", postPlayerMBTI.pid},
            {"data", playerActionDtoJson}
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/game/update", postPlayerMBTI.res.server_host, postPlayerMBTI.res.server_port), content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
    
}
