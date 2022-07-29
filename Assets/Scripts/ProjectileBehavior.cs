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
        playerAngle = player.GetComponent<PlayerControler>().playerAngle;
        projectileLifeTime =  projectileRange/ projectileSpeed;
        ProjectileForwardVector = new Vector2(-Mathf.Sin(playerAngle*Mathf.PI/180), Mathf.Cos(playerAngle * Mathf.PI / 180));
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Tools.RoutineCallFunctionAfterTime(Destroy, gameObject, projectileLifeTime));
        transform.Translate(projectileSpeed * Time.deltaTime * ProjectileForwardVector);
    }
}
