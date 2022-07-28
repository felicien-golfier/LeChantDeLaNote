using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerControler : NetworkBehaviour
{
    // Public Variables
    public GameObject projectilePrefab;
    public float playerAngle;
    public float playerSpeed = 15.0f;

    // Private in game usefull variable
    private float hitBoxRadius;
    private float newPlayerAngle = 0.0f;

    // Movement variable
    private VJHandler Joystick;
    private float horizontalInput;
    private float verticalInput;
    
    // Network varible
    private bool isLocalPlayer = false;


    void Start()
    {
        // Getting hitbox characteristics
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        hitBoxRadius = collider.radius;

        // Handling the directionnal stick
        Joystick = VJHandler.instance;
        Color playercolor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = playercolor;
        }

        // Network
        isLocalPlayer = GetComponent<NetworkObject>().IsLocalPlayer;
    }

    void Update()
    {
        // Network
        if (!isLocalPlayer)
            return;

        // Taking care of the support we play on
#if UNITY_ANDROID||UNITY_EDITOR
        horizontalInput = Joystick.InputDirection.x;
        verticalInput = Joystick.InputDirection.y;
#else
        // Moving the player
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
#endif

        // Get the angle of the player
        playerAngle = transform.rotation.eulerAngles.z;
        
        // Projectile Spawn
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 offsetSpawn = new Vector3(-Mathf.Sin(playerAngle*Mathf.PI/180), Mathf.Cos(playerAngle * Mathf.PI / 180), 0);
            GameObject projectile = Instantiate(projectilePrefab, hitBoxRadius * offsetSpawn + transform.position, projectilePrefab.transform.rotation);
            projectile.GetComponent<ProjectileBehavior>().player = gameObject;
        }

        // Player movement handling
        if (verticalInput == 0 && horizontalInput == 0)
            return;
        UpdateTransform();
    }



    // Player movement function
    private void UpdateTransform()
    {
        // Orientation and next frame position
        if (verticalInput > 0)
            newPlayerAngle = Mathf.Acos(horizontalInput / (Mathf.Sqrt(Mathf.Pow(horizontalInput, 2) + Mathf.Pow(verticalInput, 2))));
        else if ((verticalInput < 0))
            newPlayerAngle = -Mathf.Acos(horizontalInput / (Mathf.Sqrt(Mathf.Pow(horizontalInput, 2) + Mathf.Pow(verticalInput, 2))));
        else
            newPlayerAngle = transform.rotation.eulerAngles.z;
        newPlayerAngle = newPlayerAngle * 180 / Mathf.PI-90;
        Vector2 frameTrans = new Vector2(horizontalInput, verticalInput) * Time.deltaTime * playerSpeed;
        Vector2 newPossiblePosition = new Vector2(transform.position.x, transform.position.y) + frameTrans;

        // Checking the position of the player regarding the limits and stopping the displacement if required
        transform.rotation = Quaternion.Euler(0f, 0f, newPlayerAngle);
        int? testPosX = TouchLimitX(newPossiblePosition);
        int? testPosY = TouchLimitY(newPossiblePosition);
        Vector2 newPosition = new Vector2(testPosX == null ? newPossiblePosition.x : (Tools.limitX - hitBoxRadius - 0.7f) * testPosX.Value, testPosY == null ? newPossiblePosition.y : (Tools.limitY - hitBoxRadius - 0.7f) * testPosY.Value);

        transform.position = newPosition;

        // Network
        if (IsHost)
            UpdatePosClientRpc(newPosition, newPlayerAngle);

        else
            UpdatePosServerRpc(newPosition, newPlayerAngle);
    }

    [ServerRpc]
    public void UpdatePosServerRpc(Vector2 Position, float Orientation)
    {
        //Transfert position to other players
        UpdatePosClientRpc(Position, Orientation);
    }

    [ClientRpc]
    public void UpdatePosClientRpc(Vector2 Position, float Orientation)
    {
        if (IsLocalPlayer)
            return;

        transform.rotation = Quaternion.Euler(0f, 0f, Orientation);
        transform.position = Position;
    }

    // Player border handling
    int? TouchLimitX(Vector2 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.x) + hitBoxRadius + 0.7f >= Tools.limitX;
        return res ? Math.Sign(positionPlayer.x) : null;
    }

    int? TouchLimitY(Vector2 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.y) + hitBoxRadius + 0.7f >= Tools.limitY;
        return res ? Math.Sign(positionPlayer.y) : null;
    }
};
