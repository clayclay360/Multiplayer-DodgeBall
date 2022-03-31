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


    [Header("UI Variables:")]
    public Text nameText;
    public GameObject canvas;

    [Header("Ball Variables:")]
    public GameObject dodgeBallPrefab;
    public Transform dodgeBallSpawnTransform;
    public float power;

    private float xScaleLeft;
    private float xScaleRight;

    private Rigidbody2D rb;
    private PhotonView view;
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
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
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
        float move_x = Input.GetAxisRaw("Horizontal");
        float move_y = Input.GetAxisRaw("Vertical");

        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dir_x = mouse_pos.x - transform.position.x;

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

        Vector2 movement = new Vector2(move_x, move_y);
        rb.AddForce(movement.normalized * speed * Time.deltaTime);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, clampMagnitude);
        
        animator.SetBool("Idle", isAlive);
    }

    private void DisplayBall()
    {
        if (hasBall)
        {
            playerBall.SetActive(true);

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

    public void ThrowBall()
    {
        if (view.IsMine)
        {
            Vector2 dir = mouse_pos - dodgeBallSpawnTransform.position;
            dir.Normalize();

            GameObject ball = PhotonNetwork.Instantiate(dodgeBallPrefab.name, dodgeBallSpawnTransform.position, Quaternion.identity);
            Rigidbody2D ballRigidBody = ball.GetComponent<Rigidbody2D>();
            Collider2D ballCollider = ball.GetComponent<Collider2D>();

            ballCollider.isTrigger = true;
            hasBall = false;

            //StartCoroutine(EnableTrigger(2f, ballCollider));
            ball.GetComponent<DodeballScript>().canMove = true;
            ballRigidBody.AddForce(dir * power);
        }
    }

    IEnumerator EnableTrigger(float time, Collider2D col)
    {
        yield return new WaitForSeconds(time);
        col.isTrigger = false;
    }

    public void DisableBall()
    {
        playerBall.SetActive(true);
    }
}
