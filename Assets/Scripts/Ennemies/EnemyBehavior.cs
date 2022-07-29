using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBehavior : MonoBehaviour
{

    public float speed = 25; 

    NetworkManager networkManager;
    GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    // Update is called once per frame
    void Update()
    {
        if (!networkManager.IsHost)
            return;

        GetTarget();
        if (target != null)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        float frameSpeed = Time.deltaTime * speed;
        Vector3 vectorToTarget = target.transform.position - transform.position;
        vectorToTarget.Normalize();
        Vector3 frameTranslation = vectorToTarget * frameSpeed;
        transform.position = transform.position + frameTranslation;
    }

    private void GetTarget()
    {
        GameObject possibleTarget = null;
        float minSquaredDist = float.MaxValue;
        foreach (NetworkClient client in networkManager.ConnectedClientsList)
        {
            if (client.PlayerObject == null)
                continue;

            float squaredDist = Tools.SquaredDist(client.PlayerObject.transform.position, transform.position);
            if (squaredDist < Tools.AggroSquaredDist && squaredDist < minSquaredDist)
            {
                possibleTarget = client.PlayerObject.gameObject;
                minSquaredDist = squaredDist;
            }
        }

        if (possibleTarget != null)
            target = possibleTarget;
    }
}
