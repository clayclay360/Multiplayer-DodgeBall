using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [Header("Player Variables:")]
    public string name;
    public float speed;
    public float clampMagnitude;
    public GameObject body, playerBall;
    public bool isAlive, isPaused, hasBall;
    [HideInInspector]
    public PhotonView view;

    [Header("UI Variables:")]
    public Text nameText;
    public GameObject canvas;

    [Header("Ball Variables:")]
    public GameObject dodgeBall;
    public Transform dodgeBallSpawnTransform;
    public float power;

    private float xScaleLeft;
    private float xScaleRight;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private Animator animator;
    private Vector3 mouse_pos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        canvas.GetComponent<Canvas>();

        xScaleRight = transform.localScale.x;
        xScaleLeft = -transform.localScale.x;

        addPlayer();
        nameText.text = view.Owner.NickName;
    }

    private void addPlayer()
    {
        if (gameManager != null)
        {
            gameManager.playersAlive++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (!isPaused)
            {
                Movement();
            }

            DisplayBall();
            //ChangeBallPosition();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.Serialize(ref hasBall);
            stream.SendNext(playerBall.activeSelf);
        }
        else if (stream.IsReading)
        {
            hasBall = (bool)stream.ReceiveNext();
            playerBall.SetActive((bool)stream.ReceiveNext());
        }
    }

    private void Movement()
    {
        //get inputs
        float move_x = Input.GetAxisRaw("Horizontal");
        float move_y = Input.GetAxisRaw("Vertical");

        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dir_x = mouse_pos.x - transform.position.x;

        //change scale depending on mouse position
        if (/*move_x < 0*/ dir_x < 0)
        {
            transform.localScale = new Vector2(xScaleLeft, transform.localScale.y);
            canvas.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (/*move_x > 0*/ dir_x > 0)
        {
            transform.localScale = new Vector2(xScaleRight, transform.localScale.y);
            canvas.transform.localScale = new Vector3(1, 1, 1);
        }

        //add force to the direction the player is going
        Vector2 movement = new Vector2(move_x, move_y);
        rb.AddForce(movement.normalized * speed * Time.deltaTime);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, clampMagnitude);
        
        //play idle animation
        animator.SetBool("Idle", isAlive);
    }

    private void DisplayBall()
    {
        //if the player has a ball, display the ball
        if (hasBall)
        {
            playerBall.SetActive(true);

            //if player left clicks and is not paused, trigger the throwing animation
            if (Input.GetMouseButtonDown(0) && !isPaused)
            {
                animator.SetTrigger("Throw");
            }
        }
        else
        {
            playerBall.SetActive(false);
        }
    }

    public void GetBall(GameObject ball)
    {
        dodgeBall = ball;
        hasBall = true;
        ball.GetComponent<DodeballScript>().view.RequestOwnership();

        if(dodgeBall == null)
        {
            Debug.Log("errorOccured");
        }

    }

    public void ChangeBallPosition()
    {
        if(dodgeBall != null && hasBall)
        {
            dodgeBall.transform.position = dodgeBallSpawnTransform.position;
        }
    }

    public void ThrowBall()
    {
        if (view.IsMine)
        {
            //get the direction
            Vector2 dir = mouse_pos - dodgeBallSpawnTransform.position;
            dir.Normalize();

            DodeballScript dodgeBallScript = dodgeBall.GetComponent<DodeballScript>();
            Rigidbody2D ballRigidBody = dodgeBall.GetComponent<Rigidbody2D>();

            dodgeBallScript.isHidden = false;
            hasBall = false;

            //add force to the direction in which the ball is supposed to go
            dodgeBall.transform.position = dodgeBallSpawnTransform.position;
            ballRigidBody.AddForce(dir * power);

            Invoke("BallLeft", 2f);
        }
    }

    void BallLeft()
    {
        dodgeBall.GetComponent<DodeballScript>().isCollectable = true;
        //dodgeBall.GetComponent<DodeballScript>().owner = null;
        dodgeBall = null;
        Debug.Log("Collectable");
    }

    public void DisableBall()
    {
        playerBall.SetActive(true);
    }
}
