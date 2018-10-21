using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UNetworking
{

    /// <summary>
    /// Socket Client class who talks with the headless server outside Unity
    /// </summary>
    public class SocketClient
    {
        /// <summary>
        /// Target server port
        /// </summary>
        private Int32 _port;

        /// <summary>
        /// Targer server IPAddress
        /// </summary>
        private IPAddress _addr;

        /// <summary>
        /// Singleton for socket client in NetworkConnection as an alias
        /// </summary>
        public Socket Client { get { return this.Connection.socket; } }

        public NetworkConnection Connection { get; } = null;

        /// <summary>
        /// Delegate abstraction for communication over callbacks
        /// </summary>
        /// <param name="netMsg"></param>
        public delegate void NetworkMessageDelegate(NetworkMessage netMsg);

        /// <summary>
        /// Registered Handlers map 
        /// </summary>
        private List<Dictionary<short, NetworkMessageDelegate>> handlersMap = new List<Dictionary<short, NetworkMessageDelegate>>();

        /// <summary>
        /// Initializes a new socket and networkconnection object with the given
        /// adress(as string) and port
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        public SocketClient(string addr, Int32 port)
        {
            this._addr = IPAddress.Parse(addr);
            this._port = port;
            this.Connection = new NetworkConnection();
            this.Connection.socket = new Socket(this._addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Initializes a new socket and networkconnection object with the given
        /// adress(as IPAddress type) and port
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        public SocketClient(IPAddress addr, Int32 port)
        {
            this._addr = addr;
            this._port = port;
            this.Connection = new NetworkConnection();
            this.Connection.socket = new Socket(this._addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Starts the client to connect with the remote server
        /// </summary>
        public void Connect()
        {
            try
            {
                var remoteEP = new IPEndPoint(this._addr, this._port);

                // Connect to the remote endpoint.  
                var result = this.Connection.socket.BeginConnect(remoteEP,
                    new AsyncCallback(OnConnection), this.Connection);

                bool success = result.AsyncWaitHandle.WaitOne(20000, true);
                if (!success)
                {
                    NotifyServerOffline(new NetworkMessage(null, null));
                }

               // StartHeartbeat();

            }
            catch (SocketException se)
            {
                Debug.Log(se.ToString());
            }
        }

        /// <summary>
        /// Disconnects from the remote server and disposes all the resources used
        /// </summary>
        public void Disconnect()
        {
            if (!this.Connection.socket.Connected) return;
            try
            {
                this.Connection.socket.Disconnect(false);
                this.Connection.socket.Dispose();
            }
            catch (SocketException se)
            {
                Debug.Log(se.ToString());
            }
        }

        /// <summary>
        /// Registers a new handler callback for event based system
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(short opcode, NetworkMessageDelegate handler)
        {
            var op = new Dictionary<short, NetworkMessageDelegate>();
            op.Add(opcode, handler);
            this.handlersMap.Add(op);
        }

        /// <summary>
        /// Deletes every delegation for this opcode, assuming you have knowledge of this
        /// proceed carefully (This function is mentioned to be likely a Reset for the opcode)
        /// </summary>
        /// <param name="opcode"></param>
        public void UnregisterHandler(short opcode)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(opcode))
                    item.Remove(opcode);
            });
        }

        /// <summary>
        /// Internal callback to handle new connection
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnection(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var handler = (NetworkConnection)ar.AsyncState;

                //Is that client disconnected? server is offline
                if (!handler.socket.Connected)
                {
                    NotifyServerOffline(new NetworkMessage(this.Connection, null));
                    return;
                }

                // Complete the connection.  
                handler.socket.EndConnect(ar);

                //Notify the listeners
                var netMsg = new NetworkMessage(this.Connection, null);
                NotifyNewConnection(netMsg);

                handler.socket.BeginReceive(handler.buffer, 0, handler.buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceive), handler);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Internal callback to handle data receive
        /// </summary>
        /// <param name="ar"></param>
        private void OnDataReceive(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                var handler = (NetworkConnection)ar.AsyncState;
                var client = handler.socket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    var encodedBytes = Encoding.ASCII.GetString(handler.buffer, 0, bytesRead);
                    Debug.Log("Data received : " + encodedBytes);

                    var packet = JPacketBuilder.Deserialize<Packet>(encodedBytes);
                    var netMsg = new NetworkMessage(handler, packet.data);

                    //Notify the callbacks waiting for this opcode
                    NotifyDataReceived(packet.opcode, netMsg);

                    // Get the rest of the data.  
                    client.BeginReceive(handler.buffer, 0, handler.buffer.Length, 0,
                        new AsyncCallback(OnDataReceive), handler);
                }
            }
            catch (Exception e)
            {
                NotifyDisconnection(new NetworkMessage(null, null));
            }
        }

        /// <summary>
        /// Sends an object with an opcode identifier to the remote server
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="data"></param>
        public void Send(short opcode, object data)
        {
            if (!this.Connection.IsConnected()) return;

            //Create a internal data structure
            var packet = new Packet
            {
                opcode = opcode,
                data = JPacketBuilder.Serialize(data)
            };

            //Serialize all the packet data
            var payload = JPacketBuilder.Serialize(packet);

            Debug.Log("Payload = " + payload);
            //Now convert to a buffer byte 
            byte[] byteData = Encoding.ASCII.GetBytes(payload);

            Debug.Log($"Sending {byteData.Length} bytes to the server");

            // Begin sending the data to the remote device.  
            this.Client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), this.Client);
        }

        /// <summary>
        /// Internal callback when the send method finishes
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Debug.Log($"Sent {bytesSent} bytes to server.");

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Notify all actions subscribed for this opcode
        /// </summary>
        /// <param name="netMsg"></param>
        private void NotifyDisconnection(NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(NetworkInstructions.Disconnect))
                    item[NetworkInstructions.Disconnect].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Notify all actions subscribed about this opcode and give them the data
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="netMsg"></param>
        private void NotifyDataReceived(short opcode, NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(opcode))
                    item[opcode].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Notify all actions subscribed for connect opcode
        /// </summary>
        /// <param name="netMsg"></param>
        private void NotifyNewConnection(NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(NetworkInstructions.Connect))
                    item[NetworkInstructions.Connect].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Notify all actions subscribed for server offline event
        /// </summary>
        /// <param name="netMsg"></param>
        private void NotifyServerOffline(NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(NetworkInstructions.ServerOffline))
                    item[NetworkInstructions.ServerOffline].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Heartbeat detection for disconnections
        /// </summary>
        private void StartHeartbeat()
        {
            Console.WriteLine("Starting heartbeat thread");
            var heartbeatThread = new Thread(delegate ()
            {
                var serverIsOnline = true;
                var nextTime = DateTime.Now.AddSeconds(2);
                while (serverIsOnline)
                {
                    if (nextTime < DateTime.Now)
                    {
                        try
                        {
                            if (this.Client.Poll(1, SelectMode.SelectRead) && this.Client.Available == 0)
                            {
                                NotifyServerOffline(new NetworkMessage(this.Connection, null));
                                serverIsOnline = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            nextTime = DateTime.Now.AddSeconds(2);
                        }
                    }
                }
            });

            //Starts
            heartbeatThread.Start();
        }

    }

}