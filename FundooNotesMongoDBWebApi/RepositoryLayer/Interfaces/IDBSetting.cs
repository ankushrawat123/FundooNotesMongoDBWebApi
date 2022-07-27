using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface IDBSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
