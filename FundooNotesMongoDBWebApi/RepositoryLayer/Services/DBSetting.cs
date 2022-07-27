using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class DBSetting:IDBSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
