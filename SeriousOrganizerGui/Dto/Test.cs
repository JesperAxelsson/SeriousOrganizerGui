using MessagePack;
using SeriousOrganizerGui.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Dto
{

    public enum SortColumn : UInt32
    {
        Name = 0,
        Path = 1,
        Date = 2,
        Size = 3,
    }

    public enum  SortOrder : UInt32
    {
        Asc = 0,
        Desc = 1,
    }


    public static class RequestType
    {
        private const byte DirRequest = 1;
        private const byte FileRequest = 2;
        private const byte AddPath = 3;
        private const byte RemovePath = 4;
        private const byte ReloadStore = 5;
        private const byte ChangeSearchText = 6;
        private const byte DirCount = 7;
        private const byte DirFileCount = 8;
        private const byte DeletePath = 9;
        private const byte Sort = 10;
        private const byte LabelAdd = 11;
        private const byte LabelRemove = 12;
        private const byte LabelsGet = 13;

        public static byte[] CreateReloadRequest()
        {
            return new byte[] { ReloadStore };
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

        public static byte[] CreateDirFileCountRequest(int ix)
        {
            var req = new List<byte>();
            req.Add(DirFileCount);
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateFileRequest(int dirIx, int fileIx)
        {
            var req = new List<byte>();
            req.Add(FileRequest);
            req.AddRange(BitConverter.GetBytes((UInt32)dirIx));
            req.AddRange(BitConverter.GetBytes((UInt32)fileIx));
            return req.ToArray();
        }

        public static byte[] CreateSortRequest(SortColumn column, SortOrder order)
        {
            var req = new List<byte>();
            req.Add(Sort);
            req.AddRange(BitConverter.GetBytes((UInt32)column));
            req.AddRange(BitConverter.GetBytes((UInt32)order));
            return req.ToArray();
        }

        public static byte[] CreateLabelAddRequest()
        {
            return new byte[] { LabelAdd };
        }

        public static byte[] CreateLabelRemoveRequest(int id)
        {
            var req = new List<byte>();
            req.Add(LabelRemove);
            req.AddRange(BitConverter.GetBytes((UInt32)id));
            return req.ToArray();
        }

        public static byte[] CreateLabelsGetRequest()
        {
            return new byte[] { LabelsGet };
        }
    }

    [MessagePackObject, ToString]
    public class TextSearch
    {
        [Key(0)]
        public string Text { get; set; }
    }

    [MessagePackObject, ToString]
    public class DirEntry : Indexed
    {
        [Key(0)]
        public Int32 Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Path { get; set; }
        [Key(3)]
        public UInt64 Size { get; set; }

        [IgnoreMember]
        public int Index { get; set; }
    }

    [MessagePackObject, ToString]
    public class FileEntry : Indexed
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public string Path { get; set; }
        [Key(2)]
        public UInt64 Size { get; set; }

        [IgnoreMember]
        public int Index { get; set; }
    }

    [MessagePackObject, ToString]
    public class DirCountResponse
    {
        [Key(0)]
        public UInt32 Count { get; set; }
    }

    [MessagePackObject, ToString]
    public class Label
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name{ get; set; }
    }
}
