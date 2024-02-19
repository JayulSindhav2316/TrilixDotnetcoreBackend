using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Telerik.Reporting.Data.Schema;
using Telerik.WebReportDesigner.Services;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public class MySqlSettingsStorage : ISettingsStorage
    {
        readonly string connectionString;
        readonly string reportConnectionString;

        public MySqlSettingsStorage(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("Default");
            this.reportConnectionString = configuration.GetConnectionString("Report");
        }
        public void AddConnection(ConnectionInfo connectionInfo)
        {
            throw new NotImplementedException("This should not be hit when using the server. Use the native endpoints instead.");
        }

        public IEnumerable<ConnectionInfo> GetConnections()
        {
            List<ConnectionInfo> enumerable = new List<ConnectionInfo>
            {
                //CreateModel(this.connectionString,"Default"),
                CreateModel(this.reportConnectionString, "Report")
            };

            //IEnumerable<ConnectionInfo> enumerable = CreateModel(this.connectionString);

            List<ConnectionInfo> list = new List<ConnectionInfo>();
            foreach (ConnectionInfo item in enumerable)
            {
                list.Add(item);
            }

            return list;
        }
        private ConnectionInfo CreateModel(string connections,string name)
        {
            return new ConnectionInfo
            {
                Name = string.Concat("Connection-",name),
                Provider = "MySql.Data.MySqlClient",
                ConnectionString = connections
            };

        }
    }
}
