using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraControler : MonoBehaviour
{
    private float xSizeCamera;
    private float ySizeCamera;
    private float limitX = 100f;
    private float limiteY = 50f;

    // Start is called before the first frame update
    void Start()
    {
        ySizeCamera = Camera.main.orthographicSize * 2; // Heuteur
        xSizeCamera = ySizeCamera * Camera.main.aspect;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
            FollowPlayer();    
        else 
           Camera.main.transform.position = new Vector3(0,0,-10);
    
    }


    void FollowPlayer()
    {

        //if (!TouchLimit())
        //{
        Vector3 positionPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;
        Camera.main.transform.position = new Vector3(positionPlayer.x, positionPlayer.y,-10);
        //}

    }

    //bool TouchLimit()
    //{
    //    if ()
    //        return True
    //}
        
}
