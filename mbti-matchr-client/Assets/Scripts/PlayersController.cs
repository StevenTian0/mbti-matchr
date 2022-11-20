using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class PlayersController : MonoBehaviour
{
    //reqeuest related
    GameObject thisPlayer;
    GameObject otherPlayer;
    PostPlayerMBTI postPlayerMBTI;
    GameObject dataTransferListener;
    FinishPoint finishPoint;
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
    private int won = 0;
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
    int reset = 0;

    public float thisInitX;
    public float thisInitY;
    public float thisInitZ;
    public float otherInitX;
    public float otherInitY;
    public float otherInitZ;

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
        GameObject.Find("Winning Screen").SetActive(false);
        Destroy(GameObject.Find("Animation"));
        dataTransferListener = GameObject.Find("Data Transfer Listener");
        postPlayerMBTI = dataTransferListener.GetComponent<PostPlayerMBTI>();
        finishPoint = GameObject.Find("Finish").GetComponent<FinishPoint>();
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

        if (thisPlayer.tag == "Moose") jumpableGround = LayerMask.GetMask("Structure", "Racoon");
        else jumpableGround = LayerMask.GetMask("Structure", "Moose");

        thisInitX = thisPlayer.transform.position.x;
        thisInitY = thisPlayer.transform.position.y;
        thisInitZ = thisPlayer.transform.position.z;
        otherInitX = otherPlayer.transform.position.x;
        otherInitY = otherPlayer.transform.position.y;
        otherInitZ = otherPlayer.transform.position.z;
    }

    public void Update()
    {
        if (won == 0){
            if (finishPoint.arrived == 2) {
                won = 1;
                Win();
            }
            else {
                if (Input.GetButtonDown("Fire1"))
                {
                    Debug.Log("ctrl is pressed");
                    GSUpdate(1);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

                thisdirX = Input.GetAxisRaw("Horizontal");
                thisrb.velocity = new Vector2(thisdirX * moveSpeed, thisrb.velocity.y);

                if (Input.GetButtonDown("Jump") && IsGrounded())
                {
                    thisrb.velocity = new Vector2(thisrb.velocity.x, jumpForce);
                }
                GSUpdate(0);
                UpdatethisAnimationState();
                UpdateOtherAnimationState();
            }
        }
    }

    private void GSUpdate(int reset)
    {
        PlayerActionDto playerActionDto;
        if (reset == 1)
        {
            playerActionDto = new PlayerActionDto
            (
                thisInitX,
                thisInitY,
                thisInitZ,
                0,
                reset
            );
        }
        else
        {
            playerActionDto = new PlayerActionDto
            (
                thisPlayer.transform.position.x,
                thisPlayer.transform.position.y,
                thisPlayer.transform.position.z,
                (int)thisState,
                reset
            );
        }

        string playerActionDtoJson = playerActionDto.SaveToString();
        byte[] buf = System.Text.Encoding.UTF8.GetBytes(PostPlayerMBTI.BZUPDATE + PostPlayerMBTI.gsPid + playerActionDtoJson);

        PostPlayerMBTI.GSConnect();
        NetworkStream stream = PostPlayerMBTI.gsClient.GetStream();
        stream.Write(buf, 0, buf.Length);

        buf = new byte[1024];
        string responseData = string.Empty;
        int bytes = stream.Read(buf, 0, buf.Length);
        PostPlayerMBTI.gsClient?.Close();

        responseData = System.Text.Encoding.UTF8.GetString(buf, 0, bytes);

        // take care of responseData = "{}"
        if (responseData.Equals("{}"))
        {
            otherPlayer.transform.position = new Vector3(otherInitX, otherInitY, otherInitZ);
        }

        playerActionDtoReponse = PlayerActionDto.CreateFromJSON(responseData);

        otherPlayer.transform.position = new Vector3(playerActionDtoReponse.positionX, playerActionDtoReponse.positionY,
            playerActionDtoReponse.positionZ);

        otherState = (MovementState)playerActionDtoReponse.state;

        if (playerActionDtoReponse.reset == 1)
        {
            Debug.Log("ctrl is pressed");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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

        if (otherrb.velocity.y > .1f)
        {
            otherState = MovementState.jumping;
        }
        else if (otherrb.velocity.y < -.1f)
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

    private void Win() {
        GameObject.Find("Winning Screen").SetActive(true);
    }
}
