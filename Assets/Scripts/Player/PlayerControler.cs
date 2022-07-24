using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControler : MonoBehaviour
{

    public float horizontalInput;
    public float verticalInput;
    public float speed = 15.0f;
    public string[] direction;

    // Useless for now
    private readonly float limitX = 20.0f;
    private readonly float limitZUp = 15.0f;
    private readonly float limitZDown = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Moving the player
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
        // transform.Translate(Vector3.up * Time.deltaTime * speed * verticalInput);

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        transform.Translate(movement * Time.deltaTime * speed * horizontalInput);

        //Quaternion targetRotation = Quaternion.LookRotation(movement); 
        //targetRotation = Quaternion.RotateTowards(transform.rotation,targetRotation, 360 * Time.fixedDeltaTime); transform.MovePosition(m_Rb.position + movement * speed * Time.fixedDeltaTime);
        //m_Rb.MoveRotation(targetRotation);

    }


}
