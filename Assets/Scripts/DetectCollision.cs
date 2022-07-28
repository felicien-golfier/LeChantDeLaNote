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
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    //Debug.Log(gameObject.tag);

    //    if (other.gameObject.CompareTag("Projectile"))
    //    {
    //        if (gameObject.tag == "Ennemy")
    //        {
    //            Destroy(other.gameObject);
    //            Destroy(gameObject);
    //        }
    //    }
    //}
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(gameObject.tag);

        if (other.CompareTag("Projectile"))
        {
            if (gameObject.tag == "Ennemy")
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}