using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Utils
{
    class ArDBConnection

    {
        public static string strConnectinfo = System.Configuration.ConfigurationSettings.AppSettings["ARDB_ConnectString"];
        private static SqlConnection connection = new SqlConnection(strConnectinfo);
        private static DateTime dtConnect = DateTime.Now;
        public static void OpenConnect()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        public static void CloseConnect()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        public static void ExceuteSQLNoReturn(string strSQL)
        {
            //OpenConnect();
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            try
            {
                connectionTemp.Open();
                SqlCommand pCommand = new SqlCommand(strSQL, connectionTemp);
                pCommand.ExecuteNonQuery();
                pCommand.Dispose();
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("  ExceuteSQLNoReturn:  " + strSQL + "  " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            //connectionTemp.Open();
            //SqlCommand pCommand = new SqlCommand(strSQL, connectionTemp);
            //pCommand.ExecuteNonQuery();
            //pCommand.Dispose();
            //CloseConnect();
        }
        public static object ExceuteSQLScalar(string strSQL)
        {
            object pObj = null;
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            try
            {
                connectionTemp.Open();
                SqlCommand pCommand = new SqlCommand(strSQL, connectionTemp);
                pObj = pCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("  ExceuteSQLScalar:  " + strSQL + "  " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            //OpenConnect();
            //SqlCommand pCommand = new SqlCommand(strSQL, connection);
            //pObj = pCommand.ExecuteScalar();
            //CloseConnect();
            return pObj;
        }
        public static DataTable ExceuteSQLDataTable(string strSQL)
        {
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            DataTable pTable = new DataTable();
            try
            {
                connectionTemp.Open();
                SqlCommand cmd = new SqlCommand(strSQL, connectionTemp);
                SqlDataAdapter da = new SqlDataAdapter(cmd); //创建DataAdapter数据适配器实例    
                DataSet ds = new DataSet();//创建DataSet实例
                da.Fill(ds, "s");
                pTable = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("  ExceuteSQLDataTable:  " + strSQL + "  " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            return pTable;
            //OpenConnect();
            //SqlCommand cmd = new SqlCommand(strSQL, connection);
            //SqlDataAdapter da = new SqlDataAdapter(cmd); //创建DataAdapter数据适配器实例    
            //DataSet ds = new DataSet();//创建DataSet实例
            //da.Fill(ds, "s");
            //CloseConnect();
            //return ds.Tables[0];
        }
        //public static int GetID()
        //{
        //    int iValue = 0;
        //    OpenConnect();
        //    string strSQL = "select IDSec from Table_SEC";
        //    SqlCommand pCommand = new SqlCommand(strSQL, connection);
        //    object pObj = pCommand.ExecuteScalar();
        //    iValue = Convert.ToInt32(pObj);
        //    strSQL = "Update Table_SEC set IDSec=" + (iValue + 1);
        //    pCommand.CommandText = strSQL;
        //    pCommand.ExecuteNonQuery();
        //    pCommand.Dispose();
        //    return iValue;
        //}
        //public static int SetID(int step)
        //{
        //    int iValue = 0;
        //    OpenConnect();
        //    string strSQL = "select IDSec from Table_SEC";
        //    SqlCommand pCommand = new SqlCommand(strSQL, connection);
        //    object pObj = pCommand.ExecuteScalar();

        //    iValue = Convert.ToInt32(pObj);
        //    strSQL = "Update Table_SEC set IDSec=" + (iValue + step);

        //    pCommand.CommandText = strSQL;
        //    pCommand.ExecuteNonQuery();
        //    pCommand.Dispose();
        //    return iValue;
        //}
        public static void PatInsert(DataTable pTb)
        {
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            try
            {
                connectionTemp.Open();
                using (var bulkCopy = new SqlBulkCopy(connectionTemp))
                {
                    for (int i = 0; i < pTb.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(pTb.Columns[i].ColumnName, pTb.Columns[i].ColumnName);
                    }
                    bulkCopy.DestinationTableName = "dbo." + pTb.TableName;
                    bulkCopy.WriteToServer(pTb.Select());
                }
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("   PatInsert: " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            //OpenConnect();
            //using (var bulkCopy = new SqlBulkCopy(connection))
            //{
            //    for (int i = 0; i < pTb.Columns.Count; i++)
            //    {
            //        bulkCopy.ColumnMappings.Add(pTb.Columns[i].ColumnName, pTb.Columns[i].ColumnName);
            //    }
            //    bulkCopy.DestinationTableName = "dbo." + pTb.TableName;
            //    bulkCopy.WriteToServer(pTb.Select());
            //}
            //CloseConnect();
        }
        //public static bool TableExit(string strTableName)
        //{
        //    try
        //    {
        //        bool flag = false;
        //        OpenConnect();
        //        SqlCommand cmd = new SqlCommand("JudgeTableExit", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strTableName", strTableName);　　//给输入参数赋值
        //        SqlParameter parOutput = cmd.Parameters.Add("@retureValue", SqlDbType.Int);　　//定义输出参数
        //        parOutput.Direction = ParameterDirection.Output;　　//参数类型为Output
        //        SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
        //        parReturn.Direction = ParameterDirection.ReturnValue;
        //        cmd.ExecuteNonQuery();
        //        int iflag = Convert.ToInt32(parOutput.Value);
        //        cmd.Dispose();
        //        if (iflag == 1)
        //        {
        //            flag = true;
        //        }
        //        return flag;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return false;
        //    }

        //}

        public static DataTable ExceuteSQLProcedue(string strProcedueName, string[] strParamNames, string[] strParamValues)
        {
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            DataTable pTable = new DataTable();
            try
            {
                connectionTemp.Open();
                DataSet ds = new DataSet();
                //OpenConnect();
                SqlCommand cmd = new SqlCommand(strProcedueName, connectionTemp);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 240;
                for (int i = 0; i < strParamNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@" + strParamNames[i], strParamValues[i]);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds, "s");
                adapter.Dispose();
                cmd.Dispose();
                pTable = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("  ExceuteSQLProcedue:  " + strProcedueName + "  " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            return pTable;

            //DataSet ds = new DataSet();
            //OpenConnect();
            //SqlCommand cmd = new SqlCommand(strProcedueName, connection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //for (int i = 0; i < strParamNames.Length; i++)
            //{
            //    cmd.Parameters.AddWithValue("@" + strParamNames[i], strParamValues[i]);
            //}
            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //adapter.Fill(ds, "s");
            //adapter.Dispose();
            //cmd.Dispose();
            //CloseConnect();
            //return ds.Tables[0];
        }
        public static object ExceuteSQLProcedueReturnValue(string strProcedueName, string[] strParamNames, string[] strParamValues)
        {
            SqlConnection connectionTemp = new SqlConnection(strConnectinfo);
            object pObj = null;
            try
            {
                connectionTemp.Open();
                SqlCommand cmd = new SqlCommand(strProcedueName, connectionTemp);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < strParamNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@" + strParamNames[i], strParamValues[i]);
                }
                cmd.Parameters.Add(new SqlParameter("@CValue", SqlDbType.Int));
                cmd.Parameters["@CValue"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                pObj = cmd.Parameters["@CValue"].Value;
            }
            catch (Exception ex)
            {
                ProjectLog.WriteLog2("  ExceuteSQLProcedueReturnValue:  " + strProcedueName + "  " + ex.Message);
            }
            finally
            {
                connectionTemp.Close();
            }
            return pObj;

            //OpenConnect();
            //SqlCommand cmd = new SqlCommand(strProcedueName, connection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //for (int i = 0; i < strParamNames.Length; i++)
            //{
            //    cmd.Parameters.AddWithValue("@" + strParamNames[i], strParamValues[i]);
            //}
            //cmd.Parameters.Add(new SqlParameter("@CValue", SqlDbType.Int));
            //cmd.Parameters["@CValue"].Direction = ParameterDirection.Output;

            //cmd.ExecuteNonQuery();
            //pObj = cmd.Parameters["@CValue"].Value;
            //CloseConnect();
            //return pObj;
        }

        //public static void DropTable(string strTableName)
        //{
        //    try
        //    {
        //        OpenConnect();
        //        SqlCommand cmd = new SqlCommand("JudgeTableExit", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strTableName", strTableName);　　//给输入参数赋值
        //        SqlParameter parOutput = cmd.Parameters.Add("@retureValue", SqlDbType.Int);　　//定义输出参数
        //        parOutput.Direction = ParameterDirection.Output;　　//参数类型为Output
        //        SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
        //        parReturn.Direction = ParameterDirection.ReturnValue;
        //        cmd.ExecuteNonQuery();
        //        int iflag = Convert.ToInt32(parOutput.Value);
        //        cmd.Dispose();
        //        if (iflag == 1)
        //        {
        //            string strSQL = "Drop table " + strTableName;
        //            SqlCommand cmd2 = new SqlCommand();
        //            cmd2.Connection = connection;
        //            cmd2.CommandText = strSQL;
        //            cmd2.ExecuteNonQuery();
        //            cmd2.Dispose();
        //        }
        //        CloseConnect();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //        CloseConnect();
        //    }
        //}
    }
}
