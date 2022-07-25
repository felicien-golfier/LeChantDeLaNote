using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerControler : MonoBehaviour
{

    public float speed = 15.0f;
    private float horizontalInput;
    private float verticalInput;
    private string[] direction;

    // Useless for now
    private readonly float limitX = 20.0f;
    private readonly float limitZUp = 15.0f;
    private readonly float limitZDown = 0.0f;

    private bool isLocalPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
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
        // Moving the player
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (verticalInput == 0 && horizontalInput == 0)
            return;

        Vector3 oldPos = transform.position;
        float frameSpeed = Time.deltaTime * speed;
        Vector3 frameTranslation = new Vector3(frameSpeed * horizontalInput, frameSpeed * verticalInput, 0);
        float acos = Mathf.Acos(frameTranslation.normalized.x);
        float sign = Mathf.Sign(Mathf.Sin(frameTranslation.normalized.y));
        float radAngle = acos * sign;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg* acos *sign -90);
        transform.position = transform.position + frameTranslation;
        //Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        //transform.Translate(movement * Time.deltaTime * speed * horizontalInput);

        //Quaternion targetRotation = Quaternion.LookRotation(movement); 
        //targetRotation = Quaternion.RotateTowards(transform.rotation,targetRotation, 360 * Time.fixedDeltaTime); transform.MovePosition(m_Rb.position + movement * speed * Time.fixedDeltaTime);
        //m_Rb.MoveRotation(targetRotation);

    }


}
