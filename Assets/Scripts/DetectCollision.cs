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

        GameObject OriginPlayer = other.GetComponent<ProjectileBehavior>().player;
        ulong ClientId = OriginPlayer.GetComponent<NetworkBehaviour>().OwnerClientId;
        if (gameObject.tag == "Player")
        {
            if (NetworkManager.Singleton.IsHost)
                Destroy(other.gameObject);
            else
                other.gameObject.SetActive(false);

            ScoreManager.instance.AddScore(5, ClientId, OriginPlayer);
        }
        else if (gameObject.tag == "Ennemy")
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
            else
                other.gameObject.SetActive(false);

            ScoreManager.instance.AddScore(1, ClientId, OriginPlayer);
        }
    }
}