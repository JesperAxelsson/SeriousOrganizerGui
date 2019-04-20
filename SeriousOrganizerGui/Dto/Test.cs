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
        private const UInt16 DirRequest = 1;
        private const UInt16 FileRequest = 2;
        private const UInt16 AddPath = 3;
        private const UInt16 RemovePath = 4;
        private const UInt16 ReloadStore = 5;
        private const UInt16 ChangeSearchText = 6;
        private const UInt16 DirCount = 7;
        private const UInt16 DirFileCount = 8;
        private const UInt16 DeletePath = 9;
        private const UInt16 Sort = 10;
        private const UInt16 LabelAdd = 11;
        private const UInt16 LabelRemove = 12;
        private const UInt16 LabelsGet = 13;
        private const UInt16 LabelsGetForEntry = 14;
        private const UInt16 AddLabelsToDir = 15;
        private const UInt16 FilterLabel = 16;


        public static byte[] CreateReloadRequest()
        {
            return BitConverter.GetBytes(ReloadStore);
        }

        public static byte[] CreateChangeSearchText()
        {
            return BitConverter.GetBytes(ChangeSearchText);
        }

        public static byte[] CreateDirCountRequest()
        {
            return BitConverter.GetBytes(DirCount);
        }


        public static byte[] CreateDirRequest(int ix)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(DirRequest));
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateDirFileCountRequest(int ix)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(DirFileCount));
            req.AddRange(BitConverter.GetBytes((UInt32)ix));
            return req.ToArray();
        }

        public static byte[] CreateFileRequest(int dirIx, int fileIx)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(FileRequest));
            req.AddRange(BitConverter.GetBytes((UInt32)dirIx));
            req.AddRange(BitConverter.GetBytes((UInt32)fileIx));
            return req.ToArray();
        }

        public static byte[] CreateSortRequest(SortColumn column, SortOrder order)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(Sort));
            req.AddRange(BitConverter.GetBytes((UInt32)column));
            req.AddRange(BitConverter.GetBytes((UInt32)order));
            return req.ToArray();
        }

        public static byte[] CreateLabelAddRequest()
        {
            return BitConverter.GetBytes(LabelAdd);
        }

        public static byte[] CreateLabelRemoveRequest(int id)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(LabelRemove));
            req.AddRange(BitConverter.GetBytes((UInt32)id));
            return req.ToArray();
        }

        public static byte[] CreateLabelsGetRequest()
        {
            return BitConverter.GetBytes(LabelsGet);
        }

        public static byte[] CreateLabelsGetForEntryRequest(int id)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(LabelsGetForEntry));
            req.AddRange(BitConverter.GetBytes((UInt32)id));
            return req.ToArray();
        }

        public static byte[] CreateAddLabelsToDir(IEnumerable<int> ids, IEnumerable<int> labelIds)
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(AddLabelsToDir));
            AddList(req, ids);
            AddList(req, labelIds);
            return req.ToArray();
        }

        public static byte[] CreateFilterLabel(int labelId, byte state )
        {
            var req = new List<byte>();
            req.AddRange(BitConverter.GetBytes(FilterLabel));
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
