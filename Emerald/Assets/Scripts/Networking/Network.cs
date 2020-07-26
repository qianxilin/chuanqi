using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;
using C = ClientPackets;
using S = ServerPackets;
using UnityEngine;


namespace EmeraldNetwork
{
    static class Network
    {
        private static TcpClient _client;
        public static int ConnectAttempt = 0;
        public static bool Connected = false;
        public static bool LoginConnected = false;
        public static DateTime TimeOutTime, TimeConnected;

        public static GameObject LoginManager;

        private static ConcurrentQueue<Packet> _receiveList;
        private static ConcurrentQueue<Packet> _sendList;

        static byte[] _rawData = new byte[0];


        public static void Connect()
        {
            if (_client != null)
                Disconnect();

            ConnectAttempt++;

            _client = new TcpClient {NoDelay = true};
            _client.BeginConnect(GameManager.networkInfo.IPAddress, GameManager.networkInfo.Port, Connection, null);
        }

        private static void Connection(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);

                if (!_client.Connected)
                {
                    Connect();
                    return;
                }
                _receiveList = new ConcurrentQueue<Packet>();
                _sendList = new ConcurrentQueue<Packet>();
                _rawData = new byte[0];

                TimeOutTime = DateTime.Now + TimeSpan.FromSeconds(5);
                TimeConnected = DateTime.Now;


                BeginReceive();
            }
            catch (SocketException)
            {
                Connect();
            }
            catch (Exception ex)
            {
                //if (Settings.LogErrors) CMain.SaveError(ex.ToString());
                Disconnect();
            }
        }

        private static void BeginReceive()
        {
            if (_client == null || !_client.Connected) return;

            byte[] rawBytes = new byte[8 * 1024];

            try
            {
                _client.Client.BeginReceive(rawBytes, 0, rawBytes.Length, SocketFlags.None, ReceiveData, rawBytes);
            }
            catch
            {
                Disconnect();
            }
        }
        private static void ReceiveData(IAsyncResult result)
        {
            if (_client == null || !_client.Connected) return;

            int dataRead;

            try
            {
                dataRead = _client.Client.EndReceive(result);
            }
            catch
            {
                Disconnect();
                return;
            }

            if (dataRead == 0)
            {
                Disconnect();
            }

            byte[] rawBytes = result.AsyncState as byte[];

            byte[] temp = _rawData;
            _rawData = new byte[dataRead + temp.Length];
            Buffer.BlockCopy(temp, 0, _rawData, 0, temp.Length);
            Buffer.BlockCopy(rawBytes, 0, _rawData, temp.Length, dataRead);

            Packet p;
            while ((p = Packet.ReceivePacket(_rawData, out _rawData)) != null)
                _receiveList.Enqueue(p);

            BeginReceive();
        }

        private static void BeginSend(List<byte> data)
        {
            if (_client == null || !_client.Connected || data.Count == 0) return;
            
            try
            {
                _client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendData, null);
            }
            catch
            {
                Disconnect();
            }
        }
        private static void SendData(IAsyncResult result)
        {
            try
            {
                _client.Client.EndSend(result);
            }
            catch
            { }
        }


        public static void Disconnect()
        {
            if (_client == null) return;

            _client.Close();

            TimeConnected = DateTime.MinValue;
            Connected = false;
            _sendList = null;
            _client = null;

            _receiveList = null;
        }

        public static void Process()
        {
            if (_client == null || !_client.Connected)
            {
                if (Connected)
                {
                    while (_receiveList != null && !_receiveList.IsEmpty)
                    {
                        Packet p;

                        if (!_receiveList.TryDequeue(out p) || p == null) continue;
                        if (!(p is ServerPackets.Disconnect) && !(p is ServerPackets.ClientVersion)) continue;

                        ProcessPacket(p);
                        _receiveList = null;
                        return;
                    }

                    //MirMessageBox.Show("Lost connection with the server.", true);
                    Disconnect();
                    return;
                }
                return;
            }

            if (!Connected && TimeConnected > DateTime.MinValue && DateTime.Now > TimeConnected + TimeSpan.FromSeconds(5))
            {
                Disconnect();
                Connect();
                return;
            }

            while (_receiveList != null && !_receiveList.IsEmpty)
            {
                Packet p;
                if (!_receiveList.TryDequeue(out p) || p == null) continue;
                ProcessPacket(p);
            }


            if (DateTime.Now > TimeOutTime && _sendList != null && _sendList.IsEmpty)
                _sendList.Enqueue(new C.KeepAlive());

            if (_sendList == null || _sendList.IsEmpty) return;

            TimeOutTime = DateTime.Now + TimeSpan.FromSeconds(5);

            List<byte> data = new List<byte>();
            while (!_sendList.IsEmpty)
            {
                Packet p;
                if (!_sendList.TryDequeue(out p)) continue;
                data.AddRange(p.GetPacketBytes());
            }

            BeginSend(data);
        }

        public static void ProcessPacket(Packet p)
        {
            switch (GameManager.gameStage)
            {
                case GameStage.Login:
                    ProcessLoginPacket(p);
                    break;
            }            
        }

        public static void ProcessLoginPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.Connected:
                    Connected = true;
                    SendVersion();
                    break;
                case (short)ServerPacketIds.ClientVersion:
                    ClientVersion((S.ClientVersion)p);
                    break;
                case (short)ServerPacketIds.NewAccount:
                    NewAccount((S.NewAccount)p);
                    break;
                default:
                    //base.ProcessPacket(p);
                    break;
            }
        }

        public static void SendVersion()
        {
            C.ClientVersion p = new C.ClientVersion();
            try
            {
                byte[] sum = new byte[0];
                //using (MD5 md5 = MD5.Create())
                //using (FileStream stream = File.OpenRead(Application.dataPath))
                //    sum = md5.ComputeHash(stream);
                p.VersionHash = sum;
                Enqueue(p);
            }
            catch (Exception ex)
            {
                //if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }
        }

        public static void ClientVersion(S.ClientVersion p)
        {
            LoginConnected = true;
        }

        public static void NewAccount(S.NewAccount p)
        {
            if (LoginManager == null) return;

            switch (p.Result)
            {
                case 0:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Account creation is disabled.");
                    break;
                case 1:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid account ID.");
                    break;
                case 2:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid password.");
                    break;
                case 3:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid email address.");
                    break;
                case 4:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid username.");
                    break;
                case 5:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid secret question.");
                    break;
                case 6:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Invalid secret answer.");
                    break;
                case 7:
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Account ID already exists.");
                    break;
                case 8:
                    LoginManager.GetComponent<LoginManager>().RegisterCancel_OnClick();
                    LoginManager.GetComponent<LoginManager>().ShowMessageBox("Account creation successful.");                    
                    break;
            }
        }

        public static void Enqueue(Packet p)
        {
            if (_sendList != null && p != null)
                _sendList.Enqueue(p);
        }
    }
}
