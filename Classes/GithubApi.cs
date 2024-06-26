using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PES5_WE9_LE_GDB_Manager
{
    [DataContract]
    public class GithubApi
    {
        [DataMember(Name = "url")]
        public string Url;
        [DataMember(Name = "tag_name")]
        public string TagName;
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "body")]
        public string Body;
        [DataMember(Name = "assets")]
        public List<GithubAsset> Assets;
    }
}
