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

        [Route("GetCompanyName")]
        public ActionResult GetCompanyName()
        {
            IFormCollection rq = HttpContext.Request.Form;
            string BeforeCompanyID = rq["companyid"];

            UserInfo uu = new UserInfo();
            uu.UserID = "Login";
            uu.CompanyId = BeforeCompanyID;

            string sql = "";
            sql += "Select ChineseName from CompanyWeb";
            sql += " where CompanyCode='" + BeforeCompanyID + "'";


            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCompanyNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");
                dtTmp.TableName = "dtCompanyName";
                ds.Tables.Add(dtTmp);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
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
                uu.UserID = USERID;  //"Login"
                uu.CompanyId = CompanyID;
                DataTable ldt_Date = PubUtility.SqlQry("Select convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12)", uu, "SYS");

                string sql = "";
                if (System.Environment.MachineName.ToUpper() == "ANDYNB4")
                {
                    USERID = "008";
                    PASSWORD = "008";
                    sql = "select CompanyCode,Man_ID,Man_Name,Password from EmployeeWeb ";
                    sql += " where CompanyCode='" + CompanyID.SqlQuote() + "'";
                    sql += " and Man_ID='" + USERID.SqlQuote() + "'";
                    sql += " and Password='" + PASSWORD.SqlQuote() + "'";
                    sql += "  and Password<>''";
                }
                else
                {
                    sql = "select CompanyCode,Man_ID,Man_Name,CONVERT(varchar(20),CONVERT(varbinary(60),Password)) Password from EmployeeWeb ";
                    sql += " where CompanyCode='" + CompanyID.SqlQuote() + "'";
                    sql += " and Man_ID='" + USERID.SqlQuote() + "'";
                    sql += " and CONVERT(varchar(20),CONVERT(varbinary(60),Password))='" + PASSWORD.SqlQuote() + "'";
                    sql += " and Password<>''";
                }

                DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtTmp.Rows.Count > 0)
                {
                    uu.CompanyId = Convert.ToString(dtTmp.Rows[0]["CompanyCode"]);
                    uu.UserID = Convert.ToString(dtTmp.Rows[0]["Man_ID"]);

                    //20220817 增加檢查員工是否重複登入
                    sql = "Select * from ISAMLoginRecWeb (nolock) ";
                    sql += " where CompanyCode='" + uu.CompanyId + "' and Man_ID='" + uu.UserID + "' ";
                    sql += " and isnull(LogOutType,'')=''";
                    DataTable dtLogin = PubUtility.SqlQry(sql, uu, "SYS");
                    if (dtLogin.Rows.Count > 0)
                    {
                        sql = "Update ISAMLoginRecWeb Set LogOutType='X' Where Companycode='" + uu.CompanyId + "' ";
                        sql += "and Man_ID='" + uu.UserID + "' ";
                        sql += "and isnull(LogOutType,'')='' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                        throw new Exception("二次登入");
                    }
                    else
                    {
                        sql = "Insert into ISAMLoginRecWeb (CompanyCode,CrtUSer,CrtDate,CrtTime,ModUser,ModDate,ModTime,Man_ID,LogInDT,LogOutDT,LogOutType) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "','" + ldt_Date.Rows[0][0].ToString() + "','" + ldt_Date.Rows[0][1].ToString() + "',";
                        sql += "'" + uu.UserID + "','" + ldt_Date.Rows[0][0].ToString() + "','" + ldt_Date.Rows[0][1].ToString() + "',";
                        sql += "'" + uu.UserID + "','" + ldt_Date.Rows[0][0].ToString() + "' + ' ' + left('" + ldt_Date.Rows[0][1].ToString() + "',8),'',''";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                }

                if (dtTmp.Rows.Count == 0)
                    throw new Exception("密碼錯誤");

                dtTmp.TableName = "dtEmployee";
                dtTmp.Columns.Add("token", typeof(string));
                //uu.UserID = USERID;
                string token = PubUtility.GenerateJwtToken(uu);
                dtTmp.Rows[0]["token"] = token;
                ds.Tables.Add(dtTmp);

                sql = "Select * from ISAMLoginRecWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' ";
                sql += "and LoginDT='" + ldt_Date.Rows[0][0].ToString() + "' + ' ' + left('" + ldt_Date.Rows[0][1].ToString() + "',8) ";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                dt.TableName = "dtLogin";
                ds.Tables.Add(dt);
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

                string sql = "select a.Man_Name,b.ChineseName,a.WhNo";
                sql += " from EmployeeWeb a";
                sql += " left join CompanyWeb b on a.CompanyCode=b.CompanyCode";
                sql += " where a.Man_ID='" + uu.UserID + "'";
                sql += " and a.CompanyCode='" + uu.CompanyId + "' ";
                DataTable dtU = PubUtility.SqlQry(sql, uu, "SYS");
                dtU.TableName = "dtEmployeeWeb";
                ds.Tables.Add(dtU);

                //新增登入者的公司別,員工代碼,所屬店櫃
                if (dtU.Rows.Count > 0)
                {
                    sql = "select *";
                    sql += " from ISAMShopWeb (nolock) where CompanyCode='" + uu.CompanyId + "'";
                    sql += " and Man_ID='" + uu.UserID + "' ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    if (dtE.Rows.Count > 0)
                    {
                        sql = "update ISAMShopWeb set WhNo='" + dtU.Rows[0]["WhNo"].ToString() + "'";
                        sql += ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12),ModUser='LOGIN'";
                        sql += " where CompanyCode='" + uu.CompanyId + "' and Man_ID='" + uu.UserID + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                    else
                    {
                        sql = "Insert into ISAMShopWeb (CompanyCode,CrtUSer,CrtDate,CrtTime,ModUser,ModDate,ModTime,Man_ID,WhNo) ";
                        sql += "values ('" + uu.CompanyId + "','LOGIN',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12)";
                        sql += ",'LOGIN',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12)";
                        sql += ",'" + uu.UserID + "','" + dtU.Rows[0]["WhNo"].ToString() + "')";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
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
                if (CkNo != "")
                {
                    sql += " and a.CkNo='" + CkNo + "'";
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
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
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
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("FileName", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
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
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
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
                string sql = "";
                sql = "select WhNO+b.ST_Sname WhName from ISAMShopWeb a (NoLock)";
                sql += " left join WarehouseWeb b (NoLock) on a.companycode=b.companycode and a.whno=b.st_id ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' ";
                sql += " and Man_ID='" + uu.UserID + "' ";
                DataTable dtISAMShop = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtISAMShop.Rows.Count == 0)
                {
                    sql = "select WhNO+b.ST_Sname WhName from EmployeeWeb a (NoLock)";
                    sql += " left join WarehouseWeb b (NoLock) on a.companycode=b.companycode and a.whno=b.st_id ";
                    sql += " Where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and Man_ID='" + uu.UserID + "' ";

                    DataTable dtUserWh = PubUtility.SqlQry(sql, uu, "SYS");
                    dtUserWh.TableName = "dtUserWh";
                    ds.Tables.Add(dtUserWh);
                }
                else
                {
                    dtISAMShop.TableName = "dtUserWh";
                    ds.Tables.Add(dtISAMShop);
                }
                sql = "select ST_ID ,ST_ID+' '+ST_SName STName ";
                sql += " from WarehouseWeb (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' and ST_StopDay=''";
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

        [Route("SystemSetup/SaveST_ID")]
        public ActionResult SaveST_ID()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveWhSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string STID = rq["WHSetSV"];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ISAMShopWeb set ";
                            sql += " WhNo='" + STID.ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Man_ID='" + uu.UserID + "'";
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
                dtMessage.Rows[0][1] = STID;

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
                            sql += " AppDate=convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)";
                            sql += ",AppUser='" + uu.UserID + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += ",SNnoOut=W.SNno";
                            sql += " From ChangeShopSV C Inner Join WarehouseDSV W On C.CompanyCode=W.CompanyCode And C.WhNoOut=W.ST_ID And C.CkNoOut=W.CkNo";
                            sql += " where C.CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            //sql = "update ChangeShopSV set ";
                            //sql += " AppDate=convert(char(10),getdate(),111)";
                            //sql += ",AppUser='" + uu.UserID + "'";
                            //sql += ",ModDate=convert(char(10),getdate(),111)";
                            //sql += ",ModTime=convert(char(12),getdate(),108)";
                            //sql += ",ModUser='" + uu.UserID + "'";
                            //sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
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

                string DocNo = PubUtility.GetNewDocNo(uu, "CS", 3);

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


 


        //public string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
        //{
        //    string sDocNo = "";
        //    string sDate;
        //    string sDateWithSlash = "";
        //    string sql = "";
        //    sql = "select convert(char(8),getdate(),112),convert(char(10),getdate(),111)";
        //    DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
        //    if (dtDate.Rows.Count == 0)
        //    {
        //        sDocNo = "";
        //        return sDocNo;
        //    }
        //    else
        //    {
        //        sDate = dtDate.Rows[0][0].ToString().Trim();
        //        sDateWithSlash = dtDate.Rows[0][1].ToString().Trim();
        //    }

        //    sql = "select SeqNo from DocumentNo a";
        //    sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate=convert(char(8),getdate(),112)";

        //    DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
        //    //dtDoc.TableName = "dtDoc";

        //    using (DBOperator dbop = new DBOperator())

        //        if (dtDoc.Rows.Count == 0)
        //        {
        //            string str = new string('0', Digits) + "1";
        //            sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
        //            sql = "Insert Into DocumentNo (SGID, CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, Initial, DocDate, SeqNo) ";
        //            sql += " Select '" + uu.CompanyId.SqlQuote() + DocType + sDate + "', '" + uu.CompanyId.SqlQuote() + "'"
        //                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
        //                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
        //                 + ", '" + DocType + "', '" + sDate + "', 1 ";
        //            dbop.ExecuteSql(sql, uu, "SYS");
        //        }
        //        else
        //        {
        //            int SeqNo = Convert.ToInt32(dtDoc.Rows[0][0]) + 1;

        //            sql = "Update DocumentNo Set SeqNo=" + SeqNo + " "
        //                + " ,ModUser='" + uu.UserID + "', ModDate=convert(char(10),getdate(),111), ModTime=convert(char(8),getdate(),108) "
        //                + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate='" + sDate + "'";
        //            dbop.ExecuteSql(sql, uu, "SYS");
        //            string str = new string('0', Digits) + SeqNo.ToString();
        //            sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
        //        }

        //    return sDocNo;
        //}


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

                string DocNo = PubUtility.GetNewDocNo(uu, "TH", 6);

                string sql = "";
                string WhNoOut = "";
                string SysDate = "";


                sql = "select convert(char(10),getdate(),111) SysDate";

                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtSysDate.Rows.Count > 0)
                {
                    SysDate = dtSysDate.Rows[0][0].ToString();
                }

                sql = "Select * From TempDocumentSV a ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                sql += " And IsNull(RNum,0) <> 0 ";
                sql += " And IsNull(ModDate,'')<>'' ";
                DataTable dtChkQty = PubUtility.SqlQry(sql, uu, "SYS");


                sql = "Select WhNoIn From WarehouseDSV "
                    + "Where CompanyCode='" + uu.CompanyId + "' "
                    + "And ST_ID='" + dr["WhNo"].ToString().SqlQuote() + "' "
                    + "And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWhNoOut = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtWhNoOut.Rows.Count > 0)
                {
                    WhNoOut = dtWhNoOut.Rows[0][0].ToString();
                }

                bool bSameWh = false;
                string CkNoOut = ""; string LayerOut = "";
                if (WhNoOut == dr["WhNo"].ToString())
                {
                    bSameWh = true; CkNoOut = "00"; LayerOut = "Z";
                }
                else
                {
                    bSameWh = false; CkNoOut = "XX"; LayerOut = "";
                }


                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
 
                            string sqlAdj = "";
                            //異動庫存最近有效日期
                            sqlAdj = "Select * From TempDocumentSV a (Nolock) ";
                            sqlAdj += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            sqlAdj += " And IsNull(RNum,0) = 0 And IsNull(EffectiveDate,'')<>''";
                            sqlAdj += " And IsNull(ModDate,'')<>'' ";

                            sql = "Update InventorySV "
                                + "Set ModUser='" + uu.UserID + "' "
                                + ",ModDate=convert(char(10),getdate(),111) "
                                + ",ModTime=convert(char(8),getdate(),108) "
                                + ",EffectiveDate = a.EffectiveDate "
                                + "From (" + sqlAdj + ") a Inner Join InventorySV b "
                                + "On a.CompanyCode=b.CompanyCode and a.Layer=b.Layer And a.Sno=b.Sno And a.PLU=b.PLU "
                                + "and b.WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' and b.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                            sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";

                            dbop.ExecuteSql(sql, uu, "SYS");

                            //沒有庫存資料-新增
                            sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime"
                                + ", WhNo, PLU, CkNo, Layer, Sno, EffectiveDate) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", a.WhNo, a.PLU, a.CkNo, a.Layer, a.Sno, a.EffectiveDate "
                                 + " From (" + sqlAdj + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode and a.WhNo = b.WhNo and a.CkNo = b.CkNo "
                                 + " and a.Layer=b.Layer And a.Sno=b.Sno And a.PLU=b.PLU "
                                 + "Where b.PLU Is Null ";
                            dbop.ExecuteSql(sql, uu, "SYS");


                            //檢查有數量異動，才寫入調整資料
                            if (dtChkQty.Rows.Count > 0)
                            {

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
                                    + ", '" + DocNo + "', convert(char(10),getdate(),111), '" + WhNoOut + "', '" + uu.UserID + "'"
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
                                //2021/08/09 調整為以 品號 寫入調出貨道
                                sql += ", a.PLU ";
                                //if (LayerOut == "Z")
                                //{
                                //    sql += ", d.Sno ";
                                //}
                                //else
                                //{
                                //    sql += ", '' ";
                                //}
                                sql += " From TempDocumentSV a (Nolock) ";
                                sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                                //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                                sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                    + "And d.WhNo='" + WhNoOut + "' And d.CkNo='" + CkNoOut + "' And d.Layer='Z' ";

                                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                                //2021/08/09 修正
                                sql += " And IsNull(RNum,0) <> 0 ";
                                //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";

                                sql += " And IsNull(a.ModDate,'')<>'' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //變更庫存數量及庫存增減日
                                //調出方
                                string sqlTROut = "";

                                ////#####特殊處理：因 補貨 調出方 非實際的機器，沒有貨倉、貨道，故必須將相同商品Group起來，並以品號作為調出的貨道
                                sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU SnoOut, "
                                    + " Row_Number() Over(Order By H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU) SeqNo, D.PLU, Sum(D.OutNum) OutNum "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' "
                                    + " Group By H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU ";


                                //sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum "
                                //    + " From TransferHSV H Inner Join TransferDSV D "
                                //    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                //    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";

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
                                        + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                                        + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                        + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                    sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //沒有庫存資料-新增
                                    sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                        + ", ModUser, ModDate, ModTime "
                                        + ", WhNo, PLU, Out_Date, CkNo, Layer, Sno, PtNum, SafeNum ) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", WhNoOut, a.PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum, 1 "
                                         + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                         + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                         + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                         + "Where b.PLU Is Null ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }




                                //調入方
                                string sqlTRIn = "";
                                sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum, T.EffectiveDate "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Inner Join TempDocumentSV T On H.CompanyCode = T.CompanyCode And H.WhNoIn=T.WhNo And H.CkNoIn=T.CkNo And D.LayerIn=T.Layer And D.SnoIn=T.Sno "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' And T.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
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
                                    + ",EffectiveDate=a.EffectiveDate "
                                    + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                    + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime"
                                    + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum, SafeNum, EffectiveDate) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum, 1, a.EffectiveDate "
                                     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                     + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }



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


        [Route("SystemSetup/GetPageInitBefore")]
        public ActionResult SystemSetup_GetPageInitBefore()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetPageInitBeforeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                
                string sql = "select Man_ID,Whno from ISAMShopWeb (nolock) ";
                sql += "where Companycode='"+ uu.CompanyId +"' and Man_ID='"+ uu.UserID +"'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtComp";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetWhName")]
        public ActionResult SystemSetup_GetWhName()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select a.WhNo,case when ST_Sname is null then '' else left(a.Whno+'      ',6)+' '+ST_SName end STName from ISAMShopWeb a (nolock) ";
                sql += "left join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
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


        [Route("SystemSetup/GetInitISAM01")]
        public ActionResult SystemSetup_GetInitISAM01()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAM01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select a.Man_ID,left(a.Man_ID+'      ',6)+' '+Man_Name ManName ,a.Whno,left(a.Whno+'      ',6)+' '+ST_SName STName from ISAMShopWeb a (nolock) ";
                sql += "inner join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.Man_ID=b.Man_ID ";
                sql += "inner join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtWh";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchBINWeb")]
        public ActionResult SystemSetup_SearchBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                
                string sql = "select top 1 * from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='"+ Shop +"' and ISAMDate='"+ ISAMDate +"' and BINNO='"+ BinNo + "' and BINman='"+ uu.UserID +"'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0)
                {
                    sql = "select top 1 * from BINWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }
                
                dtC.TableName = "dtBINData";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveBINWeb")]
        public ActionResult SystemSetup_ChkSaveBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select Sum(Qty1)SumQty from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and PLU='" + PLU + "'";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                dt.TableName = "dtBIN";
                ds.Tables.Add(dt);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveBINWeb")]
        public ActionResult SystemSetup_SaveBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select * from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; } 
                else
                {
                    DataRow[] dr = dtC.Select("PLU='"+ PLU +"'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update BINWeb set Qty1=Qty1+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12),";
                        sql += "ISAMTime=CONVERT(varchar,getdate(),108) where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "'";
                        sql += " and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'"; 
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into BINWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,BINStore,BINNO,BINman,ISAMDate,ISAMTime,SeqNo,PLU,QTY1) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + Shop + "','" + BinNo + "','" + uu.UserID + "','" + ISAMDate + "',Convert(varchar,getdate(),108),";
                    sql += "(select isnull(max(seqno),0)+1 from BINWeb (nolock) where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and Binman='" + uu.UserID + "'),";
                    sql += "'" + PLU + "', "+ Qty.ToString() ;
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //分區單品數
                sql = "Select Sum(Qty1) SQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BinNo='" + BinNo + "' and PLU='"+ PLU +"'";
                DataTable dtSQ= PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);
                //分區總和(不看商品)
                sql = "Select Sum(Qty1) SBQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BinNo='" + BinNo + "'";
                DataTable dtSBQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSBQ.TableName = "dtSBQ";
                ds.Tables.Add(dtSBQ);
                //門市總和(不看商品)
                sql = "Select Sum(Qty1) SWQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "'";
                DataTable dtSWQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSWQ.TableName = "dtSWQ";
                ds.Tables.Add(dtSWQ);
                sql = "Select '"+ PLU +"' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetBINWebMod")]
        public ActionResult SystemSetup_GetBINWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetBINWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string Comd = "";

                if (rq.Count > 3)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from BINWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "'"+ Comd +";";
                sql += "select a.PLU,a.Qty1,a.PLU+' '+case when isnull(a.GD_Name,'')='' then isnull(b.GD_Name,'') else isnull(a.GD_Name,'') end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo desc";


                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtBin";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetGDName")]
        public ActionResult SystemSetup_GetGDName()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGDNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PLU = rq["PLU"];
              
                string sql = "select '"+ PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtPLU";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        
        [Route("SystemSetup/DelISAM01PLU")]
        public ActionResult SystemSetup_DelISAM01PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM01PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["PLU"];

                string sql = "Delete from BINWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='"+ PLU +"'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,Binstore,binno,isamdate,seqno) NewSeq,* into #tmpA from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join BINWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.BINStore=b.BINStore and a.BINNO=b.BINNO and a.ISAMDate=b.ISAMDate and a.SeqNo=b.SeqNo;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM01PLUMod")]
        public ActionResult SystemSetup_SaveISAM01PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM01PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update BINWeb set Qty1=" + Qty.SqlQuote() ;
                sql += " where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,Qty1,PLU+' '+GD_Name GD_Name from BINWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,Qty1,PLU+' '+isnull(GD_Name,'') GD_Name from BINWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtBINMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/AddISAMToFTPRecWeb")]
        public ActionResult SystemSetup_AddISAMToFTPRecWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddISAMToFTPRecWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Doctype = rq["Type"];
                string Shop = rq["Shop"];
                string Comd = "";

                switch (Doctype)
                {
                    case "T":
                        if (rq.Count > 2)
                        {
                            string ISAMDate = rq["ISAMDate"];
                            string BinNo = rq["BinNo"];
                            Comd = Shop + ";" + BinNo + ";" + ISAMDate + ";" + uu.UserID;  //店;分區代碼;盤點日期;登入者
                        }
                        else
                        {
                            Comd = Shop+";"+uu.UserID;
                        }
                        break;
                    case "C":
                        Comd = Shop + ";" + uu.UserID;
                        break;
                    case "D":
                        string DocDate = rq["DocDate"];
                        string InShop = rq["WhNoIn"];
                        Comd = Shop + ";" + InShop + ";" + DocDate + ";" + uu.UserID;  //出貨店碼；進貨店碼；單據日期；登入者
                        break;
                    default:
                        break;
                }

                string strDateTime = PubUtility.SqlQry("select replace(Convert(varchar,getdate(),121),'-','/')", uu, "SYS").Rows[0][0].ToString();
                string sql = "Insert into ISAMToFTPRecWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,DocType,WhNo,UpLoadComd) ";
                sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "','" + strDateTime.Split(" ")[0] + "','" + strDateTime.Split(" ")[1].Substring(0, 8) + "',";
                sql += "'" + uu.UserID + "','" + strDateTime.Split(" ")[0] + "','" + strDateTime.Split(" ")[1].Substring(0, 8) + "',";
                sql += "'" + Doctype +"','" + Shop +"','" + Comd + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "Select * from ISAMToFTPRecWeb (nolock) where Companycode='"+ uu.CompanyId +"' and CrtUser='"+ uu.UserID +"' and CrtDate='"+ strDateTime.Split(" ")[0] + "' and CrtTime='"+ strDateTime.Split(" ")[1].Substring(0, 8) + "' ";
                sql += "and DocType='"+ Doctype +"' and WhNo='"+ Shop +"'";
                DataTable dtFTPRec= PubUtility.SqlQry (sql, uu, "SYS");

                dtFTPRec.TableName = "dtRec";
                ds.Tables.Add(dtFTPRec);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveCollectWeb")]
        public ActionResult SystemSetup_ChkSaveCollectWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveCollectWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["Barcode"];

                string sql = "Select Sum(Qty) SumQty from CollectWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and Whno='" + Shop + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtC";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveCollectWeb")]
        public ActionResult SystemSetup_SaveCollectWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveCollectWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string Shop = rq["Shop"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select * from CollectWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; }
                else
                {
                    DataRow[] dr = dtC.Select("PLU='" + PLU + "'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update CollectWeb set Qty=Qty+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12) ";
                        sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into CollectWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,Whno,WorkUser,SeqNo,PLU,QTY) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + Shop + "','" + uu.UserID + "',";
                    sql += "(select isnull(max(seqno),0)+1 from CollectWeb (nolock) where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'),";
                    sql += "'" + PLU + "', " + Qty.ToString();
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //單品數
                sql = "Select Sum(Qty) SQ1 from CollectWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and Whno='" + Shop + "' and PLU='" + PLU + "'";
                DataTable dtSQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);

                sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCollectWebMod")]
        public ActionResult SystemSetup_GetCollectWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCollectWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string Comd = "";

                if (rq.Count > 1)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from CollectWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'" + Comd + ";";
                sql += "select a.PLU,a.Qty,a.PLU + ' ' + case when isnull(a.GD_Name,'')='' then isnull(b.GD_Name,'') else isnull(a.GD_Name,'') end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo desc";


                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtCollect";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM02PLUMod")]
        public ActionResult SystemSetup_SaveISAM02PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM02PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update CollectWeb set Qty=" + Qty.SqlQuote();
                sql += " where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,Qty,PLU+' '+GD_Name GD_Name from CollectWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,Qty,PLU+' '+isnull(GD_Name,'') GD_Name from CollectWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtCollectMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DelISAM02PLU")]
        public ActionResult SystemSetup_DelISAM02PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM02PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["PLU"];

                string sql = "Delete from CollectWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,Whno,WorkUser,seqno) NewSeq,* into #tmpA from CollectWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join CollectWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.Whno=b.Whno and a.WorkUser=b.WorkUser and a.SeqNo=b.SeqNo;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/DelISAMData")]
        public ActionResult SystemSetup_DelISAMData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAMDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string WorkType = rq["Type"];
                string[] DocType = WorkType.Split(",");
                string sql = "set nocount on;";

                for (int i = 0; i < DocType.Length; i++)
                {
                    switch (DocType[i])
                    {
                        case "T":
                            sql += "select * into #tmpT from binweb where companycode='" + uu.CompanyId + "' and binstore='" + Shop + "';";
                            sql += "insert into binweb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpT;";
                            sql += "delete a from binweb a inner join #tmpT b on a.companycode=b.companycode and a.binstore=b.binstore and a.binno=b.binno and a.binman=b.binman and a.isamdate = b.isamdate and a.seqno = b.seqno;";
                            sql += "drop table #tmpT;";
                            break;
                        case "C":
                            sql += "select * into #tmpC from CollectWeb where companycode='" + uu.CompanyId + "' and Whno='" + Shop + "';";
                            sql += "insert into CollectWeb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpC;";
                            sql += "delete a from CollectWeb a inner join #tmpC b on a.companycode=b.companycode and a.Whno=b.Whno and a.WorkUser=b.WorkUser and a.seqno=b.seqno and a.PLU=b.PLU;";
                            sql += "drop table #tmpC;";
                            break;
                        case "D":
                            sql += "select * into #tmpD from Deliveryweb where companycode='" + uu.CompanyId + "' and WhnoOut='" + Shop + "';";
                            sql += "insert into Deliveryweb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpD;";
                            sql += "delete a from Deliveryweb a inner join #tmpD b on a.companycode=b.companycode and a.WhnoOut=b.WhnoOut and a.DocDate=b.DocDate and a.OutUser=b.OutUser and a.WhnoIn=b.WhnoIn and a.seqno=b.seqno and a.PLU=b.PLU;";
                            sql += "drop table #tmpD;";
                            break;
                        default:
                            break;
                    }
                }

                if (sql != "")
                {
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitISAM03")]
        public ActionResult SystemSetup_GetInitISAM03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAM01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select a.Man_ID,left(a.Man_ID+'      ',6)+' '+Man_Name ManName ,a.Whno,left(a.Whno+'      ',6)+' '+ST_SName STName from ISAMShopWeb a (nolock) ";
                sql += "inner join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.Man_ID=b.Man_ID ";
                sql += "inner join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtWh";
                ds.Tables.Add(dtC);


                sql = "select ST_ID ,ST_ID+' '+ST_SName STName ";
                sql += " from WarehouseWeb (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' and ST_StopDay=''";
                sql += " Order By ST_ID ";

                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtShop.TableName = "dtShop";
                ds.Tables.Add(dtShop);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchDeliveryWeb")]
        public ActionResult SystemSetup_SearchDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];

                string sql = "select top 1 * from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0)
                {
                    sql = "select top 1 * from DeliveryWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtDeliveryData";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveDeliveryWeb")]
        public ActionResult SystemSetup_ChkSaveDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string WhNoIn = rq["WhNoIn"];
                string DocDate = rq["DocDate"];
                string PLU = rq["Barcode"];

                string sql = "Select Sum(OutNum) SumQty from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtD = PubUtility.SqlQry(sql, uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveDeliveryWeb")]
        public ActionResult SystemSetup_SaveDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtPLU.Rows.Count == 0)
                {
                    sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                }

                if (dtPLU.Rows.Count == 0)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = "無符合之商品資料!";
                    return PubUtility.DatasetXML(ds);
                }

                sql = "select * from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; }
                else
                {
                    DataRow[] dr = dtC.Select("PLU='" + PLU + "'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update DeliveryWeb set OutNum=OutNum+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12)";
                        sql += " where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "'";
                        sql += " and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into DeliveryWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,WhNoOut, DocDate, OutUser,WhNoIn, SeqNo, PLU, OutNum, FTPUpDate) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + WhNoOut + "','" + DocDate + "','" + uu.UserID + "','" + WhNoIn + "',";
                    sql += "(select isnull(max(seqno),0)+1 from DeliveryWeb (nolock) where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'),";
                    sql += "'" + PLU + "'," + Qty +" ,''";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //單品數
                sql = "Select Sum(OutNum) SQ1 from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtSQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);
                //門市總和(不看商品)
                sql = "Select Sum(OutNum) SWQ1 from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtSWQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSWQ.TableName = "dtSWQ";
                ds.Tables.Add(dtSWQ);
                sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetDeliveryWebMod")]
        public ActionResult SystemSetup_GetDeliveryWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetDeliveryWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string Comd = "";

                if (rq.Count > 3)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from DeliveryWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'" + Comd + ";";
                sql += "select a.PLU,a.OutNum,a.PLU+' '+case when isnull(a.GD_Name,'')='' then b.GD_Name else a.GD_Name end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo;";
                sql += "drop table #tmpA;";

                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtDelivery";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DelISAM03PLU")]
        public ActionResult SystemSetup_DelISAM03PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM03PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["PLU"];

                string sql = "Delete from DeliveryWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,WhNoOut,WhNoIn,DocDate,OutUser,seqno) NewSeq,* into #tmpA from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join DeliveryWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNoOut and a.WhNoIn=b.WhNoIn and a.DocDate=b.DocDate and a.SeqNo=b.SeqNo and a.OutUser=b.OutUser;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM03PLUMod")]
        public ActionResult SystemSetup_SaveISAM03PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM03PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update DeliveryWeb set OutNum=" + Qty.SqlQuote();
                sql += " where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,OutNum,PLU+' '+GD_Name GD_Name from DeliveryWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,OutNum,PLU+' '+isnull(GD_Name,'') GD_Name from DeliveryWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtDeliveryMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetISAMQFTPRECData")]
        public ActionResult SystemSetup_GetInitISAMQFTPREC()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetISAMQFTPRECDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["WhNo"];
                string Edit = rq["EditMode"];
                string Comd = "";
                string DateS = "";
                string DateE = "";
                string DocT = "";
                string sql = "";

                if (Edit == "Init")
                {
                    String[,] DocType = { {"T","盤點" },{ "C", "條碼蒐集" },{ "D", "出貨/調撥" } };
                    sql = "select Convert(varchar(1),'') DocType,Convert(nvarchar(10),'') DocTypeDesc where 1=0";
                    DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
                    for (int i = 0; i <= DocType.GetUpperBound(0); i++)
                    {
                        DataRow dr = dtDoc.NewRow();
                        dr["DocType"] = DocType[i, 0];
                        dr["DocTypeDesc"] = DocType[i, 1];
                        dtDoc.Rows.Add(dr);
                    }
                    
                    dtDoc.TableName = "dtDoc";
                    ds.Tables.Add(dtDoc);

                    sql = "select Convert(varchar,Dateadd(d,-1,getdate()),111) DTS,Convert(varchar,getdate(),111) DTE ";
                    DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                    dtC.TableName = "dtQDate";
                    ds.Tables.Add(dtC);

                    DateS = dtC.Rows[0][0].ToString();
                    DateE = dtC.Rows[0][1].ToString();
                }
                else
                {
                    DateS = rq["DateS"];
                    DateE = rq["DateE"];
                    DocT = rq["DocType"];
                }
                
                if (DateS!="" && DateE != "")
                {
                    Comd = "and a.CrtDate between '" + DateS + "' and '" + DateE + "' ";
                }
                if (DocT != "") { Comd += "and DocType='"+ DocT + "' "; }

                sql = "select case DocType when 'T' then N'盤點' when 'C' then N'條碼蒐集' when 'D' then N'出貨調撥' end DocTypeDesc ,a.CrtDate,a.CrtDate+' '+a.CrtTime CrtDT,case when isnull(Man_Name,'')='' then a.CrtUser else b.Man_Name end CrtUserName,";
                sql += "case UpdateType when 'N' then N'未上傳' when 'Y' then N'成功' when 'E' then N'異常' end UpdateTypeDesc,a.CrtUser,DocType from ISAMTOFTPRecWeb a (nolock) ";
                sql += "left join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.CrtUser=b.Man_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Whno='" + Shop + "' "+ Comd +"order by a.CompanyCode,a.CrtDate desc,a.CrtTime desc";
                DataTable dtQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtQ.TableName = "dtQRec";
                ds.Tables.Add(dtQ);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchISAMFTPRecDetl")]
        public ActionResult SystemSetup_SearchISAMFTPRecDetl()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchISAMFTPRecDetlOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CrtUser = rq["CrtUser"];
                string CrtDT = rq["CrtDT"];
                string CrtDate = CrtDT.Split(" ")[0].Trim();
                string CrtTime = CrtDT.Split(" ")[1].Trim();
                string DocType = rq["DocType"];
                string Shop = rq["Shop"];
                
                string sql = "select case DocType when 'T' then N'盤點' when 'C' then N'條碼蒐集' when 'D' then N'出貨調撥' end DocTypeDesc ,a.CrtDate+' '+a.CrtTime CrtDT,case when isnull(Man_Name,'')='' then a.CrtUser else b.Man_Name end CrtUserName,";
                sql += "case UpdateType when 'N' then N'未上傳' when 'Y' then N'成功' when 'E' then N'異常' end UpdateTypeDesc,FTPDate,UpLoadComd,DocType from ISAMTOFTPRecWeb a (nolock) ";
                sql += "left join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.CrtUser=b.Man_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.CrtUser='"+ CrtUser + "' and a.CrtDate='" + CrtDate + "' and a.Crttime='" + CrtTime + "' and DocType='" + DocType + "' and a.Whno='" + Shop + "' ";
                DataTable dtQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtQ.TableName = "dtRec";
                ds.Tables.Add(dtQ);

                if(DocType=="D" && dtQ.Rows[0]["UpLoadComd"].ToString().Split(";").Length>2)
                {
                    sql = "select left(ST_ID+'      ',6)+' '+ST_SName InSTName from WarehouseWeb c (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and ST_ID='" + dtQ.Rows[0]["UpLoadComd"].ToString().Split(";")[1] + "'";
                    DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                    dtWh.TableName = "dtWh";
                    ds.Tables.Add(dtWh);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/SearchVIV10")]
        public ActionResult SystemSetup_SearchVIV10()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIV10OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string YM = rq["YM"];
                string WhNo = rq["WhNo"];

                string sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name ModUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_No As Integer) DiffQty ";
                sql += " from InvDistribute a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.ModUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and a.ShopNo='" + WhNo + "'";
                }
                //if (CkNo != "")
                //{
                //    sql += " and b.CkNo='" + CkNo + "'";
                //}
                sql += " Order By a.INV_YM,a.ShopNo ";
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


        [Route("SystemSetup/GetVIV10Detail")]
        public ActionResult SystemSetup_GetVIV10Detail()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIV10DetailOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string CompanyCode = rq["CompanyCode"];
                string ShopNo = rq["ShopNo"];
                string sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + ShopNo.SqlQuote() + "'";
                sql += " Order By CkNo ";

                DataTable dtCk = PubUtility.SqlQry(sql, uu, "SYS");
                dtCk.TableName = "CkList";
                ds.Tables.Add(dtCk);

                //sql = "select LayerNo,ChannelType,max(ChannelNo) as ChannelNo  from MachineListSpec a";
                //sql += " where a.CompanyCode='" + CompanyCode.SqlQuote() + "'";
                //sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                //sql += " group by LayerNo,ChannelType";
                //sql += " order by LayerNo";
                //DataTable MachineListSpec = PubUtility.SqlQry(sql, uu, "SYS");
                //MachineListSpec.TableName = "MachineListSpec";
                //ds.Tables.Add(MachineListSpec);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetVIV10View")]
        public ActionResult SystemSetup_GetVIV10View()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIV10ViewOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];
                string YM = rq["YM"];
                string sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name AppUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_SNo As Integer)+1 DiffQty ";
                sql += " from InvDistributeSV a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.AppUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (ShopNo != "")
                {
                    sql += " and a.ShopNo='" + ShopNo + "'";
                }
                //sql += " and a.SNno='" + SNno.SqlQuote() + "'";

                DataTable dtDtl = PubUtility.SqlQry(sql, uu, "SYS");
                dtDtl.TableName = "dtDtl";
                ds.Tables.Add(dtDtl);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/SaveVIV10")]
        public ActionResult SaveVIV10()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVIV10OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                UserInfo uu = PubUtility.GetCurrentUser(this);
                DataTable dtH = new DataTable("ShopData");
                PubUtility.AddStringColumns(dtH, "ShopNo,INV_YM,INV_Head,INV_No");
                //dtH.Columns.Add("Temperature", typeof(double));

                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtH);

                DataTable dtD = new DataTable("AssignQty");
                PubUtility.AddStringColumns(dtD, "ShopNo,CkNo");
                dtD.Columns.Add("AssignQty", typeof(double));
                dsRQ.Tables.Add(dtD);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);

                //dtH.Rows[0]["CompanyCode"] = uu.CompanyId;
                //DataTable dtS = new DataTable();
                //PubUtility.AddStringColumns(dtS, "CompanyCode,SNno,LayerNo,ChannelNo,ChannelType");

                //foreach (DataRow dr in dtD.Rows)
                //{
                //    dr["CompanyCode"] = uu.CompanyId;
                //    int ch = 1;
                //    for (int i = 0; i < PubUtility.CB(dr["ChannelQty"]); i++)
                //    {
                //        DataRow drS = dtS.NewRow();
                //        dtS.Rows.Add(drS);
                //        List<string> fds = "CompanyCode,SNno,LayerNo,ChannelType".Split(',').ToList();
                //        foreach (string fd in fds)
                //        {
                //            drS[fd] = dr[fd];
                //        }
                //        drS["ChannelNo"] = ch.ToString().PadLeft(2, '0');
                //        ch++;
                //    }
                //}
                //string EditMode = dtH.Rows[0]["EditMode"].ToString();
                //string CompanyCode = dtH.Rows[0]["CompanyCode"].ToString();
                //string SNno = dtH.Rows[0]["SNno"].ToString();
                //dsRQ.Tables.Add(dtS);
                string sql;
                string ShopNo = ""; string INV_YM = ""; string INV_Head = ""; string INV_No = "";
                string CkNo = "";
                ShopNo = dtH.Rows[0]["ShopNo"].ToString();
                INV_YM = dtH.Rows[0]["INV_YM"].ToString();
                INV_Head = dtH.Rows[0]["INV_Head"].ToString();
                INV_No = dtH.Rows[0]["INV_No"].ToString();

                string DocNo = PubUtility.GetNewDocNo(uu, "ID", 4);

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            Int32 SNo = 0;
                            string SNoSql = "";
                            Int32 ENo = 0;
                            string ENoSql = "";
                            SNo = Convert.ToInt32(INV_No) + 1;
                            Int32 iCount = 0;

                            string DtlDocNo = DocNo;
                            foreach (DataRow dr in dtD.Rows)
                            {

                                if (Convert.ToInt32(dr["AssignQty"].ToString()) != 0)
                                {

                                    ENo = SNo + Convert.ToInt32(dr["AssignQty"].ToString()) - 1;

                                    SNoSql = "Right(Cast(REPLICATE('0', 8) As varChar(8))+Cast(" + SNo + " as varchar(8)),8)";
                                    ENoSql = "Right(Cast(REPLICATE('0', 8) As varChar(8))+Cast(" + ENo + " as varchar(8)),8)";

                                    CkNo = dr["CkNo"].ToString().SqlQuote();

                                    sql = "Insert Into InvDistributeSV (CompanyCode, CrtUser, CrtDate, CrtTime ";
                                    sql += ", ModUser, ModDate, ModTime ";
                                    sql += ", DocNo, ShopNo, INV_YM ";
                                    sql += ", INV_Head, INV_SNo, INV_ENo "
                                        + ", AppDate, AppUser, CkNo) Values ";
                                    sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                                    sql += ", '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                                    sql += ", '" + DtlDocNo + "', '" + ShopNo + "', '" + INV_YM + "' ";
                                    sql += ", '" + INV_Head + "'," + SNoSql + ", " + ENoSql + ""
                                        + ", convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(12),getdate(),108),1,5), '" + uu.UserID + "','" + CkNo + "')";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //string sno = DtlDocNo.Substring(10, 4);
                                    Int32 NewSno = Convert.ToInt32(DtlDocNo.Substring(10, 4)) + 1;

                                    DtlDocNo = PubUtility.StrLeft(DtlDocNo, 10) + PubUtility.PadLeft(Convert.ToString(NewSno), '0', 4);
                                    SNo += Convert.ToInt32(dr["AssignQty"].ToString());

                                    iCount += 1;

                                }



                            }

                            sql = "Update InvDistribute Set Inv_No=" + ENoSql + "";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And ShopNo='" + ShopNo + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            sql = "Update DocumentNo Set SeqNo= SeqNo + " + (iCount - 1) + " ";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And Initial='ID' And DocDate=convert(char(8),getdate(),112)";
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



                sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name ModUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_No As Integer) DiffQty ";
                sql += " from InvDistribute a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.ModUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " And a.ShopNo='" + ShopNo + "' ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
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
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
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


        //2021-06-21
        [Route("SystemSetup/GetWhCkLayerSno")]
        public ActionResult SystemSetup_GetWhCkLayerSno()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerSnoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string LayerNo = rq["LayerNo"];
                string sql = "select Distinct a.ChannelNo ";
                sql += " from MachineListSpec a (NoLock) ";
                sql += " Inner Join WarehouseDSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and b.ST_ID='" + WhNo + "' And b.CkNo='" + CkNo + "' And a.LayerNo='" + LayerNo + "' ";
                sql += " Order By a.ChannelNo ";
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


        //2021-06-21
        [Route("SystemSetup/GetWhCkLayerSnoPLU")]
        public ActionResult SystemSetup_GetWhCkLayerSnoPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerSnoPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string LayerNo = rq["LayerNo"];
                string Sno = rq["Sno"];
                string sql = "select a.PLU, b.GD_SName ";
                sql += " from InventorySV a (NoLock) ";
                sql += " Inner Join PLUSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and a.WhNo='" + WhNo + "' And a.CkNo='" + CkNo + "' And a.Layer='" + LayerNo + "' And a.SNo='" + Sno + "' ";
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


        [Route("SystemSetup/GetQueryDays")]
        public ActionResult SystemSetup_GetQueryDays()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetQueryDaysOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select QueryDays from CompanyPIDSV where Companycode='" + uu.CompanyId + "' and Programid='" + ProgramID.SqlQuote() + "'";

                DataTable dtQueryDays = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtQueryDays.Rows.Count == 0)
                    sql = "select QueryDays from ProgramID where Programid='" + ProgramID.SqlQuote() + "'";
                dtQueryDays = PubUtility.SqlQry(sql, uu, "SYS");

                dtQueryDays.TableName = "dtQueryDays";
                ds.Tables.Add(dtQueryDays);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitVIN14_1P")]
        public ActionResult GetInitVIN14_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVIN14_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "' ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtShop);
                dtShop.TableName = "dtShop";

                sql = "Select Type_ID,Type_Name from TypeData (nolock) where Companycode='" + uu.CompanyId + "' and Type_Code='DA' ";
                DataTable dtArea = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtArea);
                dtArea.TableName = "dtArea";

                sql = "Select DocNo from PickupHSV (nolock) where Companycode='" + uu.CompanyId + "' and DocType='S' " +
                      "and WhNoOut=(select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') " +
                      "and isnull(ChkDate,'')<>'' ";
                DataTable dtPick = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtPick);
                dtPick.TableName = "dtPick";




            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-14 Kris
        [Route("SystemSetup/GetVIN14_1P")]
        public ActionResult SystemSetup_GetVIN14_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Area = rq["Area"];
                string Pick = rq["Pick"];
                string DocDate = rq["DocDate"];

                string sql = "Select a.DocNo,a.DocDate,sum(b.Qty)Qty From PickupHSV a (nolock) " +
                    "left join PickupDSV b (nolock) on a.DocNo=b.DocNo and b.companycode=a.companycode " +
                    "left join WarehouseSV c (nolock) on a.WhNoOut=c.ST_ID and c.companycode=a.companycode " +
                    "Where a.Companycode='" + uu.CompanyId + "' " +
                    "and a.WhNoOut=(select WhNo from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') " +
                    "and isnull(a.ChkDate,'')<>'' and a.DocType='S' ";
                if (Area != "")
                    sql += "and c.ST_DeliArea='" + Area + "' ";
                if (Pick != "")
                    sql += "and a.DocNo='" + Pick + "' ";
                if (DocDate != "")
                    sql += "and a.DocDate='" + DocDate + "' ";
                sql += "group by a.DocNo,a.DocDate ";
                sql += "order by a.DocNo ";

                DataTable dtVIN14_1P = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P.TableName = "dtVIN14_1P";
                ds.Tables.Add(dtVIN14_1P);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_1")]
        public ActionResult SystemSetup_GetVIN14_1P_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "Select a.SeqNo,a.PLU + ' ' + b.GD_Sname as GD_NO,isnull(a.Qty,0)Qty from PickupDSV a (nolock) " +
                "left join PLUSV b(nolock) on a.PLU = b.GD_NO and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by a.seqno ";
                DataTable dtVIN14_1P_1 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_1.TableName = "dtVIN14_1P_1";
                ds.Tables.Add(dtVIN14_1P_1);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_2")]
        public ActionResult SystemSetup_GetVIN14_1P_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "Select a.WhNo,a.Ckno,b.ST_Sname+a.Ckno+'機' as SName,c.ST_Address,b.ST_DeliArea " +
                "from PickupShopSV a(nolock) " +
                "left join WarehouseSV b(nolock) on a.WhNo = b.ST_ID and b.CompanyCode = a.CompanyCode " +
                "left join WarehouseDSV c(nolock) on a.WhNo = c.ST_ID and a.Ckno = c.CkNo and c.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by a.whno,a.ckno ";
                DataTable dtVIN14_1P_2 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_2.TableName = "dtVIN14_1P_2";
                ds.Tables.Add(dtVIN14_1P_2);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_3")]
        public ActionResult SystemSetup_GetVIN14_1P_3()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_3OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "select b.SeqNo,b.PLU + ' ' + c.GD_Sname as PLU,isnull(b.Qty,0)Qty, " +
                "a.whnoin+'店 '+a.cknoin+'機 '+d.st_sname+a.cknoin as Name " +
                "from PickupHSV a (nolock) " +
                "left join PickupDSV b (nolock) on a.DocNo=b.DocNo and b.CompanyCode=a.CompanyCode " +
                "left join PLUSV c(nolock) on b.PLU = c.GD_NO and c.CompanyCode = a.CompanyCode " +
                "left join WarehouseSV d (nolock) on a.whnoin=d.st_id and d.companycode=a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by b.seqno ";
                DataTable dtVIN14_1P_3 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_3.TableName = "dtVIN14_1P_3";
                ds.Tables.Add(dtVIN14_1P_3);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-16 Kris
        [Route("SystemSetup/GetInitVMN02")]
        public ActionResult GetInitVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_ID+ST_Sname as ST_Sname from WarehouseSV (nolock) " +
                    "where Companycode='" + uu.CompanyId + "' and ST_Type='6' " +
                    "and WhnoIn=(select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') ";
                DataTable dtWarehouse = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtWarehouse);
                dtWarehouse.TableName = "dtWarehouse";

                sql = "Select Type_ID,Type_Name from TypeData (nolock) where Companycode='" + uu.CompanyId + "' and Type_Code='DA' " +
                "order by Type_ID ";
                DataTable dtDeliArea = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDeliArea);
                dtDeliArea.TableName = "dtDeliArea";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetMCSeq")]
        public ActionResult GetMCSeq()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetMCSeqOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];

                string sql = "Select MCSeq from MachineList (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (SNno != "")
                   sql += "and SNno='" + SNno + "' ";
                DataTable dtMCSeq = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtMCSeq);
                dtMCSeq.TableName = "dtMCSeq";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetVMN02")]
        public ActionResult SystemSetup_GetVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "select a.ST_ID,a.ST_Sname, " +
                "(select count(*) from WarehouseDSV b (nolock) where b.CompanyCode='" + uu.CompanyId + "' and a.ST_ID=b.ST_ID)CkNo, " +
                "a.FlagInv,a.WhnoIn,(select ST_ID + ST_SName from WarehouseSV (nolock) where companycode='" + uu.CompanyId + "' and st_id=a.whnoin)WhNoInName, " +
                "a.InvGetQty,a.InvSaveQty,a.ST_DeliArea " +
                "from WarehouseSV a (nolock) " +
                "where a.CompanyCode='" + uu.CompanyId + "' and a.ST_Type='6' ";
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                sql += "order by a.ST_ID ";

                DataTable dtVMN02 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVMN02.TableName = "dtVMN02";
                ds.Tables.Add(dtVMN02);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetCheckSN")]
        public ActionResult GetCheckSN()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckSNOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];

                string sql = "Select * from WarehouseDSV (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (SNno != "")
                    sql += "and SNno='" + SNno + "' ";
                DataTable dtCheckSN = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSN);
                dtCheckSN.TableName = "dtCheckSN";

                sql = "Select * from MachineList (nolock) where Companycode='" + uu.CompanyId + "' ";
                sql += "and FlagUse='N' ";
                if (SNno != "")
                    sql += "and SNno='" + SNno + "' ";
                DataTable dtCheckMachine = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckMachine);
                dtCheckMachine.TableName = "dtCheckMachine";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetInitMod")]
        public ActionResult GetInitMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "Select CkNo from WarehouseDSV (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (ShopNo != "")
                    sql += "and ST_ID='" + ShopNo + "' ";
                sql += "order by CkNo ";
                DataTable dtInitMod = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtInitMod);
                dtInitMod.TableName = "dtInitMod";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetVMN02_Mod")]
        public ActionResult GetVMN02_Mod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02_ModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];

                string sql = "select CkNo,a.SNno,b.MCSeq,a.ST_Address,a.InvGetQty,a.InvSaveQty, " +
                "isnull(a.ST_OpenDay, '')ST_OpenDay,isnull(a.ST_StopDay, '')ST_StopDay " +
                "from WarehouseDSV a(nolock) " +
                "left join MachineList b(nolock) on a.SNno = b.SNno and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                if (CkNo != "")
                    sql += "and a.CkNo='" + CkNo + "' ";
                DataTable dtVMN02_Mod = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtVMN02_Mod);
                dtVMN02_Mod.TableName = "dtVMN02_Mod";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/GetQuery")]
        public ActionResult SystemSetup_GetQuery()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetQueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "select a.CkNo,a.SNno," +
                "a.WhnoIn + b.ST_Sname as WhName, "+
                "a.ST_Address,a.InvGetQty,a.InvSaveQty,a.ST_OpenDay,a.ST_StopDay,a.QrCode "+
                "from WarehouseDSV a (nolock) "+
                "left join WarehouseSV b (nolock) on a.WhnoIn = b.ST_ID and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode='" + uu.CompanyId + "' "; 
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                sql += "order by a.CkNo ";

                DataTable dtQuery = PubUtility.SqlQry(sql, uu, "SYS");
                dtQuery.TableName = "dtQuery";
                ds.Tables.Add(dtQuery);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/AddVMN02")]
        public ActionResult SystemSetup_AddVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseDSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,CkNo,SNno,ST_Address,InvGetQty,InvSaveQty,ST_OpenDay,QrCode");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            dbop.Add("WarehouseDSV", dtWh, uu, "SYS");

                            sql = "Update MachineList set FlagUse='S',ModDate=convert(char(10),getdate(),111), " +
                                  "ModTime=convert(char(12),getdate(),108), " +
                                  "ModUser='" + uu.UserID + "' " +
                                  "where Companycode='" + uu.CompanyId + "' " +
                                  "and SNno='" + dr["SNno"].ToString().SqlQuote() + "' ";
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

                sql = "select a.* ";
                sql += "from WarehouseDSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                sql += "and a.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseDSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseDSV.TableName = "dtWarehouseDSV";
                ds.Tables.Add(dtWarehouseDSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/UpdateVMN02")]
        public ActionResult SystemSetup_UpdateVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseDSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,CkNo,ST_Address,InvGetQty,InvSaveQty,ST_OpenDay,ST_StopDay");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "update WarehouseDSV set ";
                            sql += "ST_Address='" + dr["ST_Address"].ToString().SqlQuote() + "',";
                            sql += "InvGetQty=" + dr["InvGetQty"].ToString().SqlQuote() + ",";
                            sql += "InvSaveQty=" + dr["InvSaveQty"].ToString().SqlQuote() + ",";
                            sql += "ST_OpenDay='" + dr["ST_OpenDay"].ToString().SqlQuote() + "',";
                            sql += "ST_StopDay='" + dr["ST_StopDay"].ToString().SqlQuote() + "',";
                            sql += "ModDate=convert(char(10),getdate(),111),";
                            sql += "ModTime=convert(char(12),getdate(),108),";
                            sql += "ModUser='" + uu.UserID + "' ";
                            sql += "where CompanyCode='" + uu.CompanyId + "' ";
                            sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                            sql += "and CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
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
                sql = "select a.* ";
                sql += "from WarehouseDSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                sql += "and a.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseDSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseDSV.TableName = "dtWarehouseDSV";
                ds.Tables.Add(dtWarehouseDSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/GetCheckDoc")]
        public ActionResult GetCheckDoc()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckDocOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ST_ID = rq["ST_ID"];
                string SysDate = rq["SysDate"];

                string sql = "select * from TransferHSV (nolock) " +
                             "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtTransfer = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtTransfer);
                dtTransfer.TableName = "dtTransfer";

                sql = "select * from UselessHSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtUseless = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtUseless);
                dtUseless.TableName = "dtUseless";

                sql = "select * from AdjustHSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNo='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtAdjust = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtAdjust);
                dtAdjust.TableName = "dtAdjust";

                sql = "select * from ChangePLUSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNo='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and ExchangeDate>='" + SysDate + "' ";
                sql += "and isnull(AppDate,'')<>'' and isnull(FinishDate,'')='' ";
                sql += "and isnull(DefeasanceDate,'')='' ";
                DataTable dtChangePLU = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtChangePLU);
                dtChangePLU.TableName = "dtChangePLU";

                sql = "select * from ChangeShopSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and ExchangeDate>='" + SysDate + "' ";
                sql += "and isnull(AppDate,'')<>'' and isnull(FinishDate,'')='' ";
                sql += "and isnull(DefeasanceDate,'')='' ";
                DataTable dtChangeShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtChangeShop);
                dtChangeShop.TableName = "dtChangeShop";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/GetCheckSaveH")]
        public ActionResult GetCheckSaveH()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckSaveHOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoIn = rq["WhNoIn"];
                string ST_ID = rq["ST_ID"];

                string sql = "select * from WarehouseSV (nolock) " +
                             "where CompanyCode='" + uu.CompanyId + "' ";
                if (WhNoIn != "")
                    sql += "and ST_ID='" + WhNoIn + "' ";
                DataTable dtCheckSaveH = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSaveH);
                dtCheckSaveH.TableName = "dtCheckSaveH";

                sql = "select * from WarehouseDSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and ST_ID='" + ST_ID + "' ";
                DataTable dtCheckSaveD = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSaveD);
                dtCheckSaveD.TableName = "dtCheckSaveD";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/UpdateVMN02H_1")]
        public ActionResult SystemSetup_UpdateVMN02H_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVMN02H_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,FlagInv,WhNoIn,OldFlagInv,ST_DeliArea");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["FlagInv"].ToString() == dr["OldFlagInv"].ToString())
                            {
                                if (dr["FlagInv"].ToString() == "Y")
                                {
                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["ST_ID"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo<>'00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                                else if (dr["FlagInv"].ToString() == "N")
                                {
                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                            }
                            else
                            {
                                if (dr["OldFlagInv"].ToString() == "N" && dr["FlagInv"].ToString() == "Y")
                                {
                                    sql = "Insert Into WarehouseDSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,ST_ID,CkNo,WhNoIn)";
                                    sql += " Values ";
                                    sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108) ";
                                    sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108) ";
                                    sql += " ,'" + dr["ST_ID"].ToString().SqlQuote() + "','00','" + dr["WhNoIn"].ToString().SqlQuote() + "') ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["ST_ID"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo<>'00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                                else if(dr["OldFlagInv"].ToString() == "Y" && dr["FlagInv"].ToString() == "N")
                                {
                                    //調撥表頭
                                    string TH_ID = PubUtility.GetNewDocNo(uu, "TH", 6);
                                    sql = "Insert Into TransferHSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,TH_ID,DocDate,WhNoOut,OutUser,";
                                    sql += "InDate,WhNoIn,InUser,DocType,CkNoOut,CkNoIn,WorkType,";
                                    sql += "ChkUser,ChkDate,PostUser,PostDate)";

                                    sql += " Values ";
                                    sql += "('" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108)";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + TH_ID + "',convert(char(10),getdate(),111),'" + dr["ST_ID"].ToString().SqlQuote() + "','" + uu.UserID + "',";
                                    sql += "convert(char(10),getdate(),111),'" + dr["WhNoIn"].ToString().SqlQuote() + "','" + uu.UserID + "',";
                                    sql += "'V','00','XX','IB','" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),";
                                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)) ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //調撥表身
                                    sql = "Insert Into TransferDSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,TH_ID,SeqNo,PLU,OutNum,InNum,";
                                    sql += "GD_Retail,Amt,LayerIn,LayerOut,SnoOut)";

                                    sql += " select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + TH_ID + "',Cast(Row_Number() Over(Order By a.plu) As int),";
                                    sql += "a.plu,a.PTNum,a.PTNum,isnull(b.GD_Retail,0)GD_Retail,a.PTNum*isnull(b.GD_Retail,0),";
                                    sql += "'','Z',(select Sno from InventorySV a1 (nolock) where a1.companycode='" + uu.CompanyId + "' and a.whno=a1.whno and a.ckno=a1.ckno and a.plu=a1.plu and a1.Layer='Z') ";
                                    sql += "from InventorySV a (nolock) ";
                                    sql += "left join plusv b (nolock) on a.plu=b.gd_no and b.CompanyCode=a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and a.whno='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and a.ckno='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //寫入調出方JahoInvSV
                                    sql = "Insert Into JahoInvSV (CompanyCode,CrtUser,CrtDate,CrtTime, ";
                                    sql += "ModUser,ModDate,ModTime, ";
                                    sql += "DocType,DocNo,WhNo,SeqNo,PLU,Q1,Q2,Q3,CkNo,Layer,Sno) ";

                                    sql += "Select '" + uu.CompanyId.SqlQuote() + "'";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                                    sql += ",'TF','" + TH_ID + "',t.WhNoOut,t.SeqNo,t.PLU,IsNull(i.PtNum,0),isnull(t.InNum,0)*-1,IsNull(i.PtNum,0)+isnull(t.InNum,0)*-1";
                                    sql += ",t.CkNoOut,t.LayerOut,t.SnoOut ";

                                    sql += "From (select a.WhNoOut,b.SeqNo,b.PLU,b.InNum,a.CkNoOut,b.LayerOut,b.SnoOut ";
                                    sql += "from TransferHSV a (nolock) ";
                                    sql += "left join TransferDSV b (nolock) on a.TH_ID = b.TH_ID and b.CompanyCode = a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.TH_ID='" + TH_ID + "')t ";

                                    sql += "Left Join InventorySV i (nolock) ";
                                    sql += "On i.CompanyCode='" + uu.CompanyId + "' And t.WhNoOut=i.WhNo and t.CkNoOut=i.CkNo ";
                                    sql += "and t.LayerOut=i.Layer And t.SnoOut=i.Sno And t.PLU=i.PLU ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //調出方庫存
                                    sql = "Update InventorySV set PTNum=PTNum-t.InNum,Out_Date=t.DocDate, ";
                                    sql += "ModUser='" + uu.UserID + "',ModDate=convert(char(10),getdate(),111),ModTime=convert(char(8),getdate(),108) ";

                                    sql += "from(select a.WhNoOut,a.CkNoOut,b.PLU,b.InNum,a.DocDate,b.LayerOut,b.SnoOut from TransferHSV a (nolock) ";
                                    sql += "left join TransferDSV b (nolock) on a.TH_ID = b.TH_ID and b.CompanyCode = a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.TH_ID='" + TH_ID + "')t ";
                                    sql += "left join InventorySV i (nolock) on t.WhNoOut=i.WhNo and t.CkNoOut=i.CkNo and t.PLU=i.PLU and t.LayerOut=i.Layer and t.SnoOut=i.Sno and i.companycode='" + uu.CompanyId + "' ";
                                    sql += "where 1=1 ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                    
                                    //刪除母倉資料
                                    sql = "Delete From WarehouseDSV where Companycode='" + uu.CompanyId + "' " ;
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' and CkNo='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "Update WarehouseDSV set WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "' ";
                                    sql += "where Companycode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }

                            }
                            sql = "update WarehouseSV set ";
                            sql += "FlagInv='" + dr["FlagInv"].ToString().SqlQuote() + "',";
                            sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                            sql += "ST_DeliArea='" + dr["ST_DeliArea"].ToString().SqlQuote() + "',";
                            sql += "ModDate=convert(char(10),getdate(),111),";
                            sql += "ModTime=convert(char(12),getdate(),108),";
                            sql += "ModUser='" + uu.UserID + "' ";
                            sql += "where CompanyCode='" + uu.CompanyId + "' ";
                            sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
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
                sql = "select a.* ";
                sql += "from WarehouseSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseSV.TableName = "dtWarehouseSV";
                ds.Tables.Add(dtWarehouseSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-08-10 Kris
        [Route("SystemSetup/GetVMN02SN_Add")]
        public ActionResult GetVMN02SN_Add()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02SN_AddOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select SNno from MachineList (nolock) where Companycode='" + uu.CompanyId + "' and FlagUse='N' " +
                             "order by SNno ";
                DataTable dtSN_Add = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSN_Add);
                dtSN_Add.TableName = "dtSN_Add";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        //2022-07-27 Larry
        [Route("SystemSetup/GetInitISAMQPLU")]
        public ActionResult GetInitISAMQPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAMQPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select E.whno, W.ST_SName from ISAMShopWeb (nolock) E Left Join WarehouseWeb W On E.CompanyCode=W.CompanyCode And E.WhNo=W.ST_ID " +
                             "where E.companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "' ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtShop);
                dtShop.TableName = "dtShop";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022-07-27 Larry
        [Route("SystemSetup/GetISAMQPLU")]
        public ActionResult SystemSetup_GetISAMQPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetISAMQPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string Area = rq["Area"];
                string WhNo = rq["WhNo"];
                string PLU = rq["PLU"];

                string sql = "Select a.GD_No,a.GD_Barcode,a.GD_Name,a.GD_Retail,IsNull(b.PTNum,0) PTNum From PluWeb a (nolock) " +
                    "left join InventoryAtonceDWeb b (nolock) on a.GD_No=b.PLU and a.companycode=b.companycode And b.WhNo='" + WhNo + "' " +
                    "Where a.Companycode='" + uu.CompanyId + "' ";
                if (PLU != "")
                    sql += "and (a.GD_No='" + PLU + "' Or a.GD_Barcode='" + PLU + "') ";
                sql += "order by a.GD_No ";

                DataTable dtQPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtQPLU.TableName = "dtQPLU";
                ds.Tables.Add(dtQPLU);

                sql = "Select CompanyCode, PP_No, StartDate, EndDate, ApproveDate From PromotePriceHWeb (Nolock) " +
                     "Where CompanyCode='" + uu.CompanyId + "' And WhNoFlag='Y' And IsNull(ApproveDate,'')<>'' And IsNull(DefeasanceDate,'')='' " +
                     "Union " +
                     "Select H.CompanyCode, H.PP_No, H.StartDate, H.EndDate, H.ApproveDate " +
                     "From PromotePriceHWeb H (Nolock) Inner Join PromotePriceShopWeb S (Nolock) On H.CompanyCode=S.CompanyCode And H.PP_No=S.PP_No " +
                     "Where H.CompanyCode='" + uu.CompanyId + "' And WhNoFlag='N' And IsNull(ApproveDate,'')<>'' And IsNull(DefeasanceDate,'')='' And S.WhNo='" + WhNo + "'";

                //取得促銷資料
                if (dtQPLU.Rows.Count > 0)

                {
                    sql = "Select H.*, D.Promote From (" +
                       sql + ") H " +
                       "Inner Join PromotePriceDWeb D (Nolock) " +
                       "On H.CompanyCode=D.CompanyCode And H.PP_No=D.PP_No " +
                       "Where D.PLU='" + dtQPLU.Rows[0]["GD_No"] + "'" +
                       "Order By ApproveDate Desc ";
                }

                else
                {
                    sql = "Select H.CompanyCode, H.PP_No, H.StartDate, H.EndDate, H.ApproveDate, D.Promote " +
                        "From PromotePriceHWeb H (Nolock) Inner Join PromotePriceDWeb D (Nolock) On H.CompanyCode=D.CompanyCode And H.PP_No=D.PP_No Where 1=2";
                }

                DataTable dtPrm = PubUtility.SqlQry(sql, uu, "SYS");
                dtPrm.TableName = "dtPrm";
                ds.Tables.Add(dtPrm);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/GetTreeData")]
        public ActionResult SystemSetup_GetTreeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetTreeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string isFormLoad = rq["FormLoad"];
                string lvtype = "";
                switch (LV)
                {
                    case "1":
                        lvtype = "3";
                        break;
                    case "2":
                        lvtype = "2";
                        break;
                    case "3":
                        lvtype = "1";
                        break;
                    default:
                        break;
                }
                //Group+Man
                string sql = "select 'G_'+GroupID [id],'#' parent,GroupName [text] from GroupIDSV where CompanyCode='" + uu.CompanyId + "' and [status]='" + LV.SqlQuote() + "' order by GroupID ";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                sql = "Select 'E_'+Man_ID [id],Man_Name [text],'G_'+Man_Group parent from EmployeeSV where CompanyCode='" + uu.CompanyId + "' and [Level]='" + LV.SqlQuote() + "' order by Man_Group,Man_ID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtEmp";
                ds.Tables.Add(dtE);

                //SystemID
                sql = "select 'S_'+a.SystemId id,'#' parent,a.ChineseName [text] from CompanySIDSV a inner join CompanyPIDSV b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.SystemId=b.SystemID ";
                sql += "inner join SystemId c on a.SystemId=c.SystemId ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and b.Flag" + lvtype + "='Y' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "group by a.SystemId,a.ChineseName,c.OrderSequence Order By c.OrderSequence ";

                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtSystemID";
                ds.Tables.Add(dtS);

                //ProgramID
                sql = "select 'P_'+b.ProgramId id,'S_'+b.SystemID parent,b.ChineseName text from CompanySIDSV a inner join CompanyPIDSV b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.SystemId=b.SystemID ";
                sql += "inner join SystemId c on a.SystemId=c.SystemId ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and b.Flag" + lvtype + "='Y' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "Order By c.OrderSequence,b.OrderSequence ";

                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                dtP.TableName = "dtProgramID";
                ds.Tables.Add(dtP);

                //GroupProgramIDSV
                string GID = "";
                if (dtG.Rows.Count > 0)
                {
                    GID = dtG.Rows[0]["id"].ToString().Substring(2, dtG.Rows[0]["id"].ToString().Length - 2);
                }

                sql = "select 'P_'+ProgramId id from GroupProgramIDSV ";
                sql += "Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + GID.SqlQuote() + "' ";
                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);


                //Lavel
                if (isFormLoad == "Y")
                {
                    sql = "select CONVERT(varchar(1),'') LV,CONVERT(nvarchar(10),N'') LName";
                    DataTable dtL = PubUtility.SqlQry(sql, uu, "SYS");
                    for (int i = 0; i < 3; i++)
                    {
                        DataRow dr = dtL.NewRow();
                        dr["LV"] = (i + 1).ToString();
                        string lname = "";
                        switch (i)
                        {
                            case 0:
                                lname = "智販店";
                                break;
                            case 1:
                                lname = "分公司";
                                break;
                            case 2:
                                lname = "總公司";
                                break;
                            default:
                                break;
                        }
                        dr["LName"] = lname;
                        dtL.Rows.Add(dr);
                    }
                    dtL = dtL.Select("", "LV desc").CopyToDataTable();
                    dtL.TableName = "dtLV";
                    ds.Tables.Add(dtL);
                }



            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetGroupEmp")]
        public ActionResult SystemSetup_GetGroupEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGroupEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string Hbtn = rq["Hbtn"];
                string Emptype = rq["EmpLoadType"];
                string ManGroup = rq["ManGroup"];

                //Group
                string sql = "select 'DG_'+GroupID [id],'#' parent,GroupName [text] from GroupIDSV where CompanyCode='" + uu.CompanyId + "' and [status]='" + LV.SqlQuote() + "' order by GroupID ";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                if (Hbtn != "G")
                {
                    string ls_Cond = "";
                    switch (Emptype)    //A-加入群組人員;D-刪除群組人員
                    {
                        case "A":
                            ls_Cond = " and NorMal='1' and isnull(Man_Group,'')='' ";
                            break;
                        case "D":
                            if (ManGroup != null)
                            {
                                ls_Cond = " and isnull(Man_Group,'')='" + ManGroup.SqlQuote() + "' ";
                            }
                            else
                            {
                                ls_Cond = " and 1=0 ";
                            }
                            break;
                        default:
                            ls_Cond = " and 1=0 ";
                            break;
                    }

                    if (Emptype != "X")
                    {
                        //Emp
                        sql = "Select Man_ID,Man_Name from EmployeeSV where CompanyCode='" + uu.CompanyId + "' and [Level]='" + LV.SqlQuote() + "'";
                        sql += ls_Cond;
                        sql += " order by Man_ID ";
                        DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                        dtE.TableName = "dtEmp";
                        ds.Tables.Add(dtE);
                    }
                }
                else
                {
                    sql = "select distinct a.GroupID,GroupName from GroupIDSV a inner join GroupProgramIDSV b ";
                    sql += "on a.CompanyCode=b.CompanyCode and a.GroupId=b.GroupID ";
                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.[status]='" + LV.SqlQuote() + "' order by GroupID ";
                    DataTable dtCG = PubUtility.SqlQry(sql, uu, "SYS");
                    dtCG.TableName = "dtCopyGroup";
                    ds.Tables.Add(dtCG);
                }


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/ChkGroupExist")]
        public ActionResult SystemSetup_ChkGroupExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkGroupExistOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string GroupID = rq["GroupID"];
                string LV = rq["LV"];
                string EditType = rq["EditType"];
                string sql = "";
                if (EditType == "A")
                { //群組新增
                    sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text] from GroupIDSV Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + GroupID + "'";
                    DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                    dtG.TableName = "dtGroupID";
                    ds.Tables.Add(dtG);
                }
                else
                { //群組刪除
                    sql = "select top 3 * from EmployeeSV Where CompanyCode='" + uu.CompanyId + "' and Man_Group='" + GroupID + "'";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtGroupEmp";
                    ds.Tables.Add(dtE);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/AddGroup")]
        public ActionResult SystemSetup_AddGroup()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddGroupOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("GroupIDSV");
                PubUtility.AddStringColumns(dtRec, "GroupID,GroupName,Status,CopyGroup");
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
                            string copygroup = dr["CopyGroup"].ToString().SqlQuote();
                            DataTable dtTmp = dtRec.Copy();
                            dtTmp.Columns.Remove("CopyGroup");
                            dbop.Add("GroupIDSV", dtTmp, uu, "SYS");
                            if (copygroup != "")
                            {
                                DataTable dtP = dbop.Query("select Companycode,'" + dr["GroupID"] + "' GroupID,ProgramID from GroupProgramIDSV where Companycode='" + uu.CompanyId + "' and GroupID='" + copygroup + "'", uu, "SYS");
                                if (dtP.Rows.Count > 0)
                                {
                                    dbop.Add("GroupProgramIDSV", dtP, uu, "SYS");
                                }
                            }

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
                sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text] from GroupIDSV a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And [Status]='" + dr["Status"].ToString().SqlQuote() + "' order by GroupID";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                sql = "select distinct a.GroupID,GroupName from GroupIDSV a inner join GroupProgramIDSV b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.GroupId=b.GroupID ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.[status]='" + dr["Status"].ToString().SqlQuote() + "' order by GroupID ";
                DataTable dtCG = PubUtility.SqlQry(sql, uu, "SYS");
                dtCG.TableName = "dtCopyGroup";
                ds.Tables.Add(dtCG);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroup")]
        public ActionResult SystemSetup_EditGroup()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("GroupIDSV");
                PubUtility.AddStringColumns(dtRec, "GroupID,GroupName,LV,EditType");
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
                            if (dr["EditType"].ToString() == "M")
                            {
                                sql = "update GroupIDSV set ";
                                sql += " GroupName='" + dr["GroupName"].ToString().SqlQuote() + "'";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And GroupID='" + dr["GroupID"].ToString().SqlQuote() + "' and [Status]='" + dr["LV"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "Delete from GroupIDSV";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And GroupID='" + dr["GroupID"].ToString().SqlQuote() + "' and [Status]='" + dr["LV"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                                sql = "Delete from GroupProgramIDSV";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }
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
                sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text]";
                sql += " from GroupIDSV a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and [Status]='" + dr["LV"].ToString().SqlQuote() + "' order by GroupID";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroupEmp")]
        public ActionResult SystemSetup_EditGroupEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("EmployeeSV");
                PubUtility.AddStringColumns(dtRec, "LV,GroupID,EmpStr,EditType");
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
                            if (dr["EditType"].ToString() == "A")
                            {   //增加群組人員
                                sql = "update EmployeeSV set ";
                                sql += " Man_Group='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And [Level]='" + dr["LV"].ToString().SqlQuote() + "' and Man_ID in ('" + dr["EmpStr"].ToString().SqlQuote().Replace(",", "','") + "')";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            { //刪除人員之群組
                                sql = "update EmployeeSV set ";
                                sql += " Man_Group=''";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Man_Group='" + dr["GroupID"].ToString().SqlQuote() + "' and [Level]='" + dr["LV"].ToString().SqlQuote() + "' and Man_ID in ('" + dr["EmpStr"].ToString().SqlQuote().Replace(",", "','") + "')";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
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
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetGroupProgram")]
        public ActionResult SystemSetup_GetGroupProgram()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGroupProgramOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string Group = rq["GroupID"];
                string lvtype = "";
                switch (LV)
                {
                    case "1":
                        lvtype = "3";
                        break;
                    case "2":
                        lvtype = "2";
                        break;
                    case "3":
                        lvtype = "1";
                        break;
                    default:
                        break;
                }
                //GroupProgramID
                string sql = "select 'P_'+a.ProgramId id,'S_'+b.SystemID parent from GroupProgramIDSV a inner join CompanyPIDSV b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.ProgramId=b.ProgramId ";
                sql += "inner join CompanySIDSV c on a.CompanyCode=c.CompanyCode and b.SystemId=c.SystemId ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and GroupID='" + Group.SqlQuote() + "' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "Order By c.OrderSequence,b.OrderSequence ";
                
                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroupProgram")]
        public ActionResult SystemSetup_EditGroupProgram()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupProgramOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtG = new DataTable("GPIDSV");
                PubUtility.AddStringColumns(dtG, "LV,GroupID,ProgramID");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtG);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow drG = dtG.Rows[0];

                string sql = "";
                string[] Programid = drG["ProgramID"].ToString().Split(",");
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //insert ProgramID至暫存表
                            sql = "Select ProgramID into #TempProgramID from GroupProgramIDSV where 1=0";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            foreach (string dr in Programid)
                            {
                                sql = "insert into #TempProgramID (ProgramID) values ('" + dr.ToString().SqlQuote().Replace("P_", "") + "')";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }

                            //刪除已取消的群組程式權限
                            sql = "Delete a from GroupProgramIDSV a full join #TempProgramID b on a.ProgramID=b.ProgramID ";
                            sql += "where a.Companycode = '" + uu.CompanyId + "' and GroupID = '" + drG["GroupID"].ToString().SqlQuote() + "' and b.ProgramID is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //新增本次設定的群組程式權限
                            sql = "insert into GroupProgramIDSV ";
                            sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),substring(Convert(varchar,getdate(),121),12,12),";
                            sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),substring(Convert(varchar,getdate(),121),12,12),";
                            sql += "'" + drG["GroupID"].ToString().SqlQuote() + "',a.ProgramID from #TempProgramID a left join GroupProgramIDSV b ";
                            sql += "on a.ProgramID=b.ProgramID and b.Companycode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                            sql += "where b.programid is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            dbop.ExecuteSql("drop table #TempProgramID", uu, "SYS");
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
                //GroupProgramID
                sql = "select 'P_'+ProgramId id from GroupProgramIDSV ";
                sql += "Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkEmpExist")]
        public ActionResult SystemSetup_ChkEmpExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkEmpExistOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SetType = rq["SetType"];
                string Setstr = rq["Setstr"].ToString().SqlQuote();
                string sql = "";
                if (SetType == "ID")
                { //ManID
                    sql = "select Man_ID from EmployeeSV (nolock) Where CompanyCode='" + uu.CompanyId + "' and Man_ID='" + Setstr + "'";
                }
                else
                { //ManEaddress
                    sql = "select Man_Eaddress from EmployeeSV (nolock) Where Man_Eaddress='" + Setstr + "'";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtEmp";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/AddEmp")]
        public ActionResult SystemSetup_AddEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("EmployeeSV");
                PubUtility.AddStringColumns(dtRec, "Man_ID,Man_Name,Password,Level,Man_ComeDay,Man_Eaddress,Man_Tel,WhNO");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                PubUtility.AddStringColumns(dtRec, "Man_forSell,Normal");
                DataRow dr = dtRec.Rows[0];
                if (dr["Level"].ToString() == "1") { dr["Man_forSell"] = "1"; }
                else { dr["Man_forSell"] = "0"; }
                dr["Normal"] = "1";

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            dbop.Add("EmployeeSV", dtRec, uu, "SYS");
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
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 正常登出
        [Route("UpdateLogOutY")]
        public ActionResult UpdateLogOutY()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutYOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='Y' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' and LoginDT='" + LoginDT + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 異常登出
        [Route("UpdateLogOutX")]
        public ActionResult UpdateLogOutX()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutXOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string GD_NO = rq["GD_NO"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='X' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 倒數登出
        [Route("js/UpdateLogOutT")]
        public ActionResult UpdateLogOutT()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutTOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='T' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' and LoginDT='" + LoginDT + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/10/24 檢查登出狀態
        [Route("js/ChkLogOut")]
        public ActionResult ChkLogOut()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkLogOutOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];

                string sql = "select * from ISAMLoginRecWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and LoginDT='" + LoginDT + "' ";
                DataTable dtLogin = PubUtility.SqlQry(sql, uu, "SYS");
                dtLogin.TableName = "dtLogin";
                ds.Tables.Add(dtLogin);

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
