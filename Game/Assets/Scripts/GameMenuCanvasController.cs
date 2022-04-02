using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameMenuCanvasController : MonoBehaviour
{
    [Header("Canvas Elements")]
    public Image dodgeballImage;
    public float dodgeballImageRotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        dodgeballImage.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        DodgeBallRotation(dodgeballImageRotSpeed);
    }

    public void DodgeBallRotation(float speed)
    {
        //rotate image
        dodgeballImage.rectTransform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
