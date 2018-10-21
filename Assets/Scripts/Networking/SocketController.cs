
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UNetworking;
using static UNetworking.SocketClient;
using System.Collections;
using System.Net.Sockets;

namespace Assets.Scripts.Network.Resources
{
    public abstract class SocketController
    {
        private static SocketClient socketClient = new SocketClient(Endpoint.ip_addr, Endpoint.port);


        public static void Initialize()
        {
            Debug.Log($"Connecting too ...{Endpoint.ip_addr}:{Endpoint.port}");

            //Remove soon we don't need them only if we got something internal to take care of
            socketClient.RegisterHandler(NetworkInstructions.Connect, OnConnect);
            socketClient.RegisterHandler(NetworkInstructions.Disconnect, OnDisconnect);

            socketClient.Connect();
        }

        private static void OnDisconnect(NetworkMessage netMsg)
        {
            Debug.Log("Server is offline called at SocketController");
        }


        /// <summary>
        /// This is very important because we will establish the connect control here
        /// </summary>
        /// <param name="netMsg"></param>
        public static void OnConnect(NetworkMessage netMsg)
        {
            Debug.Log($"Im connected to the server IP{Endpoint.ip_addr} and PORT{ Endpoint.port}");
        }

        /// <summary>
        /// Called when some network error occurs
        /// </summary>
        /// <param name="netMsg"></param>
        public static void OnError(NetworkMessage netMsg)
        {
            Debug.Log("Network error on sockets occurred: ");
        }


        public static void BindEvent(short opcode, NetworkMessageDelegate handler)
        {
            socketClient.RegisterHandler(opcode, handler);
        }

        public static void UnbindEvent(short opcode)
        {
            socketClient.UnregisterHandler(opcode);
        }

        /// <summary>
        /// todo: refactor this and make it like Send<T> as in the server
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="data"></param>
        public static void Send(short opcode, object obj)
        {
            if (socketClient != null && socketClient.Client.Connected)
            {
                socketClient.Send(opcode, obj);
                Debug.Log($"Sending data to the remote server, Instruction Code: [{opcode}]");
            }
            else
                Debug.Log("Trying to send a message without client");
        }



        /// <summary>
        /// Returns the instance of the NetworkClient
        /// </summary>
        /// <returns></returns>
        public static SocketClient GetSocketClient()
        {
            return socketClient;
        }

        /// <summary>
        /// Returns whether we have a connection with the remote server
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            return (socketClient != null && socketClient.Client.Connected);
        }
    }
}
