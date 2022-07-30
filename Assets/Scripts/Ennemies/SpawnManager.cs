using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] ennemiPrefabs;
    public float startDelay = 2.0f;
    public float spawnInterval = 3.0f;
    public uint maxEnnemy = 20;
    private uint nbEnnemy = 0;
    static private SpawnManager _instance;
    static public SpawnManager instance
    {
        get
        {
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStart;
    }
    
    void OnServerStart()
    { 
        InvokeRepeating("SpawnEnnemy", startDelay, spawnInterval);
    }

    void SpawnEnnemy()
    {
        if (NetworkManager.Singleton.IsHost && nbEnnemy < maxEnnemy)
        {
            Spawn();
            nbEnnemy += 1;
        }
    }

    public void OnEnnemyDeath()
    {
        nbEnnemy--;
    }
    // Function to spawn animal on top of the scene
    void Spawn()
    {
        if (ennemiPrefabs.Length == 0)
        {
            Debug.LogError("No ennemi prefabs");
            return;
        }
 
        float posX = Random.Range(-Tools.limitX, Tools.limitX);
        float posY = Random.Range(-Tools.limitY, Tools.limitY);
        Vector3 position = new Vector3(posX, posY, 0);
        int ennemiIndex = Random.Range(0, ennemiPrefabs.Length);
        GameObject ennemy = Instantiate(ennemiPrefabs[ennemiIndex], position, Quaternion.identity);
        ennemy.GetComponent<NetworkObject>().Spawn();
    }
}

