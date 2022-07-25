using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] ennemiPrefabs;
    public int maxEnnemies = 5;
    private float startDelay = 2.0f;
    private float spawnInterval = 3.0f;
    private int nbEnnemy = 1;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnnemy", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Spawn an animal
    void SpawnEnnemy()
    {
    
        if (nbEnnemy <= Tools.maxEnnemy)
        { 
            Spawn();
            nbEnnemy += 1;
        }
    }

    // Function to spawn animal on top of the scene
    void Spawn()
    {
        float posX = Random.Range(0, Tools.limitX);
        float posY = Random.Range(0, Tools.limitY);
        Vector3 position = new Vector3(posX, posY, 0);
        int ennemiIndex = Random.Range(0, ennemiPrefabs.Length);
        Instantiate(ennemiPrefabs[ennemiIndex], position, Quaternion.identity);
    }
}

