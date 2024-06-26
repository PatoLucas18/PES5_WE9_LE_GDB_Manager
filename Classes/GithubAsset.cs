using System.Runtime.Serialization;

namespace PES5_WE9_LE_GDB_Manager
{
    [DataContract]
    public class GithubAsset
    {
        [DataMember(Name = "browser_download_url")]
        public string DownloadUrl;
    }
}