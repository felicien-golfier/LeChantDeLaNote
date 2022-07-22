using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class UIConnectionManager : MonoBehaviour
{
    [Header("Connection Interface")]
    public TMPro.TextMeshProUGUI connectingText;
    public GameObject connectionPanel;

    private bool isConnecting = false;
    private string[] ConnectionStatusTexts = { "Not Connected", "Connected as CLIENT", "Connected as HOST", "Connection..." };
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Debug.LogError("NO NETWORK MANAGER !!");
            return;
        }
    }

    public void StartClientConnection()
    {
        if (!networkManager || networkManager.IsClient || networkManager.IsServer || isConnecting)
            return;

        networkManager.OnClientConnectedCallback += OnClientConnected;
        connectingText.text = ConnectionStatusTexts[3];
        isConnecting = true;
        networkManager.GetComponent<MyNetworkDiscovery>().StartClient();
    }

    public void StartHostButton()
    {
        if (!networkManager || networkManager.IsClient || networkManager.IsServer || isConnecting)
            return;

        networkManager.OnServerStarted += OnServerConnected;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        connectingText.text = ConnectionStatusTexts[3];
        isConnecting = true;
        networkManager.StartHost();
    }
    private void OnClientConnected(ulong id)
    {
        Debug.Log("Client connected with id " + id);

        if (id == networkManager.LocalClientId)
        {
            connectingText.text = ConnectionStatusTexts[1];
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback += ClientDisconnection;
            connectionPanel.SetActive(false);
            isConnecting = false;
        }
    }
    private void OnServerConnected()
    {
        Debug.Log("Host connected with address " + Tools.GetLocalIPv4());
    
        connectingText.text = ConnectionStatusTexts[2];
        networkManager.OnServerStarted -= OnServerConnected;
        networkManager.OnClientDisconnectCallback += ClientDisconnection;
        connectionPanel.SetActive(false);
        isConnecting = false;
    }

    private void ClientDisconnection(ulong id)
    {
        if (networkManager.LocalClientId == id)
        {
            networkManager.OnClientDisconnectCallback -= ClientDisconnection;
            connectingText.text = ConnectionStatusTexts[0];
            connectionPanel.SetActive(true);
        }
    }
}
