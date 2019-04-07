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
        public static Client Client => _client;

        public static List<Label> GetLabels()
        {
            return DataClient.Client.SendLabelsGet();
        }

    }
}
