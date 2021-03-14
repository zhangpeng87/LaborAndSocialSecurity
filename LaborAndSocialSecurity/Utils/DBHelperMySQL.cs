using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Utils
{
    public static class DBHelperMySQL
    {
        public static string connectionString = ConfigurationManager.AppSettings[""];

        public static DataSet Query(string SQLString, string dbName = "zhgd_lw")
        {
            DataSet ds = new DataSet();

            using (var client = new SshClient("122.189.155.124", 9011, "root", "Admin20181028"))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    var portForwarded = new ForwardedPortLocal(IPAddress.Loopback.ToString(), 3306, "10.0.0.201", 3306);
                    client.AddForwardedPort(portForwarded);
                    portForwarded.Start();
                    using (MySqlConnection connection = new MySqlConnection($"SERVER={ portForwarded.BoundHost };PORT={ portForwarded.BoundPort };DATABASE={ dbName };UID=zzsa;PASSWORD=NL88tNkfHnE3kFgT"))
                    {
                        try
                        {
                            MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                            command.Fill(ds, "ds");

                            LogUtils4Debug.Logger.Debug(new { Query = SQLString, Result = ds.Tables[0] }.Serialize2JSON());
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            return ds;
        }
    }
}
