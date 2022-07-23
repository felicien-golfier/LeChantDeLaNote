using System;
using System.Net;
using System.Threading;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkManager))]
public class MyNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable]
    public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData>
    {
    };
    public ServerFoundEvent OnServerFound;//Only kept to have an example

    NetworkManager networkManager;
    Coroutine broadcastCoroutine = null;

    public DiscoveryBroadcastData data;
    public string DefaultServerName = "TestServer";
    public float broadcastInterval = 2f;
    private bool bIsConnecting = false;
    private Mutex mutexStartConnection = new Mutex();
    private string addressToConnectTo;

    public void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.OnServerStarted += StartServer;
    }

    public void Update()
    {
        if (mutexStartConnection.WaitOne(1) && bIsConnecting && addressToConnectTo != string.Empty)
        {
            ConnectAsClient(addressToConnectTo); 
            mutexStartConnection.ReleaseMutex();
        }
    }

    public void ConnectAsClient(string ipAddress)
    {
        Debug.Log("Connecting to address " + ipAddress);
        if (ipAddress != Tools.GetLocalIPv4())
            ((UnityTransport)networkManager.NetworkConfig.NetworkTransport).ConnectionData.Address = ipAddress;
        networkManager.StartClient();
        PlayerPrefs.SetString("IpAddress", ipAddress);

        StopDiscovery();
        Debug.Log("Discovery Stopped");

        addressToConnectTo = string.Empty;
    }
    public override void StartClient()
    {
        base.StartClient();
        if (broadcastCoroutine == null)
        {
            broadcastCoroutine = StartCoroutine(BroadcastCoroutine());
        }
        else Debug.LogError("Start client with an existing coroutine");
        bIsConnecting = false;
    }
    public override void StopDiscovery()
    {
        if (broadcastCoroutine != null)
        {
            StopCoroutine(broadcastCoroutine);
            broadcastCoroutine = null;
        }
        base.StopDiscovery();
    }
    private IEnumerator BroadcastCoroutine()
    {
        Debug.Log("Start Broadcasting");

        while (true)
        {
            yield return new WaitForSeconds(broadcastInterval);
            if(IsClient)
                ClientBroadcast(data);
        }
    }
    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData()
        {
            ServerName = DefaultServerName,
            Port = ((UnityTransport)networkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        if (bIsConnecting)
            return;
        if (mutexStartConnection.WaitOne(10))
        {
            addressToConnectTo = sender.Address.ToString();
            bIsConnecting = true;
            mutexStartConnection.ReleaseMutex();
        }
        else
        {
            Debug.LogError("Mutex not available");
        }
    }
}