using System.Collections.Generic;

namespace PBPModSwitch.Core
{
    class MapFileInformation
    {
        public string MapPrefix { get; set; }

        public MemoryFileInformation MapFile { get; set; }

        public MemoryFileInformation ProgramFile { get; set; }

        public Dictionary<string, MemoryFileInformation> NodesFiles { get; set; }

        public Dictionary<string, long> ScanResults { get; set; }

        public MapFileInformation(string mapPrefix, Dictionary<string, long> scanResult)
        {
            MapPrefix = mapPrefix;
            MapFile = new MemoryFileInformation();
            ProgramFile = new MemoryFileInformation();
            NodesFiles = new Dictionary<string, MemoryFileInformation>();
            ScanResults = scanResult;
        }

        public int NodesFilesActiveCount()
        {
            int counter = 0;

            foreach (string nodeFile in NodesFiles.Keys)
            {
                if (NodesFiles[nodeFile].IsModActive)
                    counter++;
            }
            return counter;
        }
    }

    public class MemoryFileInformation
    {
        public string FullName { get; set; }

        public bool IsModActive { get; set; }

        public long MemoryPtr { get; set; }

        public MemoryFileInformation()
        {
            FullName = "";
            IsModActive = false;
            MemoryPtr = 0;
        }

        public bool isValidPtr()
        {
            return MemoryPtr > 0;
        }
    }
}
