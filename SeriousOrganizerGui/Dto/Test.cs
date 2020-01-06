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
        private static byte[] DirRequest = BitConverter.GetBytes((UInt16)1);
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
        private static byte[] LocationAdd = BitConverter.GetBytes((UInt16)17);
        private static byte[] LocationRemove = BitConverter.GetBytes((UInt16)18);
        private static byte[] LocationsGet = BitConverter.GetBytes((UInt16)19);


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
            return CreateIntRequest(DirRequest, ix).ToArray();
        }

        public static byte[] CreateDirFileCountRequest(int ix)
        {
            return CreateIntRequest(DirFileCount, ix).ToArray();
        }

        public static byte[] CreateFileRequest(int dirIx, int fileIx)
        {
            return CreateIntRequest(FileRequest, dirIx, fileIx).ToArray();
        }

        public static byte[] CreateSortRequest(SortColumn column, SortOrder order)
        {
            return CreateIntRequest(Sort, (Int32)column, (Int32)order).ToArray();
        }

        public static byte[] CreateLabelAddRequest()
        {
            return LabelAdd;
        }

        public static byte[] CreateLabelRemoveRequest(int id)
        {
            return CreateIntRequest(LabelRemove, id).ToArray();
        }

        public static byte[] CreateLabelsGetRequest()
        {
            return LabelsGet;
        }

        public static byte[] CreateLabelsGetForEntryRequest(int id)
        {
            return CreateIntRequest(LabelsGetForEntry, id).ToArray();
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
            var req = CreateIntRequest(FilterLabel, labelId);
            req.Add(state);
            return req.ToArray();
        }

        public static byte[] CreateLocationAddRequest(string name, string path)
        {
            var req = new List<byte>();
            req.AddRange(LocationAdd);
            AddString(req, name);
            AddString(req, path);
            return req.ToArray();
        }

        public static byte[] CreateLocationRemoveRequest(int locationId)
        {
            return CreateIntRequest(LocationRemove, locationId).ToArray();
        }
        public static byte[] CreateLocationGetRequest()
        {
            return LocationsGet;
        }

        private static List<byte> CreateIntRequest(byte[] requestType, int number)
        {
            var req = new List<byte>();
            req.AddRange(requestType);
            req.AddRange(BitConverter.GetBytes((UInt32)number));
            return req;
        }

        private static List<byte> CreateIntRequest(byte[] requestType, int number, int number2)
        {
            var req = new List<byte>();
            req.AddRange(requestType);
            req.AddRange(BitConverter.GetBytes((UInt32)number));
            req.AddRange(BitConverter.GetBytes((UInt32)number2));
            return req;
        }

        public static void AddInt32(List<byte> request, int data)
        {
            request.AddRange(BitConverter.GetBytes((UInt32)data));
        }

        public static void AddList(List<byte> request, IEnumerable<int> data)
        {
            request.AddRange(BitConverter.GetBytes((UInt32)data.Count()));
            foreach (var elem in data)
            {
                request.AddRange(BitConverter.GetBytes((UInt32)elem));
            }
        }

        public static void AddString(List<byte> request, string str)
        {
            byte[] utf16bytes = Encoding.Default.GetBytes(str);
            var utf8string = Encoding.UTF8.GetString(utf16bytes);
            byte[] utf8bytes = Encoding.UTF8.GetBytes(utf8string);

            //var st= Encoding.UTF8.GetString(utf8bytes);

            request.AddRange(BitConverter.GetBytes((UInt32)utf8bytes.Count()));
            request.AddRange(utf8bytes);
        }
    }

#nullable disable

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

    [MessagePackObject, ToString]
    public class Location
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Path { get; set; }
        [Key(3)]
        public Int64 Size { get; set; }
    }

#nullable restore
}
