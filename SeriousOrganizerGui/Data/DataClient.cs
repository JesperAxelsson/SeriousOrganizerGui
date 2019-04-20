using SeriousOrganizerGui.Data.Internal;
using SeriousOrganizerGui.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Data
{
    public class DataClient
    {
        private DataClient() { }

        public static readonly Client _client = new Client();
        private static bool _isConnected = false;

        public static Client Client => _client;
        public static LabelHandler Label;

        public static void Connect()
        {
            if (!_isConnected)
            {
                _isConnected = true;
                _client.Connect();
                Label = new LabelHandler(_client);
            }
        }


    }

    namespace Internal
    {
        public class LabelHandler
        {

            protected Client _client;

            public LabelHandler(Client client)
            {
                _client = client;
                Update();
            }


            private BetterObservable<Label> _labelList = new BetterObservable<Label>();

            public BetterObservable<Label> Get()
            {
                return _labelList;
            }
            public void Update()
            {
                _labelList.Replace(_client.SendLabelsGet());
            }

            public void Add(string name)
            {
                _client.SendLabelAdd(name);
                Update();
            }

            public void Remove(int id)
            {
                _client.SendLabelRemove(id);
                Update();
            }

            public List<int> GetForEntry(int id)
            {
                return _client.SendLabelsGetForEntry(id);
            }

            public void AddLabelsToEntry(IEnumerable< int >entryIds, IEnumerable<int> labelIds)
            {
                _client.SendAddLabelsToDir(entryIds, labelIds);
            }

            public void FilterLabel(int id , byte state)
            {
                _client.FilterLabel(id, state);
            }
        }
    }
}
