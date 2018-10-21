using System;
using System.Collections.Generic;
using System.Text;

public class NetworkMessage
{
    public NetworkConnection conn { get; set; }
    private string jdata;

    public NetworkMessage(NetworkConnection _conn, string _jdata)
    {
        this.conn = _conn;
        this.jdata = _jdata;
    }

    /// <summary>
    /// Reads a serialized stream message
    /// </summary>
    /// <typeparam name="TMsg"></typeparam>
    /// <returns></returns>
    public TMsg ReadMessage<TMsg>()
    {
        if (!String.IsNullOrWhiteSpace(this.jdata))
            return JPacketBuilder.Deserialize<TMsg>(this.jdata);
        else
            return default(TMsg);
    }
}