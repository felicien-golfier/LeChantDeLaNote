using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMPro.TMP_Text ScoreTxtPrefab;
    private Dictionary<ulong, TMPro.TMP_Text> players = new Dictionary<ulong, TMPro.TMP_Text>();
    
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

        if(NetworkManager.Singleton)
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnection;
    }
    private void OnEnable()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnection;

            if (NetworkManager.Singleton.ConnectedClients.Count > 0)
            {
                foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
                    OnConnection(clientId);
            }
        }   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var sortedPlayers = from entry in players orderby int.Parse(entry.Value.text) ascending select entry;
        players = sortedPlayers.ToDictionary(x => x.Key, x => x.Value);
        uint ind = 0;
        foreach (var player in players)
        {
            player.Value.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ind++ * -20);
        }
    }

    public static void AddScore(uint delta)
    {
        TMPro.TMP_Text PlayerText;

        if (instance.players.TryGetValue(NetworkManager.Singleton.LocalClientId, out PlayerText))
        {
            PlayerText.text = (uint.Parse(PlayerText.text) + delta).ToString();
        }
        else
        {
            Debug.LogError("The client with ID " + NetworkManager.Singleton.LocalClientId + " does not exist in the Players array !!");
            instance.CreateAndAddText(NetworkManager.Singleton.LocalClientId, delta);
        }
    }

    private void OnConnection(ulong clientID)
    {
        TMPro.TMP_Text newScoreTxt;

        if (players.TryGetValue(clientID, out newScoreTxt))
        {
            newScoreTxt.gameObject.SetActive(true);
        }
        else
        {
            newScoreTxt = CreateAndAddText(clientID);
        }
    }

    private TMP_Text CreateAndAddText(ulong clientID, uint score = 0)
    {
        TMP_Text newScoreTxt = Instantiate(ScoreTxtPrefab);
        newScoreTxt.transform.SetParent(gameObject.transform);
        newScoreTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        newScoreTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        newScoreTxt.GetComponent<RectTransform>().localScale = Vector3.one;
        newScoreTxt.text = score.ToString();
        players.Add(clientID, newScoreTxt);
        return newScoreTxt;
    }
}
