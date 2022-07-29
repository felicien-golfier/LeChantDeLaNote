using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DetectCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsHost || !other.CompareTag("Projectile") || gameObject == other.GetComponent<ProjectileBehavior>().player)
            return;

        if (gameObject.tag == "Player")
        {
            Destroy(other.gameObject);
            ScoreManager.AddScore(5);
        }
        else if (gameObject.tag == "Ennemy")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            ScoreManager.AddScore(1);
        }
        
    }
}