using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerControler : MonoBehaviour
{
    public float speed = 15.0f;
    private VJHandler Joystick;
    private float horizontalInput;
    private float verticalInput;
    private string[] direction;

    private bool isLocalPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        Joystick = VJHandler.instance;
        Color playercolor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = playercolor;
        }
        isLocalPlayer = GetComponent<NetworkObject>().IsLocalPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;
#if UNITY_ANDROID||UNITY_EDITOR
        horizontalInput = Joystick.InputDirection.x;
        verticalInput = Joystick.InputDirection.y;
#else
        // Moving the player
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
#endif

        if (verticalInput == 0 && horizontalInput == 0)
            return;

        UpdateTransform();

       
    }

    private void UpdateTransform()
    {


        Vector3 oldPos = transform.position;
        float frameSpeed = Time.deltaTime * speed;
        Vector3 frameTranslation = new Vector3(frameSpeed * horizontalInput, frameSpeed * verticalInput, 0);
        float acos = Mathf.Acos(frameTranslation.normalized.x);
        float sign = Mathf.Sign(Mathf.Sin(frameTranslation.normalized.y));
        float radAngle = acos * sign;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * acos * sign -90);
        transform.position = transform.position + frameTranslation;
    }
}
