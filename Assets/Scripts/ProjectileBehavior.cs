using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{

    public float projectileSpeed = 10.0f;
    public float projectileRange = 20f;
    private float projectileLifeTime;
    public GameObject player;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = player.GetComponent<PlayerControler>().orientationVector;
        projectileLifeTime =  projectileRange/ projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Mathf.Pow(player.transform.position.x - transform.position.x,2) + Mathf.Pow(player.transform.position.y - transform.position.y,2) >= projectileRange)
        StartCoroutine(Tools.RoutineCallFunctionAfterTime(Destroy, gameObject, projectileLifeTime));
        transform.Translate(projectileSpeed * Time.deltaTime * direction);
    }
}
