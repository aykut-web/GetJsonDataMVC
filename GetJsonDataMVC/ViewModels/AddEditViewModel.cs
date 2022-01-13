using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetJsonDataMVC.ViewModels
{
    public class AddEditViewModel
    {
        public bool Adding { get; set; }
        public string Message { get; set; }
        public string HostingDomainName { get; set; }
        public string HostingPackage { get; set; }
        public int IncomingConnections { get; set; }
        public int CpuLoad { get; set; }
        public int RamMax { get; set; }
        public int RamUsage { get; set; }
    }

}