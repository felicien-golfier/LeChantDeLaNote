using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    public TMPro.TMP_Text ScoreTxtPrefab;
    private Dictionary<ulong, TMPro.TMP_Text> playersScoreTxt = new Dictionary<ulong, TMPro.TMP_Text>();
    private Dictionary<ulong, GameObject> players = new Dictionary<ulong, GameObject>();
    public GameObject firstPlayer = null;
    private static ScoreManager _instance;
    public static ScoreManager instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    private void OnDisable()
    {
        if(NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConnection;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnection;
            OnDisconnection(NetworkManager.Singleton.LocalClientId);
        }
    }
    private void OnEnable()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnection;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnection;

            if (NetworkManager.Singleton.LocalClient != null)
            {
                foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
                    OnConnection(clientId);
            }
        }   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playersScoreTxt.Count == 0)
            return;

        var sortedPlayers = from entry in playersScoreTxt orderby int.Parse(entry.Value.text) descending select entry;
        playersScoreTxt = sortedPlayers.ToDictionary(x => x.Key, x => x.Value);
        uint ind = 0;
        foreach (var player in playersScoreTxt)
        {
            player.Value.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ind++ * - Camera.main.pixelHeight / 25);
        }

        if (players.ContainsKey(playersScoreTxt.First().Key))
        {
            firstPlayer = players[playersScoreTxt.First().Key];
        }
    }

    [ClientRpc]
    private void AddScoreClientRpc(int delta, ulong clientId)
    {
        if (!IsLocalPlayer)
            AddScoreLocal(delta, clientId);
    }
    private void AddScoreLocal(int delta, ulong clientId)
    {
        TMPro.TMP_Text PlayerText;

        if (playersScoreTxt.TryGetValue(clientId, out PlayerText))
        {
            PlayerText.text = (int.Parse(PlayerText.text) + delta).ToString();
        }
        else
        {
            Debug.LogError("The client with ID " + clientId + " does not exist in the Players array !!");
            CreateAndAddText(clientId, delta);
        }
    }
    public void AddScore(int delta, ulong clientId, GameObject givenPlayer = null)
    {
        players.TryAdd(clientId, givenPlayer);
        Debug.Log(IsSpawned);
        if (NetworkManager.Singleton.IsHost)
        {
            AddScoreClientRpc(delta, clientId);
            AddScoreLocal(delta, clientId);
        }
    }

    private void OnDisconnection(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            foreach(var  playerTxt in playersScoreTxt)
            {
                Destroy(playerTxt.Value.gameObject);
            }
            playersScoreTxt.Clear();
            players.Clear();
        }
        else
        {
            TMPro.TMP_Text newScoreTxt;
            if (playersScoreTxt.TryGetValue(clientID, out newScoreTxt))
            {
                playersScoreTxt.Remove(clientID);
                Destroy(newScoreTxt);
            }

            GameObject player;
            if (players.TryGetValue(clientID, out player))
            {
                players.Remove(clientID);
            }
        }
    }
    private void OnConnection(ulong clientID)
    {
        TMPro.TMP_Text newScoreTxt;

        if (playersScoreTxt.TryGetValue(clientID, out newScoreTxt))
        {
            newScoreTxt.gameObject.SetActive(true);
        }
        else
        {
            newScoreTxt = CreateAndAddText(clientID);
        }
    }

    private TMP_Text CreateAndAddText(ulong clientID, int score = 0)
    {
        TMP_Text newScoreTxt = Instantiate(ScoreTxtPrefab);
        newScoreTxt.transform.SetParent(gameObject.transform);
        newScoreTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        newScoreTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        newScoreTxt.GetComponent<RectTransform>().localScale = Vector3.one;
        newScoreTxt.text = score.ToString();
        newScoreTxt.transform.GetChild(0).GetComponent<TMP_Text>().text = "Player " + clientID;
        playersScoreTxt.Add(clientID, newScoreTxt);
        return newScoreTxt;
    }
}
