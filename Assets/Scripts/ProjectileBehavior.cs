using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{

    public float projectileSpeed = 10.0f;
    public float projectileRange = 5.0f;
    private float projectileLifeTime;
    public GameObject player;
    private float playerAngle;

    // Start is called before the first frame update
    void Start()
    {
        playerAngle = player.GetComponent<PlayerControler>().playerAngle;
        projectileLifeTime =  projectileRange/ projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Tools.RoutineCallFunctionAfterTime(Destroy, gameObject, projectileLifeTime));
        Vector2 newProjectilePosition = new Vector2(-Mathf.Sin(playerAngle*Mathf.PI/180), Mathf.Cos(playerAngle * Mathf.PI / 180));
        transform.Translate(projectileSpeed * Time.deltaTime * newProjectilePosition);
    }
}
