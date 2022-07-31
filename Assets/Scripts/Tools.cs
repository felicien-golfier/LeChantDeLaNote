using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public readonly static int limitY = 42;
    public readonly static int limitX = 34;
    public readonly static int zCamera = -10;
    public readonly static float AggroSquaredDist = 64;

    private static Tools _instance;
    public static Tools instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void DisplayThisShit(GameObject go)
    {
        go.SetActive(true);
    }
    public void StopDisplayThisShit(GameObject go)
    {
        go.SetActive(false);
    }

    public static float SquaredDist(Vector2 v1, Vector2 v2)
    {
        return (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
    }
    public static IEnumerator RoutineCallFunctionNextFrame(Action Func)
    {
        yield return new WaitForEndOfFrame();
        Func();
    }
    public static IEnumerator RoutineCallFunctionAfterTime<T>(Action<T> Func, T param, float time)
    {
        yield return new WaitForSeconds(time);
        Func(param);
    }
    public static IEnumerator RoutineCallFunctionAfterTime(Action Func, float time)
    {
        yield return new WaitForSeconds(time);
        Func();
    }

    public static IPAddress GetLocalIPAddressBroadcast()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string[] splitMessages = ip.ToString().Split('.');
                string ipAddressString = splitMessages[0] + "." + splitMessages[1] + "." + splitMessages[2] + ".255";
                if (!IPAddress.TryParse(ipAddressString, out IPAddress address))
                    Debug.LogError("Wrong IP address ! " + ipAddressString + " is not in the good format !!");
                return address;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    static public string GetLocalIPv4()
    {
        foreach (var Address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return Address.ToString();
        }

        return "Address not found";
    }
}
