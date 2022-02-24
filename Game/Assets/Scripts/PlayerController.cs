using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Player Variables:")]
    public string name;
    public float speed;
    public float clampMagnitude;
    public GameObject body;
    public bool isAlive;

    [Header("UI Variables:")]
    public Text nameText;
    public GameObject canvas;

    private float xScaleLeft;
    private float xScaleRight;
    private Vector2 canvasOffset;

    private Rigidbody2D rb;
    private PhotonView view;
    private GameManager gameManager;
    private Animator animator;

    private void Awake()
    {
        canvasOffset = transform.position - canvas.transform.position;
    }

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

        //canvasOffset = transform.position - canvas.transform.position;

        addPlayer();
        nameText.text = name;
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
            Movement();
            CanvasMovement();
        }
    }

    private void CanvasMovement()
    {
        canvas.transform.position = (Vector2)transform.position + canvasOffset;
    }

    private void Movement()
    {
        float move_x = Input.GetAxisRaw("Horizontal");
        float move_y = Input.GetAxisRaw("Vertical");

        if (move_x < 0)
        {
            transform.localScale = new Vector2(xScaleLeft, transform.localScale.y);
        }
        else if (move_x > 0)
        {
            transform.localScale = new Vector2(xScaleRight, transform.localScale.y);
        }

        Vector2 movement = new Vector2(move_x, move_y);
        rb.AddForce(movement.normalized * speed * Time.deltaTime);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, clampMagnitude);

        
        animator.SetBool("Idle", isAlive);
    }
}
