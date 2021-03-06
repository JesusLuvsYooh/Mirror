using System;
using UnityEngine;

namespace Mirror
{
    // a server's connection TO a LocalClient.
    // sending messages on this connection causes the client's handler function to be invoked directly
    class ULocalConnectionToClient : NetworkConnectionToClient
    {
        internal ULocalConnectionToServer connectionToServer;

        public ULocalConnectionToClient() : base(LocalConnectionId, false, 0) {}

        public override string address => "localhost";

        internal override void Send(ArraySegment<byte> segment, int channelId = Channels.DefaultReliable)
        {
            connectionToServer.buffer.Write(segment);
        }

        // true because local connections never timeout
        /// <inheritdoc/>
        internal override bool IsAlive(float timeout) => true;

        internal void DisconnectInternal()
        {
            // set not ready and handle clientscene disconnect in any case
            // (might be client or host mode here)
            isReady = false;
            RemoveObservers();
        }

        /// <summary>
        /// Disconnects this connection.
        /// </summary>
        public override void Disconnect()
        {
            DisconnectInternal();
            connectionToServer.DisconnectInternal();
        }
    }

    internal class LocalConnectionBuffer
    {
        readonly NetworkWriter writer = new NetworkWriter();
        readonly NetworkReader reader = new NetworkReader(default(ArraySegment<byte>));
        // The buffer is at least 1500 bytes long. So need to keep track of
        // packet count to know how many ArraySegments are in the buffer
        int packetCount;

        public void Write(ArraySegment<byte> segment)
        {
            writer.WriteBytesAndSizeSegment(segment);
            packetCount++;

            // update buffer in case writer's length has changed
            reader.buffer = writer.ToArraySegment();
        }

        public bool HasPackets()
        {
            return packetCount > 0;
        }
        public ArraySegment<byte> GetNextPacket()
        {
            ArraySegment<byte> packet = reader.ReadBytesAndSizeSegment();
            packetCount--;

            return packet;
        }

        public void ResetBuffer()
        {
            writer.Reset();
            reader.Position = 0;
        }
    }

    // a localClient's connection TO a server.
    // send messages on this connection causes the server's handler function to be invoked directly.
    internal class ULocalConnectionToServer : NetworkConnectionToServer
    {
        internal ULocalConnectionToClient connectionToClient;
        internal readonly LocalConnectionBuffer buffer = new LocalConnectionBuffer();

        public override string address => "localhost";

        // see caller for comments on why we need this
        bool connectedEventPending;
        bool disconnectedEventPending;
        internal void QueueConnectedEvent() => connectedEventPending = true;
        internal void QueueDisconnectedEvent() => disconnectedEventPending = true;

        internal override void Send(ArraySegment<byte> segment, int channelId = Channels.DefaultReliable)
        {
            if (segment.Count == 0)
            {
                Debug.LogError("LocalConnection.SendBytes cannot send zero bytes");
                return;
            }

            // handle the server's message directly
            connectionToClient.TransportReceive(segment, channelId);
        }

        internal void Update()
        {
            // should we still process a connected event?
            if (connectedEventPending)
            {
                connectedEventPending = false;
                NetworkClient.OnConnectedEvent?.Invoke(this);
            }

            // process internal messages so they are applied at the correct time
            while (buffer.HasPackets())
            {
                ArraySegment<byte> packet = buffer.GetNextPacket();

                // Treat host player messages exactly like connected client
                // to avoid deceptive / misleading behavior differences
                TransportReceive(packet, Channels.DefaultReliable);
            }

            buffer.ResetBuffer();

            // should we still process a disconnected event?
            if (disconnectedEventPending)
            {
                disconnectedEventPending = false;
                NetworkClient.OnDisconnectedEvent?.Invoke(this);
            }
        }

        /// <summary>
        /// Disconnects this connection.
        /// </summary>
        internal void DisconnectInternal()
        {
            // set not ready and handle clientscene disconnect in any case
            // (might be client or host mode here)
            isReady = false;
            ClientScene.HandleClientDisconnect(this);
        }

        /// <summary>
        /// Disconnects this connection.
        /// </summary>
        public override void Disconnect()
        {
            connectionToClient.DisconnectInternal();
            DisconnectInternal();
        }

        // true because local connections never timeout
        /// <inheritdoc/>
        internal override bool IsAlive(float timeout) => true;
    }
}
