using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetJsonDataMVC.ViewModels
{
    public class IndexViewModel
    {
        public List<IndexRowResponse> UserList { get; set; }
        public string Json { get; set; }
        public string Arama { get; set; }
    }

    public class IndexRowResponse
    {
        public string HostingDomainName { get; set; }
        public string HostingPackage { get; set; }
        public int IncomingConnections { get; set; }
        public int CpuLoad { get; set; }
        public int RamMax { get; set; }
        public int RamUsage { get; set; }
    }
}