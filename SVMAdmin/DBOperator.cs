using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SVMAdmin
{
    public class DBOperator : IDisposable
    {
        private string LogPath = "";
        private string strCommField = ",SGID,CREATE_DATE,MODIFY_DATE,CREATE_TIME,MODIFY_TIME,CREATE_USER,MODIFY_USER,STATUS,";
        private string strCommFieldUp = ",MODIFY_DATE,MODIFY_TIME,MODIFY_USER,";
        private double dblCounterValue;
        private string vSgid = "";
        private string CompanyID = "";
        //System.Collections.Hashtable HTConn = new System.Collections.Hashtable();
        private Dictionary<string, System.Data.SqlClient.SqlConnection> HTConn = new Dictionary<string, SqlConnection>();
        string conns = "";

        public DBOperator()
        {
            //CompanyID = ConstList.CompanyID;
            //LogPath = ConstList.LogPath + @"\" + CompanyID + @"\";
            //LogPath = LogPath.AdjPathByOS();
            //if (System.IO.Directory.Exists(LogPath) == false)
            //    System.IO.Directory.CreateDirectory(LogPath);
        }

  
        private System.Data.SqlClient.SqlConnection SetConnectionByModule(UserInfo vUser, string ModuleID)
        {
            if (vUser.CompanyId == "" | vUser.CompanyId == null) {
                vUser.CompanyId = "BLACK";
            }
            CompanyID = vUser.CompanyId;
            LogPath = ConstList.ThisSiteConfig.LogPath.Trim();
            if (! LogPath.EndsWith(@"\"))
                LogPath += @"\";
            LogPath = ConstList.ThisSiteConfig.LogPath + CompanyID + @"\";
            if (!System.IO.Directory.Exists(LogPath))
                System.IO.Directory.CreateDirectory(LogPath);
            System.Data.SqlClient.SqlConnection cnX = null;
            string cnKey = CompanyID.ToUpper() + "_" + ModuleID.ToUpper();
            if (HTConn.ContainsKey(cnKey))
                cnX = (System.Data.SqlClient.SqlConnection)HTConn[cnKey];
            else
            {
                cnX = GetConnection(vUser, ModuleID);
                HTConn.Add(cnKey, cnX);
                conns += "," + cnKey;
            }
            return cnX;
        }

        internal string Add(string TableName, DataTable dt, UserInfo vUser, System.Data.SqlClient.SqlConnection cn , string ModuleID)
        {
            DataTable dtInfo = DbopUtility.GetTableInfo(TableName, vUser, ModuleID, cn);
            string strSql;
            for (int i = dt.Columns.Count -1; i>-1; i-- )
            {
                if (!dtInfo.Columns.Contains(dt.Columns[i].ColumnName))
                    dt.Columns.Remove(dt.Columns[i]);
            }
            string sql1 = "insert into " + TableName + " (";
            string sql2 = " values (";
            List<string> allFields = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                sql1 += col.ColumnName + ",";
                sql2 += "@" + col.ColumnName + ",";
                allFields.Add(col.ColumnName);
            }
            string[] exFields = "CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,SGID,CREATE_DATE,MODIFY_DATE,CREATE_TIME,MODIFY_TIME,CREATE_USER,MODIFY_USER".Split(",");
            for (int i=0; i < exFields.Length; i++)
            {
                string fd = exFields[i];
                if (dtInfo.Columns.Contains(fd) & !dt.Columns.Contains(fd))
                {
                    sql1 += fd + ",";
                    sql2 += "@" + fd + ",";
                    allFields.Add(fd);
                }
            }
            string sqlCmd = sql1.Substring(0, sql1.Length - 1) + ")" + sql2.Substring(0, sql2.Length - 1) + ")";
            DataTable dtmp;
            if (dtInfo.Columns.Contains("SGID"))
            {
                using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        string strCon = cn.ConnectionString;
                        System.Data.SqlClient.SqlConnection cnX = GetConnection(vUser, "SYS");
                        strSql = "UPDATE sy_COUNTER set CNT_VALUE=CNT_VALUE+" + Convert.ToString(dt.Rows.Count) + " WHERE CNT_CODE='SGID'";
                        Int32 es = ExecuteSql(strSql, vUser, cnX);
                        if (es == 0)
                            throw new Exception("Can not found counter : SGID ");
                        strSql = "select CNT_VALUE FROM sy_COUNTER WHERE CNT_CODE='SGID'";
                        dtmp = Query(strSql, vUser, cnX);
                        dblCounterValue = Convert.ToDouble(dtmp.Rows[0][0]) - dt.Rows.Count + 1;
                        ts.Complete();
                        cnX.Close();
                    }
                    catch (Exception err)
                    {
                        throw new Exception(err.Message);
                    }
                    ts.Dispose();
                }
            }

            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
            {
                string dnow = DateTime.Now.ToString("yyyy/MM/dd");
                string tnow = DateTime.Now.ToString("HH:mm:ss");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sqlCmd, cn);
                    string strForLog = sqlCmd;
                    foreach (string field in allFields)
                    {
                        string fieldL = field.ToLower();
                        SqlParameter SP = null;
                        if (fieldL == "sgid")
                        { 
                            vSgid = sgidstr(dblCounterValue);
                            SP = cm.Parameters.AddWithValue("@" + field, vSgid);
                        }
                        else if (fieldL == "companycode")
                            SP = cm.Parameters.AddWithValue("@" + field, vUser.CompanyId);
                        else if (fieldL == "crtuser" | fieldL == "moduser" | fieldL == "create_user" | fieldL == "modify_user")
                            SP = cm.Parameters.AddWithValue("@" + field, vUser.UserID);
                        else if (fieldL == "crtdate" | fieldL == "moddate" | fieldL == "create_date" | fieldL == "modify_date")
                            SP = cm.Parameters.AddWithValue("@" + field, dnow);
                        else if (fieldL == "crttime" | fieldL == "modtime" | fieldL == "create_time" | fieldL == "modify_time")
                            SP = cm.Parameters.AddWithValue("@"  + field, tnow);
                        else
                            SP = cm.Parameters.AddWithValue("@" + field, dt.Rows[i][field]);
                        if (SP != null)
                        {
                            if (SP.Value == DBNull.Value)
                                strForLog = strForLog.Replace("@" + field, "null");
                            else if (dtInfo.Columns[field].DataType == typeof(string))
                                strForLog = strForLog.Replace("@" + field, "N'" + SP.Value.ToString().SqlQuote() + "'");
                            else if (PubUtility.IsNumericColumn(dtInfo.Columns[field]))
                                strForLog = strForLog.Replace("@" + field, SP.Value.ToString());
                            else if (dtInfo.Columns[field].DataType == typeof(DateTime) || dtInfo.Columns[field].DataType == typeof(DateTimeOffset))
                                strForLog = strForLog.Replace("@" + field, "'" + Convert.ToDateTime(SP.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        }
                    }
                    try
                    {
                        DateTime d1 = DateTime.Now;
                        cm.ExecuteNonQuery();
                        DateTime d2 = DateTime.Now;
                        WriteLogFile(LogPath + DateTime.Now.ToString("yyyyMMdd") + "SqlExec.Log", strForLog + " --; " + (d2 - d1).ToString() + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                    }
                    catch (Exception ex)
                    {
                        WriteLogFile(LogPath + "SqlError.Log", ex.Message);
                        WriteLogFile(LogPath + "SqlError.Log", strForLog + " --; " + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                        throw new Exception(ex.Message);
                    }
                    dblCounterValue += 1;
                }
                ts.Complete();
                ts.Dispose();
            }
            
            return vSgid;
        }

        public string Add(string TableName, DataTable dt, UserInfo vUser, string ModuleID)
        {
            return Add(TableName, dt, vUser, SetConnectionByModule(vUser, ModuleID), ModuleID);
        }

        internal Int32 Update(string TableName, DataTable dt, string[] UniqueKey, UserInfo vUser, System.Data.SqlClient.SqlConnection cn , string ModuleID)
        {
            DataTable dtInfo = DbopUtility.GetTableInfo(TableName, vUser, ModuleID , cn);
            int UpCnt = 0;
            for (int i = dt.Columns.Count - 1; i > -1; i--)
            {
                if (!dtInfo.Columns.Contains(dt.Columns[i].ColumnName))
                    dt.Columns.Remove(dt.Columns[i]);
            }
            string sql1 = "update " + TableName + " set ";
            string sql2 = " where ";
            List<string> allFields = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                string fd = col.ColumnName;
                if (UniqueKey.Contains(fd))
                {
                    sql2 += fd + "=@" + fd + " and ";
                }
                else
                {
                    sql1 += fd + "=@" + fd + ",";
                }
                allFields.Add(col.ColumnName);
            }
            string[] exFields = "ModUser,ModDate,ModTime,MODIFY_DATE,MODIFY_TIME,MODIFY_USER".Split(",");
            for (int i = 0; i < exFields.Length; i++)
            {
                string fd = exFields[i];
                if (dtInfo.Columns.Contains(fd) & !dt.Columns.Contains(fd))
                {
                    sql1 += fd + "=@" + fd + ",";
                    allFields.Add(fd);
                }
            }
            string sqlCmd = sql1.Substring(0, sql1.Length - 1) + sql2.Substring(0, sql2.Length - 5);

            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
            {
                string dnow = DateTime.Now.ToString("yyyy/MM/dd");
                string tnow = DateTime.Now.ToString("HH:mm:ss");
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sqlCmd, cn);
                    string strForLog = sqlCmd;
                    foreach (string field in allFields)
                    {
                        string fieldL = field.ToLower();
                        SqlParameter SP = null;
                        if (fieldL == "moduser"| fieldL == "modify_user")
                            SP = cm.Parameters.AddWithValue("@" + field, vUser.UserID);
                        else if ( fieldL == "moddate" | fieldL == "modify_date")
                            SP = cm.Parameters.AddWithValue("@" + field, dnow);
                        else if ( fieldL == "modtime" | fieldL == "modify_time")
                            SP = cm.Parameters.AddWithValue("@" + field, tnow);
                        else
                            SP = cm.Parameters.AddWithValue("@" + field, dt.Rows[i][field]);
                        if (SP != null)
                        {
                            if (SP.Value == DBNull.Value)
                                strForLog = strForLog.Replace("@" + field, "null");
                            else if (dtInfo.Columns[field].DataType == typeof(string))
                                strForLog = strForLog.Replace("@" + field, "N'" + SP.Value.ToString().SqlQuote() + "'");
                            else if (PubUtility.IsNumericColumn(dtInfo.Columns[field]))
                                strForLog = strForLog.Replace("@" + field, SP.Value.ToString());
                            else if (dtInfo.Columns[field].DataType == typeof(DateTime) || dtInfo.Columns[field].DataType == typeof(DateTimeOffset))
                                strForLog = strForLog.Replace("@" + field, "'" + Convert.ToDateTime(SP.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        }
                    }
                    try
                    {
                        DateTime d1 = DateTime.Now;
                        UpCnt = cm.ExecuteNonQuery();
                        DateTime d2 = DateTime.Now;
                        WriteLogFile(LogPath + DateTime.Now.ToString("yyyyMMdd") + "SqlExec.Log", strForLog + " --; " + (d2 - d1).ToString() + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                    }
                    catch (Exception ex)
                    {
                        WriteLogFile(LogPath + "SqlError.Log", ex.Message);
                        WriteLogFile(LogPath + "SqlError.Log", strForLog + " --; " + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                        throw new Exception(ex.Message);
                    }

                    

                }
                ts.Complete();
                ts.Dispose();
            }
            return UpCnt;
        }

        public Int32 Update(string TableName, DataTable dt, UserInfo vUser, string ModuleID)
        {
            return Update(TableName, dt, new string[] { "SGID" }, vUser, SetConnectionByModule(vUser, ModuleID), ModuleID);
        }

        public Int32 Update(string TableName, DataTable dt, string[] UniqueKey, UserInfo vUser, string ModuleID)
        {
            return Update(TableName, dt, UniqueKey, vUser, SetConnectionByModule(vUser, ModuleID), ModuleID);
        }


        internal Int32 Delete(string TableName, DataTable dt, UserInfo vUser, System.Data.SqlClient.SqlConnection cn)
        {
            string strSql;
            int rt = 0;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    strSql = "delete from " + TableName + " where SGID='" + dt.Rows[i]["SGID"].ToString().Trim() + "'";
                    rt += ExecuteSql(strSql, vUser, cn);
                }
            }
            return rt;
        }

        public Int32 Delete(string TableName, DataTable dt, UserInfo vUser, string ModuleID)
        {
            return Delete(TableName, dt, vUser, SetConnectionByModule(vUser, ModuleID));
        }

        internal string BulkCopy(string TableName, DataTable dt, UserInfo vUser, System.Data.SqlClient.SqlConnection cn)
        {
            string strSql;
            DataTable dtmp;
            //========
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                try
                {
                    string strCon = cn.ConnectionString;
                    System.Data.SqlClient.SqlConnection cnX = GetConnection(vUser, "SYS");
                    strSql = "UPDATE sy_COUNTER set CNT_VALUE=CNT_VALUE+" + Convert.ToString(dt.Rows.Count) + " WHERE CNT_CODE='SGID'";
                    Int32 es = ExecuteSql(strSql, vUser, cnX);
                    if (es == 0)
                        throw new Exception("Can not found counter : SGID ");
                    strSql = "select CNT_VALUE FROM sy_COUNTER WHERE CNT_CODE='SGID'";
                    dtmp = Query(strSql, vUser, cnX);
                    dblCounterValue = Convert.ToDouble(dtmp.Rows[0][0]) - dt.Rows.Count + 1;
                    ts.Complete();
                    cnX.Close();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                ts.Dispose();
            }


            string[] cfd = strCommField.Split(",", StringSplitOptions.RemoveEmptyEntries);
            for (int i=0; i < cfd.Length; i++)
            {
                if (!dt.Columns.Contains(cfd[i]))
                    dt.Columns.Add(cfd[i], typeof(string));
            }


            string strDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strTime = strDate.Substring(11);
            strDate = strDate.Substring(0, 10);
            foreach (DataRow dr in dt.Rows)
            {
                //",SGID,CREATE_DATE,MODIFY_DATE,CREATE_TIME,MODIFY_TIME,CREATE_USER,MODIFY_USER,STATUS,";
                dr["SGID"] = sgidstr(dblCounterValue);
                dr["CREATE_DATE"] = strDate;
                dr["MODIFY_DATE"] = strDate;
                dr["CREATE_TIME"] = strTime;
                dr["MODIFY_TIME"] = strTime;
                dr["CREATE_USER"] = vUser.UserID;
                dr["MODIFY_USER"] = vUser.UserID;
                dr["STATUS"] = "A";
                dblCounterValue++;
            }
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
            {
                SqlBulkCopy sbc = new SqlBulkCopy(cn);
                sbc.BulkCopyTimeout = 180;
                sbc.DestinationTableName = TableName;
                sbc.WriteToServer(dt);
                ts.Complete();
                ts.Dispose();
            }
            return vSgid;
        }

        public string BulkCopy(string TableName, DataTable dt, UserInfo vUser, string ModuleID)
        {
            return BulkCopy(TableName, dt, vUser, SetConnectionByModule(vUser, ModuleID));
        }


        internal DataTable Query(string Sql, UserInfo vUser, System.Data.SqlClient.SqlConnection cn)
        {
            DateTime d1 = DateTime.Now;
            //GetConnect(vUser, ModuleID);
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(Sql, cn);
            DateTime d2 = DateTime.Now;
            if (Sql.IndexOf("select CNT_VALUE FROM sy_COUNTER WHERE CNT_CODE='SGID'") < 0)
                WriteLogFile(LogPath + DateTime.Now.ToString("yyyyMMdd") + "SqlExec.Log", Sql + " --; " + (d2 - d1).ToString() + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            DataTable dt = null;
            DataSet ds = new DataSet();
            try
            {
                da.SelectCommand.CommandTimeout = 180;
                ds.EnforceConstraints = false;
                da.Fill(ds);
                dt = ds.Tables[0];
                ds.Tables.Remove(dt);
            } 
            catch (Exception ex)
            {
                WriteLogFile(LogPath + "SqlError.Log", ex.Message);
                WriteLogFile(LogPath + "SqlError.Log", Sql + " --; " + (d2 - d1).ToString() + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                throw new Exception(ex.Message);
            }
            return dt;
        }

        public DataTable Query(string Sql, UserInfo vUser, string ModuleID)
        {
            return Query(Sql, vUser, SetConnectionByModule(vUser, ModuleID));
        }


        internal Int32 ExecuteSql(string Sql, UserInfo vUser, System.Data.SqlClient.SqlConnection cn)
        {
            DateTime d1 = DateTime.Now;
            //GetConnect(vUser, ModuleID);
            System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(Sql, cn);
            cm.CommandTimeout = 60;
            Int32 af;
            try
            {
                af = cm.ExecuteNonQuery();
                DateTime d2 = DateTime.Now;
                if (Sql.IndexOf("CNT_VALUE=CNT_VALUE+") < 0)
                    WriteLogFile(LogPath + DateTime.Now.ToString("yyyyMMdd") + "SqlExec.Log", Sql + " --; " + (d2 - d1).ToString() + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                WriteLogFile(LogPath + "SqlError.Log", ex.Message);
                WriteLogFile(LogPath + "SqlError.Log", Sql + " --; " + ";" + vUser.UserID + " ; " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                throw new Exception(ex.Message);
            }
            //cn.Close();
            return af;
        }

        public Int32 ExecuteSql(string Sql, UserInfo vUser, string ModuleID)
        {
            return ExecuteSql(Sql, vUser, SetConnectionByModule(vUser, ModuleID));
        }

        private string sgidstr(double cntv)
        {
            double[] B = new double[] {
                1 ,
                50 ,
                2500 ,
                125000 ,
                6250000 ,
                312500000 ,
                15625000000
            };

            string[] S = new string[] {
                "(",
                ")",
                "*",
                "+",
                ",",
                "-",
                ".",
                "/",
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                ":",
                ";",
                "<",
                "=",
                ">",
                "?",
                "@",
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "I",
                "J",
                "K",
                "L",
                "M",
                "N",
                "O",
                "P",
                "Q",
                "R",
                "S",
                "T",
                "U",
                "V",
                "W",
                "X",
                "Y"
            };
            int[] sv = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            for (int ii = 6; ii >= 0; ii--)
            {
                if (cntv >= B[ii])
                    sv[ii] = Convert.ToInt32(Math.Floor(cntv / B[ii]));
                else
                    sv[ii] = 0;
                cntv -= sv[ii] * B[ii];
            }
            string sstr = "";
            for (int ii = 6; ii >= 0; ii--)
                sstr += S[sv[ii]];

            string rt = sstr;
            cntv = Math.Floor(DateTime.Now.ToOADate());
            for (int ii = 2; ii >= 0; ii--)
            {
                if (cntv >= B[ii])
                    sv[ii] = Convert.ToInt32(Math.Floor(cntv / B[ii]));
                else
                    sv[ii] = 0;
                cntv -= sv[ii] * B[ii];
            }
            sstr = "";
            for (int ii = 2; ii >= 0; ii--)
                sstr += S[sv[ii]];

            rt = sstr + rt;
            return rt;
        }

        public System.Data.SqlClient.SqlConnection GetConnection(UserInfo vUser, string ModuleID)
        {
            System.Data.SqlClient.SqlConnection cn;
            try
            {
                string strCon = DbopUtility.GetConnectionString(vUser, ModuleID);
                cn = new System.Data.SqlClient.SqlConnection(strCon);
                cn.Open();
            }
            catch (Exception err)
            {
                throw err;
            }
            return cn;

        }


        private string GetConnectionString_old(UserInfo vUser, string ModuleID)
        {
            string strCon = "";
            try
            {
                Config.ThisSiteConfig TSC = ConstList.ThisSiteConfig;
                strCon = TSC.Companys
                    .Where<Config.Company>(C => C.CompanyID == vUser.CompanyId).ToList<Config.Company>()[0]
                    .Modules.Where<Config.ModuleForDB>(M => M.ModuleName == ModuleID).ToList<Config.ModuleForDB>()[0].DB;
                LogPath = TSC.LogPath;

            }
            catch
            {
                throw new Exception("未設定DB連線，CompanyId:" + vUser.CompanyId + "ModuleID:" + ModuleID);
            }
            return strCon;
        }

        

        private string InfoAdd(string strColName, UserInfo vUser)
        {
            string strRt = "";
            switch (strColName)
            {
                case "SGID":
                    strRt = sgidstr(dblCounterValue);
                    vSgid = strRt;
                    break;
                case "CREATE_DATE":
                    strRt = DateNowString();
                    break;
                case "MODIFY_DATE":
                    strRt = DateNowString();
                    break;
                case "CREATE_TIME":
                    strRt = TimeNowString();
                    break;
                case "MODIFY_TIME":
                    strRt = TimeNowString();
                    break;
                case "CREATE_USER":
                    strRt = vUser.UserID;
                    break;
                case "MODIFY_USER":
                    strRt = vUser.UserID;
                    break;
                case "COMPANYCODE":
                    strRt = CompanyID;
                    break;
                case "STATUS":
                    strRt = "A";
                    break;
            }
            return strRt;
        }

        private string DateNowString()
        {
            return System.DateTime.Now.ToString("yyyy/MM/dd");
        }

        private string TimeNowString()
        {
            return System.DateTime.Now.ToString("HH:mm:ss");
        }

        static object lockMe = new object();
        private void WriteLogFile(string path, string strData)
        {
            lock (lockMe)
            {
                try
                {
                    using (System.IO.StreamWriter sw =
                            new System.IO.StreamWriter(path, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(strData);
                        sw.Close();
                    }

                }
                catch (Exception err)
                {
                    Random rd = new Random(DateTime.Now.Second);
                    using (System.IO.StreamWriter sw =
                            new System.IO.StreamWriter(path + "Err" + rd.Next(0, 9999).ToString().PadLeft(4, '0'), true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(err.Message);
                        sw.WriteLine(strData);
                        sw.Close();
                    }

                }
            }

        }

        bool disposed = false;
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                try
                {
                    string[] keys = conns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < keys.Length; i++)
                    {
                        System.Data.SqlClient.SqlConnection cnX = (System.Data.SqlClient.SqlConnection)HTConn[keys[i]];
                        cnX.Close();
                    }
                }
                catch { }
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~DBOperator()
        {
            Dispose(false);
        }
    }

    public static class DbopUtility
    {
        public static string GetConnectionString(UserInfo vUser, string ModuleID)
        {
            string strCon = "";
            try
            {
                Config.ThisSiteConfig TSC = ConstList.ThisSiteConfig;
                strCon = TSC.Companys
                    .Where<Config.Company>(C => C.CompanyID == vUser.CompanyId).ToList<Config.Company>()[0]
                    .Modules.Where<Config.ModuleForDB>(M => M.ModuleName == ModuleID).ToList<Config.ModuleForDB>()[0].DB;
                //LogPath = TSC.LogPath;

            }
            catch
            {
                throw new Exception("未設定DB連線，CompanyId:" + vUser.CompanyId + "ModuleID:" + ModuleID);
            }
            return GetConnectionStringSqlClient(strCon);
        }

        private static string GetConnectionStringSqlClient(string str)
        {
            string strCon = str;
            string[] ss = strCon.Split(new char[] { ';' });
            strCon = "";
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].ToUpper().IndexOf("Password".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("User ID".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Initial Catalog".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Data Source".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Server".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Database".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Uid".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Pwd".ToUpper()) > -1
                    | ss[i].ToUpper().IndexOf("Connection Timeout".ToUpper()) > -1
                    )
                    strCon += ";" + ss[i];
            }
            strCon = strCon.Substring(1);
            return strCon;
        }

        private static IDictionary<string,DataTable> TableInfos = new Dictionary<string, DataTable>();

        public static DataTable GetTableInfo(string TableName, UserInfo vUser, string ModuleID, System.Data.SqlClient.SqlConnection cn)
        {
            DataTable dt = null;
            string key = vUser.CompanyId + "_" + ModuleID + "_" + TableName;
            if (!TableInfos.ContainsKey(key))
            {
                DataSet ds = new DataSet();
                //string strConn = GetConnectionString(vUser, ModuleID);
                //System.Data.SqlClient.SqlConnection cn = new SqlConnection(strConn);
                //System.Data.SqlClient.SqlConnection cn = SetConnectionByModule()
                //cn.Open();
                try
                {
                    string sql = "select * from " + TableName + " where 1=2";
                    System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, cn);
                    da.SelectCommand.CommandTimeout = 180;
                    ds.EnforceConstraints = false;
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    ds.Tables.Remove(dt);
                    //cn.Close();
                }
                catch (Exception ex)
                {
                    //cn.Close();
                    throw new Exception(ex.Message);
                }
                TableInfos.Add(key, dt);
            }
            else
                dt = TableInfos[key];
            return dt;
        }

    }

}
