using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DodeballScript : MonoBehaviour, IPunObservable
{
    public float speed = 25;
    public bool isCollectable = true;
    public bool isDamagable = false;

    public Collider2D triggerCol;
    
    [HideInInspector]
    public  PhotonView view;
    [HideInInspector]
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        triggerCol.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(view.Owner.TagObject);
        GameObject player = (GameObject)view.Owner.TagObject;
        if (player != null && player.GetComponent<PlayerController>().hasBall)
        {
            isCollectable = false;
        }

        triggerCol.enabled = isCollectable;
        if (!isCollectable && !isDamagable)
        {
            transform.position = new Vector2(100, 100);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null &&
            !collision.gameObject.GetComponent<PlayerController>().hasBall && isCollectable)
        {
            //if collides with player cause the ball to disappear and player has ball equals true
            collision.gameObject.GetComponent<PlayerController>().hasBall = true;
            collision.gameObject.GetComponent<PlayerController>().GetBall(gameObject);
        }
    }

    public IEnumerator DisConnectFromPlayer()
    {
        yield return new WaitForSeconds(2f);
        isCollectable = true;
        isDamagable = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.SendNext(isCollectable);
            stream.SendNext(isDamagable);
        }
        else if (stream.IsReading)
        {
            isCollectable = ((bool)stream.ReceiveNext());
            isDamagable = ((bool)stream.ReceiveNext());
        }
    }
}
