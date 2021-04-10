using MySql.Data.MySqlClient;
using Polly;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Utils
{
    #region MyRegion
    //public static class DBHelperMySQL
    //{
    //    public static string connectionString = ConfigurationManager.AppSettings[""];

    //    public static DataSet Query(string SQLString, string dbName = "zhgd_lw")
    //    {
    //        DataSet ds = new DataSet();

    //        using (var client = new SshClient("122.189.155.124", 9011, "root", "Admin20181028"))
    //        {
    //            client.Connect();
    //            if (client.IsConnected)
    //            {
    //                var portForwarded = new ForwardedPortLocal(IPAddress.Loopback.ToString(), 3306, "10.0.0.201", 3306);
    //                client.AddForwardedPort(portForwarded);
    //                portForwarded.Start();
    //                using (MySqlConnection connection = new MySqlConnection($"SERVER={ portForwarded.BoundHost };PORT={ portForwarded.BoundPort };DATABASE={ dbName };UID=zzsa;PASSWORD=NL88tNkfHnE3kFgT"))
    //                {
    //                    try
    //                    {
    //                        MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
    //                        command.Fill(ds, "ds");

    //                        LogUtils4Debug.Logger.Debug(new { Query = SQLString, Result = ds.Tables[0] }.Serialize2JSON());
    //                    }
    //                    catch (System.Data.SqlClient.SqlException ex)
    //                    {
    //                        throw new Exception(ex.Message);
    //                    }
    //                }
    //            }
    //        }

    //        return ds;
    //    }
    //} 
    #endregion

    class DBHelperMySQL
    {
        private static SshClient client;
        private static ForwardedPortLocal portForwarded;

        static DBHelperMySQL()
        {
            client = new SshClient("122.189.155.124", 9011, "root", "Admin20181028");
            client.Connect();

            if (client.IsConnected)
            {
                portForwarded = new ForwardedPortLocal(IPAddress.Loopback.ToString(), 3306, "10.0.0.201", 3306);

                client.AddForwardedPort(portForwarded);
                portForwarded.Start();
            }
        }

        public static DataSet TryQuery(string SQLString, string dbName = "zhgd_lw")
        {
            return Policy
                        .Handle<SocketException>()
                        .Or<MySqlException>()
                        .Retry(3, (exception, retryCount, context) =>
                        {
                            LogUtils4Error.Logger.Debug($"当前抛出异常：{ exception.InnerException.Message }；开始第{ retryCount }次重试：{ SQLString }... ...");
                        })
                        .Execute(() =>
                        {
                            return Query(SQLString, dbName);
                        });
        }

        private static DataSet Query(string SQLString, string dbName = "zhgd_lw")
        {
            DataSet ds = new DataSet();

            if (client.IsConnected)
            {
                using (MySqlConnection connection = new MySqlConnection($"SERVER={ portForwarded.BoundHost };PORT={ portForwarded.BoundPort };DATABASE={ dbName };UID=zzsa;PASSWORD=NL88tNkfHnE3kFgT"))
                {
                    try
                    {
                        LogUtils4Debug.Logger.Debug($"向品茗数据库查询：{ SQLString }");
                        MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                        command.Fill(ds, "ds");
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    catch (SocketException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection?.Close();
                    }
                }
            }
            return ds;
        }

        public static void Dispose()
        {
            portForwarded.Stop();
            client.Disconnect();
            client.Dispose();
        }
    }
}
