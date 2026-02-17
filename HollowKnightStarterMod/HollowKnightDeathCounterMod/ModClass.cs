// using UObject = UnityEngine.Object;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlobalEnums;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace HollowKnightDeathCounterMod
{
    public class HollowKnightDeathCounterMod() : Mod("HkDeathCounter"), ILocalSettings<LocalSettings>
    {
        public override string GetVersion() => "v0.0.7";

        public LocalSettings LocalSettings { get; set; }
        public WsBroadcaster _broadcaster = null!;

        public override void Initialize()
        {
            const string wsUrl = "http://localhost:5000/ws/";
            _broadcaster = WsBroadcaster.OpenSocket(wsUrl, Log);

            On.HeroController.Die += (On.HeroController.orig_Die org, HeroController self) =>
            {
                Died();
                return org(self);
            };
            Log($"Initialized {Name}");
            Log("Going to start listening");
            _broadcaster.StartListening();
            Log("Now listening");
        }

        private void Died()
        {
#if DEBUG
            Log($"Death counter before incrementing {LocalSettings.DeathCounter}");
#endif

            LocalSettings.DeathCounter += 1;
            Log($"Death counter increased to {LocalSettings.DeathCounter}");
            _broadcaster.SendMessage(LocalSettings.DeathCounter);
        }

        public void OnLoadLocal(LocalSettings s) => LocalSettings = s;
        public LocalSettings OnSaveLocal() => LocalSettings;
    }

    public class LocalSettings
    {
        public int DeathCounter { get; set; } = 0;
    }


    public class WsBroadcaster
    {
        private readonly Action<string> _log;
        private readonly HttpListener _listener;
        private WebSocket? _socket;

        private WsBroadcaster(Action<string> log, HttpListener listener)
        {
            _log = log;
            _listener = listener;
        }

        public static WsBroadcaster OpenSocket(string url, Action<string>? log = null)
        {
            log ??= (str => Console.WriteLine(str));

            HttpListener listener = new();
            listener.Prefixes.Add(url);
            listener.Start();
            log("WebSocket server listening at ws://localhost:5000/ws/");

            return new(log, listener);
        }

        public void StartListening()
        {
            var bufferArr = new byte[1024];
            ArraySegment<byte> buffer = new(bufferArr);
            _ = Task.Run(async () =>
            {
                _log("Runner started");
                while (true)
                {
                    var context = await _listener.GetContextAsync();
                    _log("Received context");

                    try
                    {
                        _log("Attempting to accept WebSocket...");
                        var wsContext = await context.AcceptWebSocketAsync(null);
                        _socket = wsContext.WebSocket;
                        _log("Client connected!");

                        var buffer = new byte[4096];

                        while (_socket.State == WebSocketState.Open)
                        {
                            var result = await _socket.ReceiveAsync(
                                new ArraySegment<byte>(buffer),
                                CancellationToken.None
                            );

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await _socket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closing",
                                    CancellationToken.None
                                );
                                _log("Client disconnected");
                            }
                            else
                            {
                                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                _log($"Received: {message}");
                            }
                        }
                    }
                    catch (WebSocketException wse)
                    {
                        _log($"Not a websocket request or handshake failed: {wse.Message}");
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                    catch (Exception ex)
                    {
                        _log($"Error occured on ws: {ex}");
                    }
                    finally
                    {
                        Disconnect();
                    }
                }
            });
        }

        public async void SendMessage(int deathCounter)
        {
            try
            {
                if (_socket.State != WebSocketState.Open) { return; }

                var reply = Encoding.UTF8.GetBytes($"{deathCounter}");
                await _socket.SendAsync(new ArraySegment<byte>(reply), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _log($"Error occured when sending a message: {ex}");
            }
        }

        private void Disconnect()
        {
            _socket.Dispose();
            _socket = null;
            _log("Websocket disconnected");
        }
    }
}