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
    public Animator animator;
    public GameObject FirstPlayerPointer;
    public float distToArrow = 2f;
    public int MaxHealth = 5;
    public float DeathTime = 5;
    public SpriteRenderer mouthSpriteRenderer;

    // Private in game usefull variable
    private float hitBoxRadius;
    private float newPlayerAngle = 0.0f;
    private int Health;
    public bool isDead = false;
    
    // Movement variable
    private VJHandler Joystick;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        // Set arrow
        FirstPlayerPointer = transform.GetChild(0).gameObject;
        FirstPlayerPointer.SetActive(IsLocalPlayer);
        
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

        ScoreManager.instance.AddPlayer(OwnerClientId, gameObject);
        Health = MaxHealth;

        // Positioning the mouth
        playerAngle = transform.rotation.eulerAngles.z;
        transform.GetChild(1).position = transform.position + new Vector3(-hitBoxRadius * Mathf.Sin(playerAngle * Mathf.PI / 180), hitBoxRadius * Mathf.Cos(playerAngle * Mathf.PI / 180));
        transform.GetChild(1).rotation = Quaternion.Euler(0.0f, 0.0f, playerAngle+90.0f);
        mouthSpriteRenderer = transform.Find("PlayerMouth").GetComponent<SpriteRenderer>();

        playerAngle = transform.rotation.eulerAngles.z;
    }

    void Update()
    {

        if (!isDead && IsHost && Health <= 0)
            DeathClientRPC();
        
        if (!IsLocalPlayer)
        {
            return;
        }

        UpdatePlayerPointer();
        // Taking care of the support we play on
        horizontalInput = Joystick.InputDirection.x;
        verticalInput = Joystick.InputDirection.y;

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        
        // Projectile Spawn
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchProjectile();
            animator.SetTrigger("PlayerShoots");
            //animator.ResetTrigger("PlayerShoots");
            animator.Play("PlayerShoot");
        }
        //else
            //animator.Play("PlayerMouthIdle");

        //Player movement handling
        if (verticalInput == 0 && horizontalInput == 0)
        { 
            animator.SetFloat("playerMovmentSpeed", 0.0f);
            return;
        }
        else if (verticalInput != 0 || horizontalInput != 0)
            UpdateTransform();
    }
    public void Dmg()
    {
        if (isDead)
            return;

        SoundManager.instance.PlayDmg(gameObject);
    }
    public void GetHit()
    {
        if (isDead)
            return;

        Health--;
        SoundManager.instance.PlayGetHit(gameObject);
        UpdateTheme();
    }

    private void UpdateTheme()
    {
        if (IsLocalPlayer)
        {
            uint themeChoice = (uint)Mathf.FloorToInt(Health * 5 / (MaxHealth + 1));
            SoundManager.instance.ChangeTheme(Health <= 0 ? 5 : themeChoice);
        }
    }

    [ClientRpc]
    private void DeathClientRPC()
    {
        Death();
    }
    private void Death()
    {
        StartCoroutine(Tools.RoutineCallFunctionAfterTime(Resurection, DeathTime));
        isDead = true;
        playerSpeed /= 2;
        SpriteRenderer SR = transform.Find("PlayerBody").GetComponent<SpriteRenderer>();
        //SpriteRenderer SR = GetComponent<SpriteRenderer>();
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b,.3f);
        ScoreManager.instance.PlayerReset(OwnerClientId);
    }

    private void Resurection()
    {
        playerSpeed *= 2;
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1);
        Health = MaxHealth;
        isDead = false;
        SoundManager.instance.PlayHeal(gameObject);
        UpdateTheme();
    }

    public void LaunchProjectile()
    {
        if (isDead)
            return;

        LaunchProjectileLocal();
        if (NetworkManager.Singleton.IsHost)
        {
            LaunchProjClientRpc();
        }
        else
        {
            LaunchProjServerRpc();
        }
    }

    private void LaunchProjectileLocal()
    {
        Quaternion angleSpawn = Quaternion.Euler(0f, 0f, newPlayerAngle);
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(-hitBoxRadius*1.5f * Mathf.Sin(newPlayerAngle * Mathf.PI / 180), hitBoxRadius * Mathf.Cos(newPlayerAngle * Mathf.PI / 180)) + transform.position, angleSpawn);
        projectile.GetComponent<ProjectileBehavior>().player = gameObject;
        SoundManager.instance.PlayAttack(gameObject);
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
            newPlayerAngle = horizontalInput>0 ? 0 : Mathf.PI;
        newPlayerAngle = newPlayerAngle * 180 / Mathf.PI-90;
        Vector2 frameTrans = new Vector2(horizontalInput, verticalInput) * Time.deltaTime * playerSpeed;
        Vector2 newPossiblePosition = new Vector2(transform.position.x, transform.position.y) + frameTrans;

        // Checking the position of the player regarding the limits and stopping the displacement if required
        playerAngle = newPlayerAngle;

        //transform.rotation = Quaternion.Euler(0f, 0f, newPlayerAngle);
        
        int? testPosX = TouchLimitX(newPossiblePosition);
        int? testPosY = TouchLimitY(newPossiblePosition);
        Vector2 newPosition = new Vector2(testPosX == null ? newPossiblePosition.x : (Tools.limitX - hitBoxRadius - 0.7f) * testPosX.Value, testPosY == null ? newPossiblePosition.y : (Tools.limitY - hitBoxRadius - 0.7f) * testPosY.Value);

        transform.position = newPosition;
        flipMouth(newPlayerAngle);
        animator.SetFloat("playerMovmentSpeed", Mathf.Sqrt(Mathf.Pow(frameTrans.x, 2) + Mathf.Pow(frameTrans.y, 2)));

        // Taking care of the mouth
        transform.GetChild(1).position = transform.position + new Vector3(-hitBoxRadius*1.5f*Mathf.Sin(newPlayerAngle*Mathf.PI/180), hitBoxRadius*1.5f * Mathf.Cos(newPlayerAngle * Mathf.PI / 180));
        transform.GetChild(1).rotation = Quaternion.Euler(0f, 0f, newPlayerAngle+90.0f);

        // Network
        if (IsHost)
            UpdatePosClientRpc(newPosition, newPlayerAngle);

        else
            UpdatePosServerRpc(newPosition, newPlayerAngle);
    }

    public void flipMouth(float angle)
    {
        if (angle <= 0 && angle >= -180)
            mouthSpriteRenderer.flipY = false;
        else
            mouthSpriteRenderer.flipY = true;
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

    [ClientRpc]
    public void LaunchProjClientRpc()
    {
        if (!IsLocalPlayer)
            LaunchProjectileLocal();
    }
    [ServerRpc]
    public void LaunchProjServerRpc()
    {
        LaunchProjClientRpc();
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

    private void UpdatePlayerPointer()
    {
        GameObject firstPlayer = ScoreManager.instance.firstPlayer;
        if (firstPlayer == null || gameObject == firstPlayer)
        {
            FirstPlayerPointer.transform.localPosition = new Vector2(0, distToArrow);
            FirstPlayerPointer.transform.rotation = new Quaternion(0,0,0.7071f, 0.7071f);
        }
        else
        {
            var VectorToFirstPlayer = firstPlayer.transform.position - transform.position;
            FirstPlayerPointer.transform.localPosition = VectorToFirstPlayer.normalized * distToArrow;
            FirstPlayerPointer.transform.rotation = Quaternion.Euler(0,0,Mathf.Rad2Deg * Mathf.Sign(Mathf.Sin(VectorToFirstPlayer.normalized.y)) * Mathf.Acos(VectorToFirstPlayer.normalized.x) +180);
        }
    }
}