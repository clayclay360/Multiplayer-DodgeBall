using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [Header("Player Variables:")]
    public string playerName;
    public float speed;
    public float clampMagnitude;
    public GameObject body;
    public bool isAlive, isPaused, hasBall, isOut;
    public int lives = 3;
    public bool isDamagable { get; set; }

    [Header("Team Variables:")]
    public string teamName;
    public Color teamColor;
    public PhotonView view { get; set; }

    [Header("UI Variables:")]
    public Text nameText;
    public GameObject canvas;

    [Header("Ball Variables:")]
    public Transform dodgeBallSpawnTransform;
    public GameObject playerBall;
    
    public float power;
    public string ballName;

    private float xScaleLeft;
    private float xScaleRight;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private TeamManager teamManager;
    private Animator animator;
    private Vector3 mouse_pos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        teamManager = FindObjectOfType<TeamManager>();
        canvas.GetComponent<Canvas>();

        xScaleRight = transform.localScale.x;
        xScaleLeft = -transform.localScale.x;

        nameText.text = view.Owner.NickName;
        view.Owner.TagObject = gameObject;
        isDamagable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (!isPaused && !isOut)
            {
                Movement();
            }
        }
        DisplayBall();
        CheckLives();
        TeamColor();
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
        playerBall.SetActive(hasBall);

        if (hasBall)
        {
            playerBall.GetComponent<SpriteRenderer>().color = teamColor;
        }

        //if player left clicks and is not paused, trigger the throwing animation
        if (Input.GetMouseButtonDown(0) && !isPaused && hasBall && view.IsMine)
        {
            animator.SetTrigger("Throw");
        }
    }

    public void GetBall(GameObject ball)
    {
        ball.GetComponent<DodeballScript>().view.RequestOwnership();
        ballName = ball.GetComponent<DodeballScript>().ballName;
    }

    public void ThrowBall()
    {
        DodeballScript[] ballControllers = FindObjectsOfType<DodeballScript>();
        for (int i = 0; i < ballControllers.Length; i++)
        {
            if (ballControllers[i].view.AmOwner && !ballControllers[i].isCollectable 
                && ballName.Equals(ballControllers[i].ballName))
            {
                ballControllers[i].isDamagable = true;
                ballControllers[i].gameObject.transform.position = dodgeBallSpawnTransform.position;

                Vector2 dir = mouse_pos - dodgeBallSpawnTransform.position;
                dir.Normalize();
                ballControllers[i].rb.AddForce(dir * power, ForceMode2D.Force);



                ballControllers[i].spriteColor = teamName;

                StartCoroutine(ballControllers[i].DisConnectFromPlayer(2));
                break;
            }
        }
    }

    public void DisableBall()
    {
        hasBall = false;
    }

    public void ChangeScene(int index)
    {
        PhotonNetwork.LoadLevel(index);
    }

    public void CheckLives()
    {
        if (isOut)
        {
            transform.position = new Vector2(90, 90);
        }

        if(lives <= 0)
        {
            isOut = true;
        }
    }

    public void TeamColor()
    {
        switch (teamName)
        {
            case "Red":
                teamColor = teamManager.redTeamColor;
                break;
            case "Blue":
                teamColor = teamManager.blueTeamColor;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<DodeballScript>() != null &&
            collision.gameObject.GetComponent<DodeballScript>().spriteColor != teamName &&
            gameManager != null && !isOut && isDamagable)
        {
            lives -= 1;
            isDamagable = false;
            StartCoroutine(collision.gameObject.GetComponent<DodeballScript>().DisConnectFromPlayer(.75f));
            StartCoroutine(Invulnerable());
        }
    }

    private IEnumerator Invulnerable()
    {
        yield return new WaitForSeconds(1);
        isDamagable = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.Serialize(ref hasBall);
            stream.Serialize(ref teamName);
            stream.Serialize(ref playerName);
            stream.Serialize(ref ballName);
            stream.Serialize(ref lives);
            stream.Serialize(ref isOut);
        }
        else if (stream.IsReading)
        {
            hasBall = (bool)stream.ReceiveNext();
            teamName = (string)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            ballName = (string)stream.ReceiveNext();
            lives = (int)stream.ReceiveNext();
            isOut = (bool)stream.ReceiveNext();
        }
    }
}
