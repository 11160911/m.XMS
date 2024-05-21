using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SVMAdmin
{
    public class iXmsApiParameter
    {
        public string Method { get; set; }
        public string ObjName { get; set; }
        public string UnknowName { get; set; }
        public UserInfo user { get; set; }
    }

    public class UserInfo
    {
        public string CompanyId { get; set; }
        public string OperUnitID { get; set; }
        public string UserID { get; set; }
    }

    public class ApiSetting
    {
        public bool isTestSite { get; set; }
        public string LogPath { get; set; }
        public string url_HTADDVIP_ET { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    internal class AutoCompleteData
    {
        [System.Runtime.Serialization.DataMember]
        public string label { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string value { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    internal class AutoCompleteDataArray
    {
        [System.Runtime.Serialization.DataMember]
        public List<AutoCompleteData> list { get; set; }
    }


}

namespace SVMAdmin.Config
{
    public class ModuleForDB
    {
        public string ModuleName { get; set; }
        public string DB { get; set; }
    }

    public class Company
    {
        public string CompanyID { get; set; }
        public string iXmsApiUrl { get; set; }
        public List<ModuleForDB> Modules { get; set; }
    }

    public class ThisSiteConfig
    {
        public bool isTestSite { get; set; }
        public string LogPath { get; set; }
        public string SecurityKey { get; set; }
        public List<Company> Companys { get; set; }
    }

}