using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;

using SeriousOrganizerGui.Dto;

namespace SeriousOrganizerGui
{
    public class Client : IDisposable

    {
        private NamedPipeClientStream _client;
        public void Connect()
        {
            _client = new NamedPipeClientStream(".", "dude", direction: PipeDirection.InOut);
            _client.Connect();
            _client.ReadMode = PipeTransmissionMode.Message;
            Console.WriteLine("Up and running!");
        }

        public void SendReloadRequest(Action doneCallback)
        {
            Task.Run(() =>
            {
                _client.SendRequest(RequestType.CreateChangeSearchText());
                var resp = _client.WaitResponseU32();
                doneCallback();
            });
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
            return response;
        }


        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
