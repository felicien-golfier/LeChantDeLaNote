using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileBehavior : MonoBehaviour
{

    public float projectileSpeed = 10.0f;
    public float projectileRange = 5.0f;
    private float projectileLifeTime;
    public GameObject player;
    private float playerAngle;
    Vector2 ProjectileForwardVector;

    // Start is called before the first frame update
    void Start()
    {
        playerAngle = player.GetComponent<PlayerControler>().GetPlayerAngle();
        projectileLifeTime =  projectileRange/ projectileSpeed;
        ProjectileForwardVector = new Vector2(-Mathf.Sin(playerAngle*Mathf.PI/180), Mathf.Cos(playerAngle * Mathf.PI / 180));
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Tools.RoutineCallFunctionAfterTime(Destroy, gameObject, projectileLifeTime));
        transform.rotation = Quaternion.Euler(0f, 0f, playerAngle * Mathf.PI / 180);
        transform.Translate(projectileSpeed * Time.deltaTime * ProjectileForwardVector);
    }
}
