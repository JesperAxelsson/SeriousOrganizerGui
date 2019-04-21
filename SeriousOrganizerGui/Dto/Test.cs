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

    public enum SortOrder : UInt32
    {
        Asc = 0,
        Desc = 1,
    }


    public static class RequestType
    {
        private static byte[] DirRequest = BitConverter.GetBytes((UInt16) 1 );
        private static byte[] FileRequest = BitConverter.GetBytes((UInt16)2);
        private static byte[] AddPath = BitConverter.GetBytes((UInt16)3);
        private static byte[] RemovePath = BitConverter.GetBytes((UInt16)4);
        private static byte[] ReloadStore = BitConverter.GetBytes((UInt16)5);
        private static byte[] ChangeSearchText = BitConverter.GetBytes((UInt16)6);
        private static byte[] DirCount = BitConverter.GetBytes((UInt16)7);
        private static byte[] DirFileCount = BitConverter.GetBytes((UInt16)8);
        private static byte[] DeletePath = BitConverter.GetBytes((UInt16)9);
        private static byte[] Sort = BitConverter.GetBytes((UInt16)10);
        private static byte[] LabelAdd = BitConverter.GetBytes((UInt16)11);
        private static byte[] LabelRemove = BitConverter.GetBytes((UInt16)12);
        private static byte[] LabelsGet = BitConverter.GetBytes((UInt16)13);
        private static byte[] LabelsGetForEntry = BitConverter.GetBytes((UInt16)14);
        private static byte[] AddLabelsToDir = BitConverter.GetBytes((UInt16)15);
        private static byte[] FilterLabel = BitConverter.GetBytes((UInt16)16);


        public static byte[] CreateReloadRequest()
        {
            return ReloadStore;
        }

        public static byte[] CreateChangeSearchText()
        {
            return ChangeSearchText;
        }

        public static byte[] CreateDirCountRequest()
        {
            return DirCount;
        }


        public static byte[] CreateDirRequest(int ix)
        {
            var req = new List<byte>();
            req.AddRange(DirRequest);
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateDirFileCountRequest(int ix)
        {
            var req = new List<byte>();
            req.AddRange(DirFileCount);
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateFileRequest(int dirIx, int fileIx)
        {
            var req = new List<byte>();
            req.AddRange(FileRequest);
            req.AddRange(BitConverter.GetBytes((UInt32)dirIx));
            req.AddRange(BitConverter.GetBytes((UInt32)fileIx));
            return req.ToArray();
        }

        public static byte[] CreateSortRequest(SortColumn column, SortOrder order)
        {
            var req = new List<byte>();
            req.AddRange(Sort);
            req.AddRange(BitConverter.GetBytes((UInt32)column));
            req.AddRange(BitConverter.GetBytes((UInt32)order));
            return req.ToArray();
        }

        public static byte[] CreateLabelAddRequest()
        {
            return LabelAdd;
        }

        public static byte[] CreateLabelRemoveRequest(int id)
        {
            var req = new List<byte>();
            req.AddRange(LabelRemove);
            req.AddRange(BitConverter.GetBytes((UInt32)id));
            return req.ToArray();
        }

        public static byte[] CreateLabelsGetRequest()
        {
            return LabelsGet;
        }

        public static byte[] CreateLabelsGetForEntryRequest(int id)
        {
            var req = new List<byte>();
            req.AddRange(LabelsGetForEntry);
            req.AddRange(BitConverter.GetBytes((UInt32)id));
            return req.ToArray();
        }

        public static byte[] CreateAddLabelsToDir(IEnumerable<int> ids, IEnumerable<int> labelIds)
        {
            var req = new List<byte>();
            req.AddRange(AddLabelsToDir);
            AddList(req, ids);
            AddList(req, labelIds);
            return req.ToArray();
        }

        public static byte[] CreateFilterLabel(int labelId, byte state)
        {
            var req = new List<byte>();
            req.AddRange(FilterLabel);
            req.AddRange(BitConverter.GetBytes(labelId));
            req.Add(state);

            return req.ToArray();
        }

        public static void AddList(List<byte> request, IEnumerable<int> data)
        {
            request.AddRange(BitConverter.GetBytes((UInt32)data.Count()));
            foreach (var elem in data)
            {
                request.AddRange(BitConverter.GetBytes((UInt32)elem));
            }
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

        //[IgnoreMember]
        //public int Index { get; set; }
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

        //[IgnoreMember]
        //public int Index { get; set; }
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
        public string Name { get; set; }
    }
}
