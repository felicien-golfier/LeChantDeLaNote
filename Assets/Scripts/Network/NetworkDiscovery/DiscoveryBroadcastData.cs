using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct DiscoveryBroadcastData : INetworkSerializable
{
    public string ConnexionMessage;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ConnexionMessage);
    }
}