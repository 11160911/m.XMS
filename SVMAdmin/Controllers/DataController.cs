using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace SVMAdmin.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DataController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("LoginSys")]
        public ActionResult LoginSys()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LoginSysOK", "" });

            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                UserInfo uu = new UserInfo();
                uu.UserID = "Login";
                uu.CompanyId = CompanyID;


                if (System.Environment.MachineName.ToUpper() == "ANDYNB4")
                {
                    USERID = "008";
                    PASSWORD = "008";
                }

                string sql = "select Man_ID,Man_Name,Password from EmployeeSV ";
                sql += " where CompanyCode='" + CompanyID.SqlQuote() + "'";
                sql += " and Man_ID='" + USERID.SqlQuote() + "'";
                sql += " and Password='" + PASSWORD.SqlQuote() + "'";
                sql += "  and Password<>''";
                DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtTmp.Rows.Count == 0)
                    throw new Exception("密碼錯誤");
                dtTmp.TableName = "dtEmployee";
                dtTmp.Columns.Add("token", typeof(string));
                uu.UserID = USERID;
                string token = PubUtility.GenerateJwtToken(uu);
                dtTmp.Rows[0]["token"] = token;

                ds.Tables.Add(dtTmp);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("LoginSysByApi")]
        public ActionResult LoginSysByApi()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LoginSysOK", "" });

            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                UserInfo uu = new UserInfo();
                uu.UserID = "Login";
                uu.CompanyId = CompanyID;


                DataTable dt = new DataTable("Employee");
                dt.Columns.Add("Man_ID", typeof(string));
                dt.Columns.Add("Password", typeof(string));
                dt.Columns.Add("CompanyCode", typeof(string));

                if (System.Environment.MachineName.ToUpper() == "ANDYNB4")
                {
                    USERID = "008";
                    PASSWORD = "008";
                }

                dt.Rows.Add(new object[] { USERID, PASSWORD, CompanyID });
                DataSet dsP = new DataSet();
                dsP.Tables.Add(dt);

                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "LoginSys";
                AP.ObjName = "";
                AP.UnknowName = "";


                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";


                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());

                    DataTable dtE = dsR.Tables["dtEmployee"];
                    dtE.Columns.Add("token", typeof(string));
                    uu.UserID = USERID;
                    string token = PubUtility.GenerateJwtToken(uu);
                    dtE.Rows[0]["token"] = token;
                    ds.Tables.Add(dtE.Copy());
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetMenuInit")]
        public ActionResult GetMenuInit()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetMenuInitOK", uu.UserID });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dt = ConstList.AllFunction();
                if (ds.Tables["dtAllFunction"] == null)
                    if (dt.DataSet == null)
                        ds.Tables.Add(dt);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("FileUpload")]
        public ActionResult FileUpload()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "FileUploadOK", "" });
            UserInfo uu = PubUtility.GetCurrentUser(this);
            string picSGID = "";
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                string UploadFileType = HttpContext.Request.Form["UploadFileType"];
                if (UploadFileType == "PLU+Pic1" | UploadFileType == "PLU+Pic2")
                {
                    picSGID = ImportPLUPic(files, UploadFileType);
                    DataTable dtMessage = ds.Tables["dtMessage"];
                    dtMessage.Rows[0][1] = picSGID;
                }
            }
            catch (Exception err)
            {
                DataTable dtMessage = ds.Tables["dtMessage"];
                dtMessage.Rows[0][0] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImage")]
        public ActionResult GetImage()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string SGID = PubUtility.DecodeSGID(rq["SGID"]);
                string UU = rq["UU"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = PubUtility.SqlQry("select * from ImageTable where SGID='" + SGID + "'", uu, "SYS");
                DataRow dr = dt.Rows[0];
                string ContentType = dr["DocType"].ToString();
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));
                return File(dr["DocImage"] as byte[], ContentType);
            }
            catch (Exception err)
            {
                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/GetInitGMMacPLUSet")]
        public ActionResult SystemSetup_GetInitGMMacPLUSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitGMMacPLUSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select Type_ID,Type_Name";
                sql += " from TypeData ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='G' Order By Type_ID";
                DataTable dtDept = PubUtility.SqlQry(sql, uu, "SYS");
                dtDept.TableName = "dtDept";
                ds.Tables.Add(dtDept);

                sql = "select Type_ID,Type_Name";
                sql += " from TypeData ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='L' Order By Type_ID";
                DataTable dtBGNo = PubUtility.SqlQry(sql, uu, "SYS");
                dtBGNo.TableName = "dtBGNo";
                ds.Tables.Add(dtBGNo);
                ds.Tables.Add(dtBGNo);

                ds.Tables.Add(dtBGNo);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-04-27
        [Route("SystemSetup/GetInittest")]
        public ActionResult SystemSetup_GetInittest()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitGMMacPLUSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-04-28
        [Route("SystemSetup/GetInitInv")]
        public ActionResult SystemSetup_GetInitInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitInvOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select * from Layer order by Type_ID";
                //DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                //dtPLU.TableName = "dtLayer";
                //ds.Tables.Add(dtPLU);

                sql = "select Distinct WhNo ,WhNo+ST_SName WhName ";
                sql += " from InventorySV a (NoLock) Left Join WarehouseSV b (NoLock) ";
                sql += " On a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " Order By WhNo ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtInvWh";
                ds.Tables.Add(dtWh);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/SuspendPLU")]
        public ActionResult SystemSetup_SuspendPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SuspendPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string GD_NO = rq["GD_NO"];
                string SetSuspend = rq["SetSuspend"];
                string sql = "update PLUSV set GD_Flag1='"+ SetSuspend+"'";
                sql += " where CompanyCode='" + uu.CompanyId + "' And GD_NO='" + GD_NO.SqlQuote() + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");
                sql = "select a.*";
                sql += " ,Case When GD_Flag1='0' Then '未設定' When GD_Flag1='1' Then '啟用' When GD_Flag1='2' Then '停用' End GDStatus ";
                sql += " from PLUSV a";
                sql += " where CompanyCode='" + uu.CompanyId + "' And a.GD_NO='" + GD_NO.SqlQuote() + "'";
                //sql = "select a.*,b.GD_PRICES,b.GD_NAME";
                //sql += " from PLUSVM a";
                //sql += " inner join PLUSV b on a.GD_NO=b.GD_NO";
                //sql += " where b.GD_NO='" + GD_NO.SqlQuote() + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/UpdatePLU")]
        public ActionResult SystemSetup_UpdatePLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdatePLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("PLUSV");
                PubUtility.AddStringColumns(dtRec, "CompanyCode,GD_NO,GD_Sname,Photo1");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];
                dr["CompanyCode"] = uu.CompanyId;
                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "select * from PLUSV where CompanyCode='" + uu.CompanyId + "' And GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                            DataTable dtOld = dbop.Query(sql, uu, "SYS");
                            //sql = "update PLUSVM set ";
                            //sql += " GD_Sname='" + dr["GD_Sname"].ToString().SqlQuote() + "'";
                            //sql += ",Photo1='" + dr["Photo1"].ToString().SqlQuote() + "'";
                            //sql += ",Photo2='" + dr["Photo2"].ToString().SqlQuote() + "'";
                            //sql += " where GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Update("PLUSV", dtRec, new string[] { "CompanyCode" , "GD_NO" }, uu, "SYS");

                            string OldPhoto1 = dtOld.Rows[0]["Photo1"].ToString();
                            if (OldPhoto1 != "" & OldPhoto1 != dr["Photo1"].ToString())
                            {
                                sql = "delete from ImageTable where SGID='" + OldPhoto1 + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //string OldPhoto2 = dtOld.Rows[0]["Photo2"].ToString();
                            //if (OldPhoto2 != "" & OldPhoto2 != dr["Photo2"].ToString())
                            //{
                            //    sql = "delete from ImageTable where SGID='" + OldPhoto2 + "'";
                            //    dbop.ExecuteSql(sql, uu, "SYS");
                            //}

                            sql = "Update PLUSV Set GD_Flag1='1' ";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And GD_No='" + dr["GD_NO"].ToString().SqlQuote() + "' ";
                            sql += " And GD_Flag1='0' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from PLUSV a";
                //sql += " inner join PLU b on a.GD_NO=b.GD_NO";
                sql += " where a.GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchPLU")]
        public ActionResult SystemSetup_SearchPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string GDDept = rq["GDDept"];
                string GDBGNo = rq["GDBGNo"];
                string GDStatus = rq["GDStatus"];
                string sql = "select a.*";
                sql += " ,Case When GD_Flag1='0' Then '未設定' When GD_Flag1='1' Then '啟用' When GD_Flag1='2' Then '停用' End GDStatus ";
                sql += " from PLUSV a";
                //sql += " inner join PLUSV b on a.GD_NO=b.GD_NO";
                sql += " where CompanyCode='" + uu.CompanyId + "' ";
                if (KeyWord != "")
                {
                    sql += " and (";
                    sql += " a.GD_NAME like '%" + KeyWord + "%'";
                    sql += " or a.GD_NO Like '%" + KeyWord + "%'";
                    sql += " or a.GD_Sname Like '%" + KeyWord + "%'";
                    sql += ")";
                }
                if (GDDept != "" & GDDept != null)
                {
                    sql += " and a.GD_Dept='" + GDDept + "'";
                }
                if (GDBGNo != "" & GDBGNo != null)
                {
                    sql += " and a.GD_BGNo='" + GDBGNo + "'";
                }
                if (GDStatus != "" & GDStatus != null)
                {
                    sql += " and a.GD_Flag1='" + GDStatus + "'";
                }
                sql += " Order By GD_No ";

                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ImportPLU")]
        public ActionResult SystemSetup_ImportPLU()
        {

            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ImportPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("PLUSVM");
                PubUtility.AddStringColumns(dtRec, "GD_NO,GD_Sname,Photo1,Photo2");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];
                
                string GD_NO = dtRec.Rows[0]["GD_NO"].ToString();
                string sql = "select * from PLU ";
                sql += " where GD_NO='" + GD_NO.SqlQuote() + "'";
                sql += " and CompanyCode='" + uu.CompanyId + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtPLU.Rows.Count > 0)
                    throw new Exception("這個貨號已經存在,不可匯入");

                DataSet dsP = new DataSet();
                dsP.Tables.Add(dtRec.Copy());
                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "SystemSetupImportPLU";
                AP.ObjName = "";
                AP.UnknowName = "";
                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";

                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                DataTable dtPLUi = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());
                    dtPLUi = dsR.Tables["dtPLU"];
                }
                if (dtPLUi.Rows.Count == 0)
                    throw new Exception("這個貨號不存在,不可匯入");

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            for (int i= dtPLUi.Columns.Count-1; i >-1; i--)
                            {
                                string fname = dtPLUi.Columns[i].ColumnName;
                                if (!dtPLU.Columns.Contains(fname))
                                    dtPLUi.Columns.Remove(fname);
                            }
                            //dbop.BulkCopy("PLU", dtPLUi, uu, "SYS");
                            sql = PubUtility.ConvertInsertSql("PLU", dtPLUi.Rows[0], uu);
                            dbop.ExecuteSql(sql, uu, "SYS");

                            List<string> fds = "CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime".Split(new char[] { ',' }).ToList<string>();
                            foreach (string str in fds)
                                dtRec.Columns.Add(str, typeof(string));
                            sql = PubUtility.ConvertInsertSql("PLUSVM", dtRec.Rows[0], uu);
                            dbop.ExecuteSql(sql, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw err;
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,b.GD_PRICES,b.GD_NAME";
                sql += " from PLUSVM a";
                sql += " inner join PLU b on a.GD_NO=b.GD_NO";
                sql += " where b.GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetPLUFromIXms")]
        public ActionResult GetPLUFromIXms()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            string term = HttpContext.Request.Form["term"].ToString().SqlQuote();
            string sql = "";
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            if (term != "")
            {
                DataTable dtP = new DataTable("dtPLUTerm");
                dtP.Columns.Add("KeyWord", typeof(string));

                dtP.Rows.Add(new object[] { term });
                DataSet dsP = new DataSet();
                dsP.Tables.Add(dtP);

                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "GetPLUFromIXmsByKeyWord";
                AP.ObjName = "";
                AP.UnknowName = "";
                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";

                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());

                    DataTable dt = dsR.Tables["dtPLU"];
                    if (dt != null)
                    {
                        AutoCompleteDataArray ACDA = new AutoCompleteDataArray();
                        ACDA.list = new List<AutoCompleteData>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            AutoCompleteData acd = new AutoCompleteData();
                            acd.label = dr["GD_NAME"].ToString();
                            acd.value = dr["GD_NO"].ToString();
                            //ACDA.List[dt.Rows.IndexOf(dr)] = acd;
                            ACDA.list.Add(acd);
                        }
                        System.Runtime.Serialization.Json.DataContractJsonSerializer js =
                                    new System.Runtime.Serialization.Json.DataContractJsonSerializer(ACDA.GetType());
                        js.WriteObject(ms, ACDA);
                    }
                }
            }
            string ss = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(ss, "application/json");
        }


        [Route("SystemSetup/SearchInv")]
        public ActionResult SystemSetup_SearchInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchInvOK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string GDLayer = rq["GDLayer"];
                string ToExcel = rq["ToExcel"];
                string sql = "select a.WhNo, a.CkNo, a.Layer, a.SNO, a.PLU, b.GD_SNAME, a.EffectiveDate, a.PtNum, a.DisplayNum,";
                //string sql = "select a.*,b.GD_SNAME,";
                sql += " cast(case when DisPlayNum>0 then Cast(Round(Cast(a.PtNum as numeric(5,1))/Cast(a.DisPlayNum as numeric(5,1))*100,0) As Int) Else 0 End As Varchar(10)) + '%' Share";
                sql += " from InventorySV a";
                sql += " inner join PLUSV b on a.PLU=b.GD_NO And a.CompanyCode=b.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (KeyWord != "")
                {
                    sql += " and (";
                    sql += " b.GD_NAME like '" + KeyWord + "%'";
                    sql += " or b.GD_NO='" + KeyWord + "'";
                    sql += " or b.GD_Sname='" + KeyWord + "'";
                    sql += ")";
                }
                if (WhNo != "")
                {
                    sql += " and a.WhNo='" + WhNo + "'";
                }
                if (GDLayer != "")
                {
                    sql += " and a.Layer='" + GDLayer + "'";
                }
                sql += " Order By a.WhNo, a.CkNo, a.Layer ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);

                if (ToExcel == "Y")
                {
                    //dtInv.Columns["SerNo"].SetOrdinal(0);
                    using (OfficeOpenXml.ExcelPackage PKD = new OfficeOpenXml.ExcelPackage())
                    {
                        string[] heads = "店碼,機號,貨倉,貨道,商品代號,商品名稱,最近有效日,庫存量,滿倉量,庫存比".Split(",");

                        OfficeOpenXml.ExcelWorksheet wsD = PKD.Workbook.Worksheets.Add("Inv");
                        for (int i = 0; i < heads.Length; i++)
                            wsD.Cells[1, i + 1].Value = heads[i];
                        wsD.Cells["A2"].LoadFromDataTable(dtInv, false);

                        DataTable dtF = new DataTable();
                        dtF.Columns.Add("DataType", typeof(string));
                        dtF.Columns.Add("FileName", typeof(string));
                        dtF.Columns.Add("DocType", typeof(string));
                        dtF.Columns.Add("DocImage", typeof(byte[]));
                        DataRow dr = dtF.NewRow();
                        dtF.Rows.Add(dr);
                        dr["DataType"] = "Temp";
                        dr["FileName"] = "庫存查詢_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx";
                        dr["DocType"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        dr["DocImage"] = PKD.GetAsByteArray();
                        dtMessage.Rows[0][1] = PubUtility.AddTable("ImageTable", dtF, uu, "SYS");
                        dtMessage.Rows[0][2] = uu.CompanyId;
                        dtMessage.Rows[0][3] = uu.UserID;
                    }
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07
        [Route("SystemSetup/GetWhCkNo")]
        public ActionResult SystemSetup_GetWhCkNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetLayer")]
        public ActionResult SystemSetup_GetLayer()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetLayerOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string sql = "select a.Type_ID,a.Type_Name ";
                sql += " from Layer a";
                sql += " where 1=1";
                //if (KeyWord != "")
                //{
                //sql += " and (";
                //sql += ")";
                //}
                sql += " Order By Type_ID ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtLayer";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        private string ImportPLUPic(IFormFileCollection files, string ParaType)
        {
            string sgid = "";
            UserInfo uu = PubUtility.GetCurrentUser(this);
            foreach (IFormFile file in files)
            {
                string filename = file.FileName;
                if (filename.ToLower().IndexOf(".jpg") < 0 & filename.ToLower().IndexOf(".jpeg") < 0)
                {
                    throw new Exception("必須是.jpg檔案!");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                file.CopyTo(ms);
                DataTable dtF = new DataTable();
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("FileName", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));

                DataRow drF = dtF.NewRow();
                drF["DataType"] = HttpContext.Request.Form["UploadFileType"];
                drF["FileName"] = file.FileName;
                drF["DocType"] = file.ContentType;
                drF["DocImage"] = ms.ToArray();
                dtF.Rows.Add(drF);
                sgid = PubUtility.AddTable("ImageTable", dtF, uu, "SYS");
            }
            return sgid;
        }






        //2021-06-07 Larry
        [Route("SystemSetup/GetInitVMN01")]
        public ActionResult SystemSetup_GetInitVMN01()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/GetTypeData")]
        public ActionResult SystemSetup_GetTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_Code = rq["Type_Code"];
                string sql = "select * from TypeData Where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + Type_Code + "' order by Type_ID";

                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-06-07 Larry
        [Route("SystemSetup/ChkTypeDataUsed")]
        public ActionResult SystemSetup_ChkTypeDataUsed()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkTypeDataUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select Distinct ST_DeliArea from WarehouseSV Where CompanyCode='" + uu.CompanyId + "' And ST_DeliArea='" + Type_ID + "'";

                DataTable dtChk = PubUtility.SqlQry(sql, uu, "SYS");
                dtChk.TableName = "dtChk";
                ds.Tables.Add(dtChk);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/ChkTypeDataExist")]
        public ActionResult SystemSetup_ChkTypeDataExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkTypeDataUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_Code = rq["Type_Code"];
                string Type_ID = rq["Type_ID"];
                string sql = "select * from TypeData Where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + Type_Code + "' And Type_ID='" + Type_ID + "'";

                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/UpdateTypeData")]
        public ActionResult SystemSetup_UpdateTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,OldType_ID,Type_ID,Type_Name");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["Type_ID"].ToString() == dr["OldType_ID"].ToString())
                            {
                                sql = "update TypeData set ";
                                sql += " Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "update TypeData set ";
                                sql += " Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                sql += " ,Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += " ,ModDate=convert(char(10),getdate(),111)";
                                sql += " ,ModTime=convert(char(12),getdate(),108)";
                                sql += " ,ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["OldType_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And a.Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-06-07 Larry
        [Route("SystemSetup/AddTypeData")]
        public ActionResult SystemSetup_AddTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,Type_ID,Type_Name");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //sql = "Insert Into Rack (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            //sql += " ,ModUser, ModDate, ModTime";
                            //sql += ", Type_ID, Type_Name, DisplayNum) Values ";
                            //sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + dr["Type_ID"].ToString().SqlQuote() + "','" + dr["Type_Name"].ToString().SqlQuote() + "'," + dr["DisplayNum"].ToString().SqlQuote() + ")";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Add("TypeData", dtRec, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/DelTypeData")]
        public ActionResult SystemSetup_DelTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,Type_ID");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Delete From TypeData ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }













        //2021-05-07 Larry
        [Route("SystemSetup/GetInitVMN29")]
        public ActionResult SystemSetup_GetInitVMN29()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN29OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/GetRack")]
        public ActionResult SystemSetup_GetRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select * from Rack Where CompanyCode='" + uu.CompanyId + "' order by Type_ID";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-05-07 Larry
        [Route("SystemSetup/ChkRackUsed")]
        public ActionResult SystemSetup_ChkRackUsed()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkRackUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select Distinct ChannelType from MachineListSpec Where CompanyCode='" + uu.CompanyId + "' And ChannelType='" + Type_ID + "'";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/ChkRackExist")]
        public ActionResult SystemSetup_ChkRackExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkRackUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select * from Rack Where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + Type_ID + "'";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/UpdateRack")]
        public ActionResult SystemSetup_UpdateRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "OldType_ID,Type_ID,Type_Name,DisplayNum");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["Type_ID"].ToString() == dr["OldType_ID"].ToString())
                            {
                                sql = "update Rack set ";
                                sql += " Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += ",DisplayNum=" + dr["DisplayNum"].ToString().SqlQuote() + "";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "update Rack set ";
                                sql += " Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                sql += " ,Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += " ,DisplayNum=" + dr["DisplayNum"].ToString().SqlQuote() + "";
                                sql += " ,ModDate=convert(char(10),getdate(),111)";
                                sql += " ,ModTime=convert(char(12),getdate(),108)";
                                sql += " ,ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["OldType_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/DelChgShop")]
        public ActionResult SystemSetup_DelChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "Delete From ChangeShopSV ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }




        //2021-05-07 Larry
        [Route("SystemSetup/AddRack")]
        public ActionResult SystemSetup_AddRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "Type_ID,Type_Name,DisplayNum");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //sql = "Insert Into Rack (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            //sql += " ,ModUser, ModDate, ModTime";
                            //sql += ", Type_ID, Type_Name, DisplayNum) Values ";
                            //sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + dr["Type_ID"].ToString().SqlQuote() + "','" + dr["Type_Name"].ToString().SqlQuote() + "'," + dr["DisplayNum"].ToString().SqlQuote() + ")";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Add("Rack", dtRec, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/DelRack")]
        public ActionResult SystemSetup_DelRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "Type_ID");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Delete From Rack ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/GetInitVXT03")]
        public ActionResult SystemSetup_GetInitVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select ST_ID ,ST_ID+ST_SName STName ";
                sql += " from WarehouseSV (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' And ST_Type ='6'";
                sql += " Order By ST_ID ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);


                sql = "select ST_ID ,ST_ID+ST_SName STName ";
                sql += " from WarehouseSV (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' And ST_Type Not In ('2','3','6')";
                sql += " Order By ST_ID ";

                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/SearchVXT03")]
        public ActionResult SystemSetup_SearchVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string SWhNo = rq["SWhNo"];
                string VMStatus = rq["VMStatus"];
                string NetStatus = rq["NetStatus"];

                string sql = "select a.*,c.ST_SNAME+b.ckno+'機' VMName,b.ST_ID,b.ckno,d.ST_SName SWhName";
                sql += " from MachineList a";
                sql += " inner join WarehouseDSV b on a.SNNo=b.SNNo And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on b.ST_ID=c.ST_ID And b.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on b.WhNoIn=d.ST_ID And b.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and b.ST_ID='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and b.CkNo='" + CkNo + "'";
                }
                if (SWhNo != "")
                {
                    sql += " and b.WhNoIn='" + SWhNo + "'";
                }
                if (VMStatus != "")
                {
                    sql += " and a.FlagUse='" + VMStatus + "'";
                }
                if (NetStatus != "")
                {
                    sql += " and a.FlagNet='" + NetStatus + "'";
                }
                sql += " Order By a.SNNo ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18
        [Route("SystemSetup/GetWhDSVCkNo")]
        public ActionResult SystemSetup_GetWhDSVCkNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhDSVCkNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }




        //2021-05-26 Larry
        [Route("SystemSetup/GetWh")]
        public ActionResult SystemSetup_GetWh()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select ST_ID ,ST_ID+ST_SName STName ";
                sql += " from WarehouseSV (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' And ST_Type ='6'";
                sql += " Order By ST_ID ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        //2021-06-02 Larry
        [Route("SystemSetup/UpdateVXT03")]
        public ActionResult SystemSetup_UpdateVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("MachineList");
                PubUtility.AddStringColumns(dtRec, "SNNo,FlagT,FlagNet");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                        
                            sql = "update MachineList set ";
                            sql += " FlagT='" + dr["FlagT"].ToString().SqlQuote() + "'";
                            sql += ",FlagNet='" + dr["FlagNet"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,c.ST_SNAME+b.ckno+'機' VMName,b.ST_ID,b.ckno,d.ST_SName SWhName";
                sql += " from MachineList a";
                sql += " inner join WarehouseDSV b on a.SNNo=b.SNNo And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on b.ST_ID=c.ST_ID And b.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on b.WhNoIn=d.ST_ID And b.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                //sql = "select a.*";
                //sql += " from MachineList a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }





        //2021-05-26 Larry
        [Route("SystemSetup/SearchVIN13_1")]
        public ActionResult SystemSetup_SearchVIN13_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN13_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string exDate = rq["exDate"];

                string sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " ,c.ST_OpenDay";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and a.WhNoOut='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and a.CkNoOut='" + CkNo + "'";
                }
                if (exDate != "")
                {
                    sql += " and a.exchangeDate='" + exDate + "'";
                }
                sql += " Order By a.DocNo Desc";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        ////2021-06-24 Larry
        //[Route("SystemSetup/ChkChgShopCols")]
        //public ActionResult SystemSetup_ChkChgShopCols()
        //{
        //    UserInfo uu = PubUtility.GetCurrentUser(this);
        //    System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkChgShopColsOK", "" });
        //    DataTable dtMessage = ds.Tables["dtMessage"];
        //    try
        //    {
        //        IFormCollection rq = HttpContext.Request.Form;
        //        string WhNoOut = rq["WhNoOut"];
        //        string CkNoOut = rq["CkNoOut"];
        //        string WhNoIn = rq["WhNoIn"];
        //        string CkNoIn = rq["CkNoIn"];
        //        string ExchangeDate = rq["ExchangeDate"];
        //        string ChkMod = rq["ChkMod"];

        //        if (ChkMod=="App")
        //        {

        //        }
        //        else
        //        {

        //        }

        //        string sql = "select Distinct ChannelType from MachineListSpec Where CompanyCode='" + uu.CompanyId + "' And ChannelType='" + Type_ID + "'";

        //        DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
        //        dtRack.TableName = "dtRack";
        //        ds.Tables.Add(dtRack);

        //    }
        //    catch (Exception err)
        //    {
        //        dtMessage.Rows[0][0] = "Exception";
        //        dtMessage.Rows[0][1] = err.Message;
        //    }
        //    return PubUtility.DatasetXML(ds);
        //}


        //2021-05-26 Larry
        [Route("SystemSetup/UpdateChgShop")]
        public ActionResult SystemSetup_UpdateChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo,WhNoOut,CkNoOut,WhNoIn,CkNoIn,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangeShopSV set ";
                            sql += " WhNoOut='" + dr["WhNoOut"].ToString().SqlQuote() + "'";
                            sql += ",CkNoOut='" + dr["CkNoOut"].ToString().SqlQuote() + "'";
                            sql += ",WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "'";
                            sql += ",CkNoIn='" + dr["CkNoIn"].ToString().SqlQuote() + "'";
                            sql += ",ExchangeDate='" + dr["ExchangeDate"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/AppChgShop")]
        public ActionResult SystemSetup_AppChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            //批核完成後，同樣執行異動完成，故回傳UpdateChgShopOK
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangeShopSV set ";
                            sql += " AppDate=convert(char(10),getdate(),111)";
                            sql += ",AppUser='" + uu.UserID + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/AddChgShop")]
        public ActionResult SystemSetup_AddChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string DocNo = GetNewDocNo(uu, "CS", 3);

                DataTable dtRec = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtRec, "WhNoOut,CkNoOut,WhNoIn,CkNoIn,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Insert Into ChangeShopSV (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            sql += ", ModUser, ModDate, ModTime ";
                            sql += ", DocNo, DocUser, DocDate";
                            sql += ", ExchangeDate, WhNoOut, CkNoOut "
                                +  ", WhNoIn, CkNoIn) Values ";
                            sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + DocNo + "', '" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(12),getdate(),108),1,5) ";
                            sql += ", '" + dr["ExchangeDate"].ToString().SqlQuote() + "','" + dr["WhNoOut"].ToString().SqlQuote() + "','" + dr["CkNoOut"].ToString().SqlQuote() + "'"
                                 + ", '" + dr["WhNoIn"].ToString().SqlQuote() + "', '" + dr["CkNoIn"].ToString().SqlQuote() + "')";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + DocNo + "'";
                DataTable dtChgShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtChgShop.TableName = "dtChgShop";
                ds.Tables.Add(dtChgShop);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-17
        [Route("SystemSetup/ChkWhNo")]
        public ActionResult SystemSetup_ChkWhNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkWhNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];

                string sql = "Select ST_ID,ST_Sname from WarehouseSV (NoLock) ";
                sql += "where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_ID='" + WhNo + "' and ST_Type='6'";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";

                sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitVIN14_2")]
        public ActionResult SystemSetup_GetInitVIN14_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVIN14_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select ST_ID,ST_ID+ST_SName ST_SName";
                sql += " from WarehouseSV ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And ST_Type='6' Order By ST_ID";
                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);

                //sql = "select Type_ID,Type_Name";
                //sql += " from TypeData ";
                //sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='L' Order By Type_ID";
                //DataTable dtBGNo = PubUtility.SqlQry(sql, uu, "SYS");
                //dtBGNo.TableName = "dtBGNo";
                //ds.Tables.Add(dtBGNo);
                //ds.Tables.Add(dtBGNo);

                //ds.Tables.Add(dtBGNo);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/SearchVIN14_2")]
        public ActionResult SystemSetup_SearchVIN14_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string WhNo = rq["WhNo"];
                    string CkNo = rq["CkNo"];
                    string LayerNo = rq["LayerNo"];

                    string DocNo = GetNewDocNo(uu, "VC", 6);

                    string sql = "Insert Into TempDocumentSV ";
                    sql += "(CompanyCode,ModUser,ModDate,ModTime,DocNo,SeqNo,PLU,Qty";
                    sql += ",Qty2,RNum, WhNo, CkNo, Layer, Sno, EffectiveDate, WorkType)";
                    sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                    sql += " ,'" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, 0, "
                            + "0, 0, a.WhNo, a.CkNo, a.Layer, a.Sno, a.EffectiveDate, 'IN' ";
                    sql += " from InventorySV a (Nolock) ";
                    sql += " inner join PLUSV b (Nolock) on a.CompanyCode=b.CompanyCode And a.PLU=b.GD_NO";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' And b.GD_Flag1='1' ";

                    if (WhNo != "" & WhNo != null)
                    {
                        sql += " and a.WhNo='" + WhNo + "'";
                    }
                    if (CkNo != "" & CkNo != null)
                    {
                        sql += " and a.CkNo='" + CkNo + "'";
                    }
                    if (LayerNo != "" & LayerNo != null)
                    {
                        sql += " and a.Layer='" + LayerNo + "'";
                    }
                    dbop.ExecuteSql(sql, uu, "SYS");

                    sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    DataTable dtDocNo = new DataTable("dtDocNo");
                    dtDocNo.Columns.Add("DocNo", typeof(string));

                    DataRow dr = dtDocNo.NewRow();
                    dr["DocNo"] = DocNo;

                    dtDocNo.Rows.Add(dr);
                    ds.Tables.Add(dtDocNo);

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }


        public string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
        {
            string sDocNo = "";
            string sDate;
            string sDateWithSlash = "";
            string sql = "";
            sql = "select convert(char(8),getdate(),112),convert(char(10),getdate(),111)";
            DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
            if (dtDate.Rows.Count == 0)
            {
                sDocNo = "";
                return sDocNo;
            }
            else
            {
                sDate = dtDate.Rows[0][0].ToString().Trim();
                sDateWithSlash = dtDate.Rows[0][1].ToString().Trim();
            }

            sql = "select SeqNo from DocumentNo a";
            sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate=convert(char(8),getdate(),112)";

            DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
            //dtDoc.TableName = "dtDoc";

            using (DBOperator dbop = new DBOperator())

                if (dtDoc.Rows.Count == 0)
                {
                    string str = new string('0', Digits) + "1";
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                    sql = "Insert Into DocumentNo (SGID, CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, Initial, DocDate, SeqNo) ";
                    sql += " Select '" + uu.CompanyId.SqlQuote() + DocType + sDate + "', '" + uu.CompanyId.SqlQuote() + "'"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + DocType + "', '" + sDate + "', 1 ";
                    dbop.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    int SeqNo = Convert.ToInt32(dtDoc.Rows[0][0]) + 1;

                    sql = "Update DocumentNo Set SeqNo=" + SeqNo + " "
                        + " ,ModUser='" + uu.UserID + "', ModDate=convert(char(10),getdate(),111), ModTime=convert(char(8),getdate(),108) "
                        + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate='" + sDate + "'";
                    dbop.ExecuteSql(sql, uu, "SYS");
                    string str = new string('0', Digits) + SeqNo.ToString();
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                }

            return sDocNo;
        }


        //2021-06-21 Larry
        [Route("SystemSetup/UpdateTempSV")]
        public ActionResult SystemSetup_UpdateTempSV()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateTempSVOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,SeqNo,Qty,Qty2,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];
                int Qty1 = Convert.ToInt32(dr["Qty"].ToString().SqlQuote());
                int Qty2 = Convert.ToInt32(dr["Qty2"].ToString().SqlQuote()); ;

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update TempDocumentSV set ";
                            sql += " Qty=" + Qty1 + "";
                            sql += ", Qty2=" + Qty2 + "";
                            sql += ", RNum=" + (Qty2 - Qty1) + "";
                            sql += ",EffectiveDate='" + dr["ExchangeDate"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(10),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + 
                                dr["DocNo"].ToString().SqlQuote() + "' And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                sql += " from tempdocumentsv a";
                sql += " inner join InventorySV b on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                sql += " inner join PLUSV c on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();

                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21 Larry
        [Route("SystemSetup/SaveInv")]
        public ActionResult SystemSetup_SaveInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveInvOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,WhNo,CkNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];

                string DocNo = GetNewDocNo(uu, "TH", 6);

                string sql = "";
                string WhNoOut = "";
                string SysDate = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "select convert(char(10),getdate(),111) SysDate";

                            DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                            if (dtSysDate.Rows.Count > 0)
                            {
                                SysDate = dtSysDate.Rows[0][0].ToString();
                            }

                            sql = "Select WhNoIn From WarehouseDSV " 
                                + "Where CompanyCode='" + uu.CompanyId + "' "
                                + "And ST_ID='" + dr["WhNo"].ToString().SqlQuote() + "' "
                                + "And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                            DataTable dtWhNoOut = PubUtility.SqlQry(sql, uu, "SYS");

                            if (dtWhNoOut.Rows.Count >0) {
                                WhNoOut = dtWhNoOut.Rows[0][0].ToString();
                            }

                            bool bSameWh = false;
                            string CkNoOut = ""; string LayerOut = "";
                            if (WhNoOut == dr["WhNo"].ToString()) {
                                bSameWh = true; CkNoOut = "00"; LayerOut = "Z";
                            }
                            else
                            {
                                bSameWh = false; CkNoOut = "XX"; LayerOut = "";
                            }

                            //寫入調撥資料
                            //調撥表頭
                            sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                            sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + DocNo + "', convert(char(10),getdate(),111), '" +WhNoOut+ "', '" + uu.UserID + "'"
                                + ", convert(char(10),getdate(),111),'" + uu.UserID + "','" + dr["WhNo"].ToString().SqlQuote() + "'"
                                + ", '', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                + ",'" + CkNoOut + "', '" + dr["CkNo"].ToString().SqlQuote() + "', 'IN')";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //調撥表身
                            sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " + 
                                "TH_ID, SeqNo, PLU, OutNum, InNum, " + 
                                "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, EffectiveDate, SnoOut) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, Qty, Qty"
                                 + ", b.GD_Retail, Qty*GD_Retail, a.Layer, a.Sno, '" + LayerOut + "'"
                                 + ", a.EffectiveDate";
                            if (LayerOut == "Z")
                            {
                                sql += ", d.Sno ";
                            }
                            else
                            {
                                sql += ", '' ";
                            }
                            sql += " From TempDocumentSV a (Nolock) ";
                            sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                            //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                            sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU " 
                                +  "And d.WhNo='" + WhNoOut + "' And d.CkNo='" + CkNoOut + "' And d.Layer='Z' ";
                            
                            sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //變更庫存數量及庫存增減日
                            //調出方
                            string sqlTROut = "";
                            sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";

                            if (CkNoOut != "XX")
                            {
                                //寫入調出方jahoInvSV
                                sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", 'TF', TH_ID, WhNoOut, SeqNo, a.PLU, IsNull(b.PtNum,0), -1*OutNum, IsNull(b.PtNum,0)-OutNum"
                                     + ", CkNoOut, LayerOut, SnoOut "
                                     + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode And a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                     + " and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) - OutNum "
                                    + ",Out_Date = Case When Out_Date>'" + SysDate + "' Then Out_Date Else '" + SysDate + "' End "
                                    + "From ("+ sqlTROut + ") a Inner Join InventorySV b " 
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                    + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", WhNo, a.PLU, Out_Date, CkNo, Layer, Sno, PtNum) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoOut, PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum "
                                     + " From (" +sqlTROut+ ") a Left Join InventorySV b " 
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                     + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }


 

                            //調入方
                            string sqlTRIn = "";
                            sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";
                            //寫入調入方jahoInvSV
                            sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime "
                                + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", 'TF', TH_ID, WhNoIn, SeqNo, a.PLU, IsNull(b.PtNum,0), InNum, IsNull(b.PtNum,0)+InNum"
                                 + ", CkNoIn, LayerIn, SnoIn "
                                 + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode And a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                 + " and a.LayerIn = b.Layer And a.SnoIn = b.Sno And a.PLU=b.PLU ";
                            dbop.ExecuteSql(sql, uu, "SYS");                           
                            
                            //已有庫存資料
                            sql = "Update InventorySV "
                                + "Set ModUser='" + uu.UserID + "' "
                                + ",ModDate=convert(char(10),getdate(),111) "
                                + ",ModTime=convert(char(8),getdate(),108) "
                                + ",PtNum=IsNull(PtNum,0) + InNum "
                                + ",In_Date = Case When In_Date>'" + SysDate + "' Then In_Date Else '" + SysDate + "' End "
                                + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                            sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //沒有庫存資料-新增
                            sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime"
                                + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum "
                                 + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                 + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                 + "Where b.PLU Is Null ";
                            dbop.ExecuteSql(sql, uu, "SYS");




                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                //sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                //sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, Qty, d.ST_SName ";
                //sql += " from tempdocumentsv a";
                //sql += " inner join InventorySV b on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                //sql += " inner join PLUSV c on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                //sql += " inner join WarehouseSV d on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();

                //DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                //dtRes.TableName = "dtRes";
                //ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21
        [Route("SystemSetup/GetWhDSVCkNoWithCond")]
        public ActionResult SystemSetup_GetWhDSVCkNoWithCond()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhDSVCkNoWithCondOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string StopDay = rq["StopDay"];
                string CheckUse = rq["CheckUse"];
                string sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " Inner Join MachineList b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                if (StopDay == "Y")
                {
                    sql += " And (IsNull(ST_StopDay,'')='' or ST_StopDay>convert(char(10),getdate(),111)) ";
                }
                if (CheckUse == "Y")
                {
                    sql += " And b.FlagUse='U' ";
                }

                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //private string GetNewDocNo(UserInfo uu,String DocType)
        //{

        //2021-06-21
        [Route("SystemSetup/GetWhCkLayer")]
        public ActionResult SystemSetup_GetWhCkLayer()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string sql = "select Distinct a.LayerNo ";
                sql += " from MachineListSpec a (NoLock) ";
                sql += " Inner Join WarehouseDSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and b.ST_ID='" + WhNo + "' And b.CkNo='" + CkNo + "' ";
                sql += " Order By a.LayerNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }






        //2021-05-26 Larry
        [Route("SystemSetup/GetSysDate")]
        public ActionResult SystemSetup_GetSysDate()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetSysDateOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string Type_ID = rq["Type_ID"];
                string sql = "select convert(char(10),getdate(),111) SysDate";

                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                dtSysDate.TableName = "dtSysDate";
                ds.Tables.Add(dtSysDate);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }







    }
}
