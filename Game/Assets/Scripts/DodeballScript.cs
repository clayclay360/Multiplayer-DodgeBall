using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DodeballScript : MonoBehaviour
{

    public bool canMove = false;
    public float speed = 25;
    public Vector2 direction = Vector2.zero;

    private Rigidbody2D rb;
    private PhotonView view;
    private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (canMove /*&& view.IsMine*/)
        //{
        //    Movement(direction);
        //}
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.tag == "Player" && !canMove)
    //    {
    //        PlayerController playerController = collision.collider.GetComponent<PlayerController>();
    //        playerController.hasBall = true;
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("Existing");
        col.isTrigger = false;
    }

    //public void Direction(Vector2 dir)
    //{
    //    direction = dir;
    //}

    //public void Movement(Vector2 dir)
    //{
    //    transform.Translate(dir * speed * Time.deltaTime);
    //}
}
