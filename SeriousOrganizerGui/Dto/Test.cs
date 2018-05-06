using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Dto
{

    public static class RequestType
    {
        private const byte Test = 0;
        private const byte DirRequest = 1;
        private const byte FileRequest = 2;
        private const byte AddPath = 3;
        private const byte RemovePath = 4;
        private const byte ReloadStore = 5;
        private const byte ChangeSearchText = 6;
        private const byte DirCount = 7;

        public static byte[] CreateTestRequest()
        {
            var req = new List<byte>();
            req.Add(Test);
            return req.ToArray();
        }

        public static byte[] CreateChangeSearchText()
        {
            return new byte[] { ChangeSearchText };
        }

        public static byte[] CreateDirCountRequest()
        {
            var req = new List<byte>();
            req.Add(DirCount);
            return req.ToArray();
        }


        public static byte[] CreateDirRequest(int ix)
        {
            var req = new List<byte>();
            req.Add(DirRequest);
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateFileRequest(int ix)
        {
            var req = new List<byte>();
            req.Add(FileRequest);
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }
    }

    [MessagePackObject, ToString]
    public class Test2
    {
        [Key(0)]
        public string Id { get; set; }
        [Key(1)]
        public Int32 Thing { get; set; }
    }

    [MessagePackObject, ToString]
    public class TextSearch
    {
        [Key(0)]
        public string Text { get; set; }
    }

    [MessagePackObject, ToString]
    public class DirEntry
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public string Path { get; set; }
        [Key(2)]
        public UInt64 Size { get; set; }
    }

    [MessagePackObject, ToString]
    public class DirCountResponse
    {
        [Key(0)]
        public UInt32 Count { get; set; }
    }
}
