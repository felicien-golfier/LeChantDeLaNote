using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CameraControler : MonoBehaviour
{
    private float xSizeCamera;
    private float ySizeCamera;

    // Start is called before the first frame update
    void Start()
    {
        ySizeCamera = Camera.main.orthographicSize * 2; // Heuteur
        xSizeCamera = ySizeCamera * Camera.main.aspect;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClient != null)
            FollowPlayer();    
        else 
           Camera.main.transform.position = new Vector3(0,0,Tools.zCamera);
    
    }


    void FollowPlayer()
    {
        Vector3 positionPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;

        int? testPosX = TouchLimitX(positionPlayer);
        int? testPosY = TouchLimitY(positionPlayer);
        Camera.main.transform.position = new Vector3(testPosX==null ? positionPlayer.x : (Tools.limitX - xSizeCamera/2) * testPosX.Value, testPosY == null ? positionPlayer.y : (Tools.limitY - ySizeCamera/2) * testPosY.Value, Tools.zCamera);
    }

    int? TouchLimitX(Vector3 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.x) + xSizeCamera / 2 >= Tools.limitX;
        return res ? Math.Sign(positionPlayer.x): null;
    }

    int? TouchLimitY(Vector3 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.y) + ySizeCamera / 2 >= Tools.limitY;
        return res ? Math.Sign(positionPlayer.y) : null;
    }

}
