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

namespace SeriousOrganizerGui
{
    public static class Extensions
    {
        //public static byte[] ToBson<T>(this T value)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    using (BsonDataWriter datawriter = new BsonDataWriter(ms))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        serializer.Serialize(datawriter, value);
        //        return ms.ToArray();
        //    }
        //}

        //public static T FromBson<T>(this byte[] data)
        //{
        //    using (MemoryStream ms = new MemoryStream(data))
        //    using (BsonDataReader reader = new BsonDataReader(ms))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        return serializer.Deserialize<T>(reader);
        //    }
        //}


        /// <summary>
        /// Reads a message from the pipe.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadMessage(this PipeStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] buffer = new byte[stream.InBufferSize];

            do
            {
                memoryStream.Write(buffer, 0, stream.Read(buffer, 0, buffer.Length - 1));

            } while (stream.IsMessageComplete == false);

            return memoryStream.ToArray();
        }

        public static void SendRequest<T>(this PipeStream stream, byte[] req, T obj)
        {
            var bytes = new List<byte>();
            bytes.AddRange(req);
            bytes.AddRange(MessagePackSerializer.Serialize(obj));
            stream.Write(bytes.ToArray(), 0, bytes.Count);
            stream.Flush();
        }

        public static void SendRequest(this PipeStream stream, byte[] req)
        {
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
