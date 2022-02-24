using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    [Header("Animal Info:")]
    public float speed;
    public float rotateSpeed;

    [Header("Timers:")]
    public float lookAtTimerMin;
    public float lookAtTimerMax;
    public float stunRecovery;

    public Transform[] Target;

    private float range;
    private int closestTargetIndex;

    private bool isLooking;
    private bool isRunning;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();

        StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        LookAt();
        RunTowards();
    }

    private void LookAt()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                Target[i] = players[i].transform;
            }
        }

        range = 100;

        for(int i = 0; i < Target.Length; i++)
        {
            if(range > Vector2.Distance(transform.position, Target[i].position))
            {
                range = Vector2.Distance(transform.position, Target[i].position);
                closestTargetIndex = i;
            }
        }

        if (isLooking)
        {
            if (transform.rotation.eulerAngles.z < 180)
            {
                spriteRenderer.flipY = true;
            }
            else
            {
                spriteRenderer.flipY = false;
            }

            Vector2 dir = (Vector2)Target[closestTargetIndex].position - rb.position;
            dir.Normalize();
            float rotateAmount = Vector3.Cross(dir, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
    }

    private void RunTowards()
    {
        if (isRunning)
        {
            rb.velocity = transform.up * speed;
        }
    }

    private IEnumerator Loop()
    {
        //while (gameManager.gameStarted)
        //{
            float lookAtTime = Random.Range(lookAtTimerMin, lookAtTimerMax);
            isLooking = true;
            yield return new WaitForSeconds(lookAtTime);
            isLooking = false;
            isRunning = true;
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            if (isRunning /*&& gameManager.gameStarted*/)
            {
                isRunning = false;
                rb.velocity = Vector2.zero;
                StartCoroutine(Loop());
            }
        }
    }
}
