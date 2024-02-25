using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Sage.Core.Ipc
{
    public sealed class IpcServerDaemon : IDisposable
    {
        private readonly ILogger _log;
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cancel;

        public string Name { get; }

        public int MaxThreads { get; }

        public event EventHandler<IpcCommandEventArgs> Command;

        public IpcServerDaemon(string name, int maxThreads)
        {
            Name = name;
            MaxThreads = maxThreads;
            _log = Log.ForContext<IpcServerDaemon>();
            _cancel = new CancellationTokenSource();
            _thread = new Thread(o => Daemon((CancellationToken)o));
        }

        public void Start()
        {
            _thread.Start(_cancel.Token);
        }

        private void Daemon(CancellationToken cancel)
        {
            _log.Information("IPC server daemon started");

            var instances = new List<IpcServerInstance>();
            var instanceCancel = new CancellationTokenSource();

            void AddNewInstanceIfRequired()
            {
                lock (instances)
                {
                    if (cancel.IsCancellationRequested || instanceCancel.IsCancellationRequested 
                        || instances.Count >= MaxThreads
                        || (instances.Count > 0 && instances.Any(i => !i.IsConnected))) return;
                    _log.Information("Creating new server instance");
                    var newInstance = new IpcServerInstance(Name, instanceCancel.Token);
                    newInstance.Connected += (s, _) => AddNewInstanceIfRequired();
                    newInstance.Command += OnCommand;
                    instances.Add(newInstance);
                }
            }

            Task[] GetTasks()
            {
                lock (instances) { return instances.Select(i => i.Task).ToArray(); }
            }

            void CancelAndWaitAll()
            {
                instanceCancel.Cancel();
                var t = GetTasks();
                _log.Information("Waiting for all instances to exit...");
                if (t.Length > 0) Task.WaitAll(t);
            }

            void OnCommand(object sender, IpcCommandEventArgs args)
            {
                Command?.Invoke(this, args);
            }

            while (true)
            {
                try
                {
                    AddNewInstanceIfRequired();
                    var tasks = GetTasks();
                    Task.WaitAny(tasks, cancel);
                    lock (instances) { instances.RemoveAll(t => t.Task.IsCompleted); }
                }
                catch (OperationCanceledException)
                {
                    _log.Information("Server daemon was cancelled, instances will now be cancelled");
                    CancelAndWaitAll();
                    break;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Server daemon stopped due to unhandled exception, instances will now be cancelled");
                    CancelAndWaitAll();
                    break;
                }
            }

            instanceCancel.Dispose();
        }
        
        internal class IpcServerInstance : IEquatable<IpcServerInstance>
        {
            private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);

            private readonly ILogger _log;
            private readonly string _name;

            public Task Task { get; }

            public bool IsConnected { get; private set; }

            public event EventHandler<EventArgs> Connected;

            public event EventHandler<IpcCommandEventArgs> Command;

            public IpcServerInstance(string name, CancellationToken cancel)
            {
                _log = Log.ForContext<IpcServerInstance>();
                _name = name;
                Task = Task.Run(() => Instance(cancel), cancel);
            }

            private async Task Instance(CancellationToken cancel)
            {
                const string magic = "Sage";

                var tIdStr = $"IPC server instance (thread ID {Environment.CurrentManagedThreadId})";
                NamedPipeServerStream namedPipeServer = null;
                try
                {
                    _log.Information($"{tIdStr} started");
                    namedPipeServer = new NamedPipeServerStream(_name, PipeDirection.InOut,
                                                                NamedPipeServerStream.MaxAllowedServerInstances,
                                                                PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    await namedPipeServer.WaitForConnectionAsync(cancel).ConfigureAwait(false);
                    _log.Information($"{tIdStr} recieved a client connection");
                    OnConnected();

                    while (namedPipeServer.IsConnected)
                    {
                        var responsePacket = new IpcResponsesPacket { Responses = new List<IpcResponse>() };
                        var header = new byte[8];
                        var i = await namedPipeServer.ReadAsync(header, cancel).ConfigureAwait(false);
                        if (i == 8 && Encoding.ASCII.GetString(header, 0, 4) == magic)
                        {
                            var length = BinaryPrimitives.ReadInt32LittleEndian(header.AsSpan(4));
                            var packetBytes = new byte[length];
                            i = await namedPipeServer.ReadAsync(packetBytes, cancel).ConfigureAwait(false);
                            if (i == length)
                            {
                                IpcCommandsPacket packet = null;
                                try
                                {
                                    packet = JsonSerializer.Deserialize<IpcCommandsPacket>(packetBytes, _jsonOptions);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex, $"{tIdStr}: Failed to deserialize packet");
                                }

                                if (packet != null)
                                {
                                    foreach (var command in packet.Commands)
                                    {
                                        if (cancel.IsCancellationRequested)
                                        {
                                            namedPipeServer.Disconnect();
                                            break;
                                        }
                                        switch (command)
                                        {
                                            case IpcCloseCommand _:
                                                namedPipeServer.Disconnect();
                                                break;
                                            case IpcNopCommand n:
                                                Thread.Sleep(n.Delay);
                                                responsePacket.Responses.Add(new IpcResponse());
                                                break;
                                            case IpcOpenFileCommand _: 
                                            case IpcPlaybackControlCommand _:
                                                var args = OnCommand(command, false);
                                                var resp = args.Handled switch
                                                {
                                                    true => ResponseCode.Success,
                                                    false => ResponseCode.HandlingFailed,
                                                    null => ResponseCode.NotHandled
                                                };
                                                responsePacket.Responses.Add(new IpcResponse(resp, args.Result));
                                                break;
                                            case IpcPlaybackQueryCommand _: 
                                            case IpcMetadataQueryCommand _:
                                                var argsQ = OnCommand(command, true);
                                                var respQ = argsQ.Handled switch
                                                {
                                                    true => ResponseCode.Success,
                                                    false => ResponseCode.HandlingFailed,
                                                    null => ResponseCode.NotHandled
                                                };
                                                responsePacket.Responses.Add(new IpcResponse(respQ, argsQ.Result));
                                                break;
                                            default:
                                                _log.Error($"{tIdStr}: Command deserialization resulted in invalid or null command");
                                                responsePacket.Responses.Add(new IpcResponse(ResponseCode.InvalidCommand));
                                                break;
                                        }
                                    }
                                }
                            }
                            else _log.Error($"{tIdStr}: Invalid packet length or read failed");
                        }
                        else _log.Error($"{tIdStr}: Invalid packet header magic or read failed");

                        // Deal with cancellation
                        if (cancel.IsCancellationRequested) break;

                        // Required for close command to work
                        if (!namedPipeServer.IsConnected) break;

                        // Add invalid packet response
                        if (responsePacket.Responses.Count < 1)
                            responsePacket.Responses.Add(new IpcResponse(ResponseCode.InvalidPacket));

                        // Send response packet
                        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(responsePacket);
                        await namedPipeServer.WriteAsync(Encoding.ASCII.GetBytes(magic), cancel);
                        var lengthBytes = new byte[4];
                        BinaryPrimitives.WriteInt32LittleEndian(lengthBytes, jsonBytes.Length);
                        await namedPipeServer.WriteAsync(lengthBytes, cancel);
                        await namedPipeServer.WriteAsync(jsonBytes, cancel);
                    }

                }
                catch (OperationCanceledException)
                {
                    _log.Information($"{tIdStr} was cancelled");
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"{tIdStr} stopped due to unhandled exception");
                }
                finally
                {
                    if (namedPipeServer != null)
                    {
                        if (namedPipeServer.IsConnected)
                            namedPipeServer.Disconnect();
                        namedPipeServer.Dispose();
                    }
                }
            }

            private void OnConnected()
            {
                IsConnected = true;
                Connected?.Invoke(this, EventArgs.Empty);
            }

            private IpcCommandEventArgs OnCommand(IpcCommand cmd, bool resultRequired)
            {
                var args = new IpcCommandEventArgs(cmd, resultRequired);
                Command?.Invoke(this, args);
                return args;
            }

            public override bool Equals(object obj)
            {
                return obj is IpcServerInstance s && Equals(s);
            }

            public bool Equals(IpcServerInstance other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Task, other.Task);
            }

            public override int GetHashCode()
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                return Task?.GetHashCode() ?? 0;
            }
        }

        public void Dispose()
        {
            if (!_thread.IsAlive) return;
            _cancel.Cancel();
            _thread.Join();
            _cancel.Dispose();
        }
    }

    public class IpcCommandEventArgs : EventArgs
    {
        public IpcCommandEventArgs(IpcCommand command, bool resultRequired)
        {
            Command = command;
            ResultRequired = resultRequired;
        }

        public IpcCommand Command { get; }
        public bool ResultRequired { get; }
        public bool? Handled { get; private set; }
        public string Result { get; private set; }

        public void SetHandled(bool success = true, string result = null)
        {
            Handled = success;
            Result = result;
        }
    }
}
