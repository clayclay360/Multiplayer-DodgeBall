using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallContainer_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DodeballScript[] balls = GetComponentsInChildren<DodeballScript>();
        foreach(DodeballScript b in balls)
        {
            b.view.RequestOwnership();
            Debug.Log(b.view.CreatorActorNr);
        }
    }
}
