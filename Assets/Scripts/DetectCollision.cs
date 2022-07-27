using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (gameObject.tag =="Ennemy")
                Destroy(other.gameObject);
                Destroy(gameObject);
        }

    }

    //void substractLive(int lifes)
    //{
    //    healthPoint -= lifes;
    //}

}
