using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public class NetworkConnection
{
    public Socket socket;

    // Size of receive buffer.  
    private const int BufferSize = 1024;

    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];

    public NetworkConnection(Socket sck)
    {
        this.socket = sck;
    }

    public NetworkConnection() { }

    /// <summary>
    /// Verifies whether the user is connected
    /// On server side this is much more relilable than using socket.Connected
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
        try
        {
            return !(this.socket.Poll(1, SelectMode.SelectRead) && this.socket.Available == 0);
        }
        catch (SocketException) { return false; }
    }

}
