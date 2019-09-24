using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;

using SeriousOrganizerGui.Dto;
using SeriousOrganizerGui.Extensions;

namespace SeriousOrganizerGui
{
    public class Client : IDisposable
    {
        private NamedPipeClientStream _client = new NamedPipeClientStream(".", "dude", direction: PipeDirection.InOut);
        public void Connect()
        {
            try { 
            //_client = new NamedPipeClientStream(".", "dude", direction: PipeDirection.InOut);
            _client.Connect();
            _client.ReadMode = PipeTransmissionMode.Byte;
            Console.WriteLine("Up and running!");

            }
            catch (Exception ex) { var s = ex.ToString(); }
        }

        public void SendReloadRequest(Action? doneCallback = null)
        {
            _client.SendRequest(RequestType.CreateReloadRequest());
            var resp = _client.WaitResponseU32();
            doneCallback?.Invoke();
        }

        public int SendTextSearchChanged(String text)
        {
            _client.SendRequest(RequestType.CreateChangeSearchText(), text);
            return (int)_client.WaitResponseU32();
        }

        public int GetDirCount()
        {
            _client.SendRequest(RequestType.CreateDirCountRequest());

            var response = _client.WaitResponseU32();
            Console.WriteLine($"Get dir count {response} {(int)response}");
            return (int)response;
        }

        public DirEntry GetDir(int ix)
        {
            _client.SendRequest(RequestType.CreateDirRequest(ix));
            var response = _client.WaitResponse<DirEntry>();
            response.Index = ix;
            return response;
        }

        public int GetFileCount(int dirIx)
        {
            _client.SendRequest(RequestType.CreateDirFileCountRequest(dirIx));
            return (int)_client.WaitResponseU32();
        }

        public FileEntry GetFile(int dirIx, int fileIx)
        {
            _client.SendRequest(RequestType.CreateFileRequest(dirIx, fileIx));
            var response = _client.WaitResponse<FileEntry>();
            response.Index = fileIx;
            return response;
        }

        public int SendSortOrder(SortColumn column, SortOrder order)
        {
            _client.SendRequest(RequestType.CreateSortRequest(column, order));
            return (int)_client.WaitResponseU32();
        }


        public int SendLabelAdd(String name)
        {
            _client.SendRequest(RequestType.CreateLabelAddRequest(), name);
            return (int)_client.WaitResponseU32();
        }

        public int SendLabelRemove(int id)
        {
            _client.SendRequest(RequestType.CreateLabelRemoveRequest(id));
            return (int)_client.WaitResponseU32();
        }

        public List<Label> SendLabelsGet()
        {
            _client.SendRequest(RequestType.CreateLabelsGetRequest());
            var response = _client.WaitResponse<List<Label>>();
            return new List<Label>(response);
        }

        public List<int> SendLabelsGetForEntry(int dirId)
        {
            _client.SendRequest(RequestType.CreateLabelsGetForEntryRequest(dirId));
            var response = _client.WaitResponse<List<int>>();
            return new List<int>(response);
        }

        public int SendAddLabelsToDir(IEnumerable<int> dirId, IEnumerable<int> labelIds)
        {
            _client.SendRequest(RequestType.CreateAddLabelsToDir(dirId, labelIds));
            return (int)_client.WaitResponseU32();
        }

        public int FilterLabel(int labelId, byte state)
        {
            _client.SendRequest(RequestType.CreateFilterLabel(labelId, state));
            return (int)_client.WaitResponseU32();
        }

        public int SendLocationAdd(string name, string path)
        {
            _client.SendRequest(RequestType.CreateLocationAddRequest(name, path));
            return (int)_client.WaitResponseU32();
        }

        public int SendLocationRemove(int locationId)
        {
            _client.SendRequest(RequestType.CreateLocationRemoveRequest(locationId));
            return (int)_client.WaitResponseU32();
        }

        public List<Location> SendLocationGet()
        {
            _client.SendRequest(RequestType.CreateLocationGetRequest());
            var response = _client.WaitResponse<List<Location>>();
            return new List<Location>(response);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
