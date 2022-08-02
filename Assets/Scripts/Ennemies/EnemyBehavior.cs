using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBehavior : MonoBehaviour
{

    public Animator animator;
    public float speed = 25; 

    GameObject target = null;

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        animator.SetFloat("ennemyMovmentSpeed", 0.0f);

        GetTarget();
        if (target != null)
        {
            MoveToTarget();
            animator.SetFloat("ennemyMovmentSpeed", 2.0f);
        }
    }

    private void OnDestroy()
    {
        SpawnManager.instance.OnEnnemyDeath();
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
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
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
