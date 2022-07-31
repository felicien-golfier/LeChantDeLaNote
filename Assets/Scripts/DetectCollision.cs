using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DetectCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Projectile Case
        if (other.CompareTag("Projectile") 
            && gameObject != other.GetComponent<ProjectileBehavior>().player 
            && !other.GetComponent<ProjectileBehavior>().player.GetComponent<PlayerControler>().isDead)
        {
            GameObject OriginPlayer = other.GetComponent<ProjectileBehavior>().player;
            ulong ClientId = OriginPlayer.GetComponent<NetworkBehaviour>().OwnerClientId;
            if (gameObject.tag == "Player")
            {
                // Destroy projetile
                Destroy(other.gameObject);

                //Manage Score
                if (NetworkManager.Singleton.IsHost)
                {
                    gameObject.GetComponent<PlayerControler>().GetHit();
                    ScoreManager.instance.AddScore(5, ClientId, OriginPlayer);
                }
            }
            else if (gameObject.tag == "Ennemy")
            {
                Destroy(other.gameObject);
                if (NetworkManager.Singleton.IsHost)
                {
                    OriginPlayer.GetComponent<PlayerControler>().Dmg();
                    Destroy(gameObject);
                    ScoreManager.instance.AddScore(1, ClientId, OriginPlayer);
                }
            }
        }
        else if (other.CompareTag("Ennemy") && gameObject.CompareTag("Player"))
        {
            if (NetworkManager.Singleton.IsHost)
            {
                gameObject.GetComponent<PlayerControler>().GetHit();
            }
        }
    }
}