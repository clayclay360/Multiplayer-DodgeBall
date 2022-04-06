using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DodeballScript : MonoBehaviour, IPunObservable
{
    public float speed = 25;
    public bool isCollectable = true;
    public bool isDamageable = false;
    public bool isHidden = false;
    public PlayerController owner;

    public Collider2D triggerCollider;
    
    [HideInInspector]
    public  PhotonView view;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        triggerCollider.GetComponent<Collider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        triggerCollider.enabled = isCollectable;

        if (isHidden)
        {
            transform.position = new Vector2(100,100);
            //owner.dodgeBall = gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (!playerController.hasBall && isCollectable)
            {
                //if collides with player cause the ball to disappear and player has ball equals true
                //owner = playerController;
                playerController.GetBall(gameObject);
                isCollectable = false;
                isHidden = true;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.SendNext(isCollectable);
            stream.SendNext(isDamageable);
            stream.SendNext(isHidden);
        }
        else if (stream.IsReading)
        {
            isCollectable = ((bool)stream.ReceiveNext());
            isDamageable = ((bool)stream.ReceiveNext());
            isHidden = ((bool)stream.ReceiveNext());
        }
    }
}
