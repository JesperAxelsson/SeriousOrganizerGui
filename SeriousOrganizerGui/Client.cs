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


        static int id = 0;
        public void SendTest(String text)
        {
            Console.WriteLine("Hello there");

            var obj = new Test2 { Id = text, Thing = id };
            id++;

            _client.SendRequest(RequestType.CreateTestRequest(), obj);

            var response = _client.WaitResponse<Test2>();
            Console.WriteLine("Got back: " + response?.ToString() ?? "<null>");
        }

        public int SendTextSearchChanged(String text)
        {
            _client.SendRequest(RequestType.CreateChangeSearchText(), text);
            return (int)_client.WaitResponseU32();
        }

        public int GetDirCount()
        {
            Console.WriteLine("Get dir count");

            _client.SendRequest(RequestType.CreateDirCountRequest());

            var response = _client.WaitResponseU32();
            //Console.WriteLine("Got back: " + response?.ToString() ?? "<null>");
            return (int)response;
        }

        public DirEntry GetDir(int ix)
        {
            //Console.WriteLine("Sending dir request");
            _client.SendRequest(RequestType.CreateDirRequest(ix));

            var response = _client.WaitResponse<DirEntry>();
            //Console.WriteLine("Got back: " + response?.ToString() ?? "<null>");
            return response;
        }


        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
