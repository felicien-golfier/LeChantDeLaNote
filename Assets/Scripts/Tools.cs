using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

[Serializable]
public enum Cardinal
{
    North,
    East,
    South,
    West
}
public class Tools : MonoBehaviour
{
    public static float TimeBeforeQuitForReal = .3f;

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

    public static Cardinal GetOppositCardinal(Cardinal cardinal)
    {
        int CardinalInt = (int)cardinal;
        int opositCardinalInt = (CardinalInt + 2) % 4;
        return (Cardinal)opositCardinalInt;
    }
    public static void GetSideCoordinatesToAdd(Cardinal cardinal, out int x, out int y, bool isUpsideDown)
    {

        // METHOD 1 :
        int cardinalInt = (int)cardinal;
        int CardModulo2 = cardinalInt % 2;
        int CardInferior2 = cardinalInt < 2 ? 1 : -1;
        // We calculate the Position according to the cardinal : NORTH = 0 => (0,1); EAST = 1 => (1,0); SOUTH = 2 => (0,-1); WEST = 3 => (-1,0)
        x = CardModulo2 * CardInferior2;
        y = ((CardModulo2 + 1) % 2) * CardInferior2;

        //METHOD 2:
        if (isUpsideDown)
        { 
            if(cardinal == Cardinal.East || cardinal == Cardinal.West)
            {
                x *= -1;
            }
            else
            {
                y *= -1;
            }
        }
    }
    public static void GetSideCoordinates(Cardinal cardinal, ref int x, ref int y)
    {
        GetSideCoordinatesToAdd(cardinal, out int sideXToAdd, out int sideYToAdd, IsUpsideDown(y));
        x += sideXToAdd;
        y += sideYToAdd;
    }

    public static bool IsUpsideDown(int y)
    {
        return y % 2 != 0;
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
