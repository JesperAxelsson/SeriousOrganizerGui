using SeriousOrganizerGui.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Data.Providers
{

    public class DirEntryProvider : IItemProvider<DirEntry>
    {
        private readonly Client _client;

        public DirEntryProvider(Client client)
        {
            _client = client;
        }

        public int GetCount()
        {
            return _client.GetDirCount();
        }

        public DirEntry GetItem(int index)
        {
            return _client.GetDir(index);
        }
    }

    public class FileEntryProvider : IItemProvider<FileEntry>
    {
        private readonly Client _client;
        private readonly int _dirIndex;

        public FileEntryProvider(Client client, int dirIndex)
        {
            _client = client;
            _dirIndex = dirIndex;
        }

        public int GetCount()
        {
            return _client.GetFileCount(_dirIndex);
        }

        public FileEntry GetItem(int index)
        {
            return _client.GetFile(_dirIndex, index);
        }
    }
}
