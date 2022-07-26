using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerControler : NetworkBehaviour
{
    public float speed = 15.0f;
    private VJHandler Joystick;
    private float horizontalInput;
    private float verticalInput;
    private string[] direction;

    private bool isLocalPlayer = false;

    private Vector2 sizeHitbox;
    private Vector2 posHitbox;

    // Start is called before the first frame update
    void Start()
    {

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        sizeHitbox = collider.size;
        posHitbox = collider.offset;

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
        float frameSpeed = Time.deltaTime * speed;
        Vector3 frameTranslation = new Vector3(frameSpeed * horizontalInput, frameSpeed * verticalInput, 0);
        float acos = Mathf.Acos(frameTranslation.normalized.x);
        float sign = Mathf.Sign(Mathf.Sin(frameTranslation.normalized.y));
        float rotation = Mathf.Rad2Deg * acos * sign - 90;
        
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        Vector3 newPosition = transform.position + frameTranslation;

        // Checking the position of the player regarding the limits and stopping the displacement if required
        int? testPosX = TouchLimitX(newPosition);
        int? testPosY = TouchLimitY(newPosition);
        newPosition = new Vector3(testPosX == null ? transform.position.x + frameTranslation.x : (Tools.limitX - (posHitbox[0] + sizeHitbox[0] / 2) - 1) * testPosX.Value, testPosY == null ? transform.position.y + frameTranslation.y : (Tools.limitY - (posHitbox[1] + sizeHitbox[1] / 2) - 1) * testPosY.Value, 0);

        transform.position = newPosition;

        if (IsHost)
        {
            UpdatePosClientRpc(new Vector2(transform.position.x, transform.position.y), rotation);
        }
        else
        {
            UpdatePosServerRpc(new Vector2(transform.position.x, transform.position.y), rotation);
        }
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

    int? TouchLimitX(Vector3 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.x) + posHitbox[0] + sizeHitbox[0] / 2 + 1 >= Tools.limitX;
        return res ? Math.Sign(positionPlayer.x) : null;
    }

    int? TouchLimitY(Vector3 positionPlayer)
    {
        bool res = Math.Abs(positionPlayer.y) + posHitbox[1] + sizeHitbox[1] / 2 + 1 >= Tools.limitY;
        return res ? Math.Sign(positionPlayer.y) : null;
    }
};
