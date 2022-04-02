using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DodeballScript : MonoBehaviour, IPunObservable
{
    public float speed = 25;

    private Rigidbody2D rb;
    private PhotonView view;
    private Collider2D col;
    private SpriteRenderer SR;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        col = GetComponent<Collider2D>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();

            if (!playerController.hasBall)
            {
                //if collides with player cause the ball to disappear and player has ball equals true
                playerController.hasBall = true;
                playerController.GetBall(gameObject);
                col.isTrigger = true;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        col.isTrigger = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.SendNext(SR.enabled);
            stream.SendNext(col.isTrigger);
        }
        else if (stream.IsReading)
        {
            SR.enabled = (bool)stream.ReceiveNext();
            col.isTrigger = (bool)stream.ReceiveNext();
        }
    }
}
