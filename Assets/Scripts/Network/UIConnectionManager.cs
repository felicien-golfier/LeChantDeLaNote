using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class UIConnectionManager : MonoBehaviour
{
    [Header("Connection Interface")]
    public TMPro.TextMeshProUGUI connectingText;
    public GameObject[] ToDisplayWhenConnected;
    public GameObject[] ToDisplayWhenDisconnected;
    public TMPro.TMP_Text ipAddress;

    private bool isConnecting = false;
    private string[] ConnectionStatusTexts = { "Not Connected", "Connected as CLIENT", "Connected as HOST", "Connection..." };
    private NetworkManager networkManager;
    private MyNetworkDiscovery networkDiscovery;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Debug.LogError("NO NETWORK MANAGER !!");
            return;
        }
        connectingText.text = ConnectionStatusTexts[0];
        networkDiscovery = networkManager.GetComponent<MyNetworkDiscovery>();
        OnChangeConnection(false);
        ipAddress.text = Tools.GetLocalIPv4();
    }

    public void StartClientConnection()
    { 
        if (!networkManager || networkManager.IsClient || networkManager.IsServer || isConnecting)
            return;
        OnStartClient();
        networkDiscovery.StartClient();
    }

    private void OnStartClient()
    {
        networkManager.OnClientConnectedCallback += OnClientConnected;
        connectingText.text = ConnectionStatusTexts[3];
        isConnecting = true;
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

    public void DirectIpConnection(TMPro.TMP_InputField field)
    {
        if (!networkManager || networkManager.IsClient || networkManager.IsServer || isConnecting)
            return; 
        OnStartClient();
        networkDiscovery.ConnectAsClient(field.text);
    }

    public void ResetNetwork()
    {
        networkManager.Shutdown();
        OnDisconnection();
    }
    private void OnClientConnected(ulong id)
    {
        Debug.Log("Client connected with id " + id);

        if (id == networkManager.LocalClientId)
        {
            connectingText.text = ConnectionStatusTexts[1];
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback += ClientDisconnection;

            OnChangeConnection(true);
        }
    }

    private void OnChangeConnection(bool connect)
    {
        foreach (var go in ToDisplayWhenConnected)
        {
            go.SetActive(connect);
        }
        foreach (var go in ToDisplayWhenDisconnected)
        {
            go.SetActive(!connect);
        }

        isConnecting = false;
    }

    private void OnServerConnected()
    {
        Debug.Log("Host connected with address " + Tools.GetLocalIPv4());
    
        connectingText.text = ConnectionStatusTexts[2];
        networkManager.OnServerStarted -= OnServerConnected;
        networkManager.OnClientDisconnectCallback += ClientDisconnection;
        OnChangeConnection(true);
    }

    private void ClientDisconnection(ulong id)
    {
        if (networkManager.LocalClientId == id)
        {
            OnDisconnection();
        }
    }

    private void OnDisconnection()
    {
        networkManager.OnClientDisconnectCallback -= ClientDisconnection;
        connectingText.text = ConnectionStatusTexts[0];
        networkDiscovery.StopDiscovery();
        OnChangeConnection(false);
        ScoreManager.instance.OnDisconnection(NetworkManager.Singleton.LocalClientId);
    }

    public void FireButton()
    {
        networkManager.LocalClient.PlayerObject.GetComponent<PlayerControler>().LaunchProjectile();
    }
}
