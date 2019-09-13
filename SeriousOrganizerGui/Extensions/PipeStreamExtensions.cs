using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Extensions
{
    public static class PipeStreamExtensions
    {
        /// <summary>
        /// Reads a message from the pipe.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadMessage(this PipeStream stream)
        {
            stream.Read(bufferSize, 0, 4);
            var size = BitConverter.ToInt32(bufferSize, 0);

            memoryStream.Position = 0;
            memoryStream.Write(buffer, 0, stream.Read(buffer, 0, size));

            return memoryStream.ToArray();
        }

        private static MemoryStream memoryStream = new MemoryStream();

        private static byte[] buffer = new byte[500 * 1024];
        private static byte[] bufferSize = new byte[4];

        private static List<byte> _bytesBuffer = new List<byte>();

        public static void SendRequest<T>(this PipeStream stream, byte[] req, T obj)
        {
            _bytesBuffer.Clear();
            _bytesBuffer.AddRange(req);
            _bytesBuffer.AddRange(MessagePackSerializer.Serialize(obj));
            stream.Write(BitConverter.GetBytes(_bytesBuffer.Count), 0, 4);
            stream.Write(_bytesBuffer.ToArray(), 0, _bytesBuffer.Count);
            stream.Flush();
        }

        public static void SendRequest(this PipeStream stream, byte[] req)
        {
            stream.Write(BitConverter.GetBytes(req.Length), 0, 4);
            stream.Write(req, 0, req.Length);
            stream.Flush();
        }

        public static T WaitResponse<T>(this PipeStream stream)
        {
            var bin = stream.ReadMessage();

            return MessagePackSerializer.Deserialize<T>(bin);
        }

        public static object WaitResponse(this PipeStream stream)
        {
            var bin = stream.ReadMessage();

            return MessagePackSerializer.Typeless.Deserialize(bin);
        }

        public static UInt32 WaitResponseU32(this PipeStream stream)
        {
            var bin = stream.ReadMessage();
            return BitConverter.ToUInt32(bin, 0);
        }
    }
}
