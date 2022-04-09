using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DodeballScript : MonoBehaviour, IPunObservable
{
    public float speed = 25;
    public bool isCollectable = true;
    public bool isDamagable = false;
    public string spriteColor;

    public Collider2D triggerCol;
    
    [HideInInspector]
    public  PhotonView view;
    [HideInInspector]
    public Rigidbody2D rb;

    private TeamManager teamManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        triggerCol.GetComponent<Collider2D>();
        teamManager = FindObjectOfType<TeamManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

        ColorChange();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null &&
            !collision.gameObject.GetComponent<PlayerController>().hasBall && isCollectable)
        {
            //if collides with player cause the ball to disappear and player has ball equals true
            collision.gameObject.GetComponent<PlayerController>().hasBall = true;
            collision.gameObject.GetComponent<PlayerController>().GetBall(gameObject);
            spriteColor = collision.gameObject.GetComponent<PlayerController>().teamName;
        }
    }

    public void ColorChange()
    {
        switch (spriteColor)
        {
            case "Red":
                GetComponent<SpriteRenderer>().color = teamManager.redTeamColor;
                break;

            case "Blue":
                GetComponent<SpriteRenderer>().color = teamManager.blueTeamColor;
                break;
            default:
                GetComponent<SpriteRenderer>().color = Color.white;
                break;
        }
    }

    public IEnumerator DisConnectFromPlayer()
    {
        yield return new WaitForSeconds(2f);
        isCollectable = true;
        isDamagable = false;
        spriteColor = "";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //read and write state of the object
        if (stream.IsWriting)
        {
            stream.SendNext(isCollectable);
            stream.SendNext(isDamagable);
            stream.Serialize(ref spriteColor);
        }
        else if (stream.IsReading)
        {
            isCollectable = ((bool)stream.ReceiveNext());
            isDamagable = ((bool)stream.ReceiveNext());
            spriteColor = ((string)stream.ReceiveNext());
        }
    }
}
