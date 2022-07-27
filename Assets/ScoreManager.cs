using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class ScoreManager : MonoBehaviour
{
    private NetworkManager networkManager;
    public TMPro.TMP_Text ScoreTxtPrefab;
    private Dictionary<ulong, TMPro.TMP_Text> players = new Dictionary<ulong, TMPro.TMP_Text>();
    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.Singleton;
        networkManager.OnClientConnectedCallback += OnConnection;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var sortedPlayers = from entry in players orderby entry.Value ascending select entry;
        players = sortedPlayers.ToDictionary(x => x.Key, x => x.Value);
        uint ind = 0;
        foreach (var player in players)
        {
            //player.Value.GetComponent().
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
            newScoreTxt = Instantiate(ScoreTxtPrefab);
            newScoreTxt.transform.parent = gameObject.transform;
            newScoreTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            newScoreTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            newScoreTxt.GetComponent<RectTransform>().localScale = Vector3.one;
            players.Add(clientID, newScoreTxt);
        }
    }
}
