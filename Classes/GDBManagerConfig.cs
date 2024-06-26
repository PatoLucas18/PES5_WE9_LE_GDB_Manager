using System.Runtime.Serialization;

namespace PES5_WE9_LE_GDB_Manager
{
    [DataContract]
    public class GDBManagerConfig
    {
        [DataMember]
        public string OFPath;
        [DataMember]
        public string ExePath;
        [DataMember]
        public string GDBFolderPath;

    }
}
